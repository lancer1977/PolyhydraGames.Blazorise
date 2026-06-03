using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudDialogResult = MudBlazor.DialogResult;
using PolyhydraGames.BlazorComponents.Dialog;

namespace BlazorComponents.Tests.Dialog;

public class MudDialogShimServiceCompatibilityTests
{
    [Fact]
    public async Task ToastAsync_DelegatesToNotificationAsync_WithDefaultButtonLabel()
    {
        var mudDialogs = new FakeMudDialogService
        {
            NextMessageBoxResult = true
        };
        var service = new MudDialogShimService( mudDialogs );

        await service.ToastAsync( "toast message" );

        Assert.Equal( string.Empty, mudDialogs.LastMessageBoxTitle );
        Assert.Equal( "toast message", mudDialogs.LastMessageBoxMessage );
        Assert.Equal( "OK", mudDialogs.LastMessageBoxYesText );
        Assert.Equal( MaxWidth.Small, mudDialogs.LastMessageBoxOptions?.MaxWidth );
    }

    private sealed class FakeMudDialogService : MudBlazor.IDialogService
    {
        public string? LastMessageBoxTitle { get; private set; }
        public string? LastMessageBoxMessage { get; private set; }
        public string? LastMessageBoxYesText { get; private set; }
        public string? LastMessageBoxCancelText { get; private set; }
        public DialogOptions? LastMessageBoxOptions { get; private set; }
        public MudDialogResult? NextDialogResult { get; set; }
        public bool? NextMessageBoxResult { get; set; }

        public event Func<IDialogReference, Task>? DialogInstanceAddedAsync;
        public event Action<IDialogReference, DialogResult?>? OnDialogCloseRequested;

        public Task<IDialogReference> ShowAsync<TComponent>() where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ) ) );

        public Task<IDialogReference> ShowAsync<TComponent>( string? title ) where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), title ) );

        public Task<IDialogReference> ShowAsync<TComponent>( string? title, DialogOptions options ) where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), title, options: options ) );

        public Task<IDialogReference> ShowAsync<TComponent>( DialogParameters parameters ) where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), parameters: parameters ) );

        public Task<IDialogReference> ShowAsync<TComponent>( string? title, DialogParameters parameters ) where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), title, parameters ) );

        public Task<IDialogReference> ShowAsync<TComponent>( string? title, DialogParameters parameters, DialogOptions? options ) where TComponent : IComponent =>
            Task.FromResult( CreateReference( typeof( TComponent ), title, parameters, options ) );

        public Task<IDialogReference> ShowAsync( Type component ) =>
            Task.FromResult( CreateReference( component ) );

        public Task<IDialogReference> ShowAsync( Type component, string? title ) =>
            Task.FromResult( CreateReference( component, title ) );

        public Task<IDialogReference> ShowAsync( Type component, string? title, DialogOptions options ) =>
            Task.FromResult( CreateReference( component, title, options: options ) );

        public Task<IDialogReference> ShowAsync( Type component, string? title, DialogParameters parameters ) =>
            Task.FromResult( CreateReference( component, title, parameters ) );

        public Task<IDialogReference> ShowAsync( Type component, string? title, DialogParameters parameters, DialogOptions options ) =>
            Task.FromResult( CreateReference( component, title, parameters, options ) );

        public IDialogReference CreateReference() => CreateReference( typeof( object ) );

        public Task<bool?> ShowMessageBoxAsync( string? title, string message, string yesText = "OK", string? noText = null, string? cancelText = null, DialogOptions? options = null )
        {
            LastMessageBoxTitle = title;
            LastMessageBoxMessage = message;
            LastMessageBoxYesText = yesText;
            LastMessageBoxCancelText = cancelText;
            LastMessageBoxOptions = options;
            return Task.FromResult( NextMessageBoxResult );
        }

        public Task<bool?> ShowMessageBoxAsync( string? title, MarkupString markupMessage, string yesText = "OK", string? noText = null, string? cancelText = null, DialogOptions? options = null ) =>
            ShowMessageBoxAsync( title, markupMessage.Value, yesText, noText, cancelText, options );

        public Task<bool?> ShowMessageBoxAsync( MessageBoxOptions messageBoxOptions, DialogOptions? options = null ) =>
            ShowMessageBoxAsync( messageBoxOptions.Title, messageBoxOptions.Message ?? string.Empty, messageBoxOptions.YesText ?? "OK", messageBoxOptions.NoText, messageBoxOptions.CancelText, options );

        public void Close( IDialogReference dialogReference ) => dialogReference.Close();

        public void Close( IDialogReference dialogReference, DialogResult? result ) => dialogReference.Close( result );

        private IDialogReference CreateReference( Type component, string? title = null, DialogParameters? parameters = null, DialogOptions? options = null )
        {
            var reference = new FakeDialogReference( NextDialogResult );
            DialogInstanceAddedAsync?.Invoke( reference );
            return reference;
        }

        private sealed class FakeDialogReference : IDialogReference
        {
            private readonly TaskCompletionSource<DialogResult?> _resultTask = new( TaskCreationOptions.RunContinuationsAsynchronously );

            public FakeDialogReference( DialogResult? initialResult )
            {
                RenderCompleteTaskCompletionSource = new TaskCompletionSource<bool>( TaskCreationOptions.RunContinuationsAsynchronously );
                if ( initialResult is not null )
                {
                    _resultTask.TrySetResult( initialResult );
                    RenderCompleteTaskCompletionSource.TrySetResult( true );
                }
            }

            public Guid Id { get; } = Guid.NewGuid();
            public DialogOptions Options { get; private set; } = new();
            public RenderFragment? RenderFragment { get; set; }
            public object? Dialog { get; private set; }
            public TaskCompletionSource<bool> RenderCompleteTaskCompletionSource { get; }
            public Task<DialogResult?> Result => _resultTask.Task;

            public void Close() => Close( DialogResult.Cancel() );

            public void Close( DialogResult? result )
            {
                _resultTask.TrySetResult( result );
                RenderCompleteTaskCompletionSource.TrySetResult( true );
            }

            public bool Dismiss( DialogResult? result )
            {
                Close( result );
                return true;
            }

            public void InjectRenderFragment( RenderFragment rf ) => RenderFragment = rf;

            public void InjectDialog( object inst ) => Dialog = inst;

            public void InjectOptions( DialogOptions options ) => Options = options;

            public Task<T?> GetReturnValueAsync<T>()
            {
                return _resultTask.Task.ContinueWith( task =>
                {
                    var result = task.Result;
                    if ( result is null || result.Canceled || result.Data is not T typed )
                    {
                        return default;
                    }

                    return typed;
                } );
            }
        }
    }
}
