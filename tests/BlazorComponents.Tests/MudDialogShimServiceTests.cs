using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging.Abstractions;
using MudBlazor;
using PolyhydraGames.BlazorComponents.Dialog;
using MudDialogOptions = MudBlazor.DialogOptions;
using MudDialogParameters = MudBlazor.DialogParameters;
using MudDialogReference = MudBlazor.IDialogReference;
using MudDialogResult = MudBlazor.DialogResult;
using MudMessageBoxOptions = MudBlazor.MessageBoxOptions;

namespace BlazorComponents.Tests.Dialog;

public class MudDialogShimServiceTests
{
    [Fact]
    public async Task NotificationAsync_UsesRequestedButtonTextAndDefaultOptions()
    {
        var mudDialogs = new FakeMudDialogService
        {
            NextMessageBoxResult = true
        };
        var service = new MudDialogShimService( mudDialogs );

        await service.NotificationAsync( "hello", "title", "Close" );

        Assert.Equal( "title", mudDialogs.LastMessageBoxTitle );
        Assert.Equal( "hello", mudDialogs.LastMessageBoxMessage );
        Assert.Equal( "Close", mudDialogs.LastMessageBoxYesText );
        Assert.Equal( MaxWidth.Small, mudDialogs.LastMessageBoxOptions?.MaxWidth );
        Assert.True( mudDialogs.LastMessageBoxOptions?.FullWidth );
        Assert.True( mudDialogs.LastMessageBoxOptions?.CloseButton );
        Assert.True( mudDialogs.LastMessageBoxOptions?.CloseOnEscapeKey );
        Assert.True( mudDialogs.LastMessageBoxOptions?.BackdropClick );
    }

    [Fact]
    public async Task GetBooleanAsync_MapsMessageBoxResults()
    {
        var mudDialogs = new FakeMudDialogService();
        var service = new MudDialogShimService( mudDialogs );

        mudDialogs.NextMessageBoxResult = true;
        var yes = await service.GetBooleanAsync( "msg", "title" );
        Assert.True( yes.Ok );
        Assert.True( yes.Result );

        mudDialogs.NextMessageBoxResult = false;
        var no = await service.GetBooleanAsync( "msg", "title" );
        Assert.True( no.Ok );
        Assert.False( no.Result );

        mudDialogs.NextMessageBoxResult = null;
        var cancel = await service.GetBooleanAsync( "msg", "title" );
        Assert.False( cancel.Ok );
    }

    [Fact]
    public async Task GetIntAsync_WithBounds_ClampsDefaultAndPassesPromptParameters()
    {
        var mudDialogs = new FakeMudDialogService
        {
            NextDialogResult = MudDialogResult.Ok( 42 )
        };
        var service = new MudDialogShimService( mudDialogs );

        var result = await service.GetIntAsync( "Range test", 1, 10, 999 );

        Assert.Equal( typeof( MudPromptDialog ), mudDialogs.LastDialogComponentType );
        Assert.Equal( "Range test", mudDialogs.LastDialogTitle );
        Assert.True( mudDialogs.LastDialogParameters!.Get<bool>( nameof( MudPromptDialog.IsInteger ) ) );
        Assert.Equal( 1, mudDialogs.LastDialogParameters!.Get<int>( nameof( MudPromptDialog.MinInt ) ) );
        Assert.Equal( 10, mudDialogs.LastDialogParameters!.Get<int>( nameof( MudPromptDialog.MaxInt ) ) );
        Assert.Equal( 10, mudDialogs.LastDialogParameters!.Get<int>( nameof( MudPromptDialog.DefaultInt ) ) );
        Assert.Equal( "10", mudDialogs.LastDialogParameters!.Get<string>( nameof( MudPromptDialog.InitialValue ) ) );
        Assert.True( result.Ok );
        Assert.Equal( 42, result.Result );
    }

    [Fact]
    public async Task GetStringAsync_WhenCanceled_ReturnsNotOk()
    {
        var mudDialogs = new FakeMudDialogService
        {
            NextDialogResult = MudDialogResult.Cancel()
        };
        var service = new MudDialogShimService( mudDialogs );

        var result = await service.GetStringAsync( "msg", "title" );

        Assert.Equal( typeof( MudPromptDialog ), mudDialogs.LastDialogComponentType );
        Assert.False( result.Ok );
    }

    [Fact]
    public async Task InputBoxAsync_WhenSelectionMatchesType_ReturnsTypedValue()
    {
        var mudDialogs = new FakeMudDialogService
        {
            NextDialogResult = MudDialogResult.Ok<object>( "beta" )
        };
        var service = new MudDialogShimService( mudDialogs, NullLogger<MudDialogShimService>.Instance );

        var result = await service.InputBoxAsync( "title", new[] { "alpha", "beta" } );

        Assert.Equal( typeof( MudSelectionDialog ), mudDialogs.LastDialogComponentType );
        Assert.True( result.Ok );
        Assert.Equal( "beta", result.Result );
    }

    [Fact]
    public async Task InputBoxAsync_WhenSelectionTypeDiffers_ThrowsInvalidCastException()
    {
        var mudDialogs = new FakeMudDialogService
        {
            NextDialogResult = MudDialogResult.Ok<object>( 123 )
        };
        var service = new MudDialogShimService( mudDialogs );

        var ex = await Assert.ThrowsAsync<InvalidCastException>( () => service.InputBoxAsync<string>( "title", new[] { "alpha" } ) );

        Assert.Contains( "string", ex.Message, StringComparison.OrdinalIgnoreCase );
    }

    private sealed class FakeMudDialogService : MudBlazor.IDialogService
    {
        public string? LastMessageBoxTitle { get; private set; }
        public string? LastMessageBoxMessage { get; private set; }
        public string? LastMessageBoxYesText { get; private set; }
        public string? LastMessageBoxCancelText { get; private set; }
        public MudDialogOptions? LastMessageBoxOptions { get; private set; }
        public Type? LastDialogComponentType { get; private set; }
        public string? LastDialogTitle { get; private set; }
        public MudDialogParameters? LastDialogParameters { get; private set; }
        public MudDialogOptions? LastDialogOptions { get; private set; }
        public MudDialogResult? NextDialogResult { get; set; }
        public bool? NextMessageBoxResult { get; set; }

        public event Func<MudDialogReference, Task>? DialogInstanceAddedAsync;
        public event Action<MudDialogReference, MudDialogResult?>? OnDialogCloseRequested;

        public Task<MudDialogReference> ShowAsync<TComponent>() where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), null, null, null ) );

        public Task<MudDialogReference> ShowAsync<TComponent>( string? title ) where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), title, null, null ) );

        public Task<MudDialogReference> ShowAsync<TComponent>( string? title, MudDialogOptions options ) where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), title, null, options ) );

        public Task<MudDialogReference> ShowAsync<TComponent>( MudDialogParameters parameters ) where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), null, parameters, null ) );

        public Task<MudDialogReference> ShowAsync<TComponent>( string? title, MudDialogParameters parameters ) where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), title, parameters, null ) );

        public Task<MudDialogReference> ShowAsync<TComponent>( string? title, MudDialogParameters parameters, MudDialogOptions? options ) where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), title, parameters, options ) );

        public Task<MudDialogReference> ShowAsync( Type component ) =>
            Task.FromResult( CreateReference( component, null, null, null ) );

        public Task<MudDialogReference> ShowAsync( Type component, string? title ) =>
            Task.FromResult( CreateReference( component, title, null, null ) );

        public Task<MudDialogReference> ShowAsync( Type component, string? title, MudDialogOptions options ) =>
            Task.FromResult( CreateReference( component, title, null, options ) );

        public Task<MudDialogReference> ShowAsync( Type component, string? title, MudDialogParameters parameters ) =>
            Task.FromResult( CreateReference( component, title, parameters, null ) );

        public Task<MudDialogReference> ShowAsync( Type component, string? title, MudDialogParameters parameters, MudDialogOptions options ) =>
            Task.FromResult( CreateReference( component, title, parameters, options ) );

        public MudDialogReference CreateReference() => CreateReference( typeof( object ), null, null, null );

        public Task<bool?> ShowMessageBoxAsync( string? title, string message, string yesText = "OK", string? noText = null, string? cancelText = null, MudDialogOptions? options = null )
        {
            LastMessageBoxTitle = title;
            LastMessageBoxMessage = message;
            LastMessageBoxYesText = yesText;
            LastMessageBoxCancelText = cancelText;
            LastMessageBoxOptions = options;
            return Task.FromResult( NextMessageBoxResult );
        }

        public Task<bool?> ShowMessageBoxAsync( string? title, MarkupString markupMessage, string yesText = "OK", string? noText = null, string? cancelText = null, MudDialogOptions? options = null ) =>
            ShowMessageBoxAsync( title, markupMessage.Value, yesText, noText, cancelText, options );

        public Task<bool?> ShowMessageBoxAsync( MudMessageBoxOptions messageBoxOptions, MudDialogOptions? options = null ) =>
            ShowMessageBoxAsync( messageBoxOptions.Title, messageBoxOptions.Message ?? string.Empty, messageBoxOptions.YesText ?? "OK", messageBoxOptions.NoText, messageBoxOptions.CancelText, options );

        public void Close( MudDialogReference dialogReference ) => dialogReference.Close();

        public void Close( MudDialogReference dialogReference, MudDialogResult? result ) => dialogReference.Close( result );

        private MudDialogReference CreateReference( Type component, string? title, MudDialogParameters? parameters, MudDialogOptions? options )
        {
            LastDialogComponentType = component;
            LastDialogTitle = title;
            LastDialogParameters = parameters;
            LastDialogOptions = options;
            var reference = new FakeDialogReference( NextDialogResult );
            DialogInstanceAddedAsync?.Invoke( reference );
            return reference;
        }

        private sealed class FakeDialogReference : MudDialogReference
        {
            private readonly TaskCompletionSource<MudDialogResult?> _resultTask = new( TaskCreationOptions.RunContinuationsAsynchronously );
            private MudDialogResult? _result;

            public FakeDialogReference( MudDialogResult? initialResult )
            {
                RenderCompleteTaskCompletionSource = new TaskCompletionSource<bool>( TaskCreationOptions.RunContinuationsAsynchronously );
                if ( initialResult is not null )
                {
                    _result = initialResult;
                    _resultTask.TrySetResult( initialResult );
                    RenderCompleteTaskCompletionSource.TrySetResult( true );
                }
            }

            public Guid Id { get; } = Guid.NewGuid();
            public MudDialogOptions Options { get; private set; } = new();
            public RenderFragment? RenderFragment { get; set; }
            public object? Dialog { get; private set; }
            public TaskCompletionSource<bool> RenderCompleteTaskCompletionSource { get; }
            public Task<MudDialogResult?> Result => _resultTask.Task;

            public void Close() => Close( MudDialogResult.Cancel() );

            public void Close( MudDialogResult? result )
            {
                _result = result;
                _resultTask.TrySetResult( result );
                RenderCompleteTaskCompletionSource.TrySetResult( true );
            }

            public bool Dismiss( MudDialogResult? result )
            {
                Close( result );
                return true;
            }

            public void InjectRenderFragment( RenderFragment rf ) => RenderFragment = rf;

            public void InjectDialog( object inst ) => Dialog = inst;

            public void InjectOptions( MudDialogOptions options ) => Options = options;

            public Task<T?> GetReturnValueAsync<T>()
            {
                if ( _result is null || _result.Canceled || _result.Data is not T typed )
                {
                    return Task.FromResult<T?>( default );
                }

                return Task.FromResult<T?>( typed );
            }
        }
    }
}
