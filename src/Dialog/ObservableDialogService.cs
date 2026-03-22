using System.Globalization;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using PolyhydraGames.Core.Interfaces;

namespace PolyhydraGames.BlazorComponents.Dialog;

public class ObservableDialogService : IObservableDialogService, IDialogService
{
    private readonly ILogger<ObservableDialogService>? _logger;

    public IObservable<DialogRequest> OnDialogRequest { get; }

    private Subject<DialogRequest> DialogResultRequest { get; } = new();

    public ObservableDialogService( ILogger<ObservableDialogService>? logger = null )
    {
        _logger = logger;
        OnDialogRequest = DialogResultRequest;

        _logger?.LogInformation( "ObservableDialogService Created" );
    }

    private void PublishRequest( DialogRequest request )
    {
        _logger?.LogInformation( "Dialog request published. Type={Type}, Title={Title}", request.Type, request.Title );
        DialogResultRequest.OnNext( request );
    }

    public async Task NotificationAsync( string message, string title = "", string button = "OK" )
    {
        try
        {
            var request = new DialogRequest
            {
                Message = message,
                Title = title,
                PositiveButton = button,
                Type = DialogRequestType.Alert
            };

            PublishRequest( request );
            await request.Task;
        }
        catch ( Exception ex )
        {
            _logger?.LogCritical( ex, "NotificationAsync failed!" );
        }
    }

    public async Task<IDialogResult<int>> GetIntAsync( string title, string message = "", int def = 0 )
    {
        var request = new DialogRequest
        {
            Message = message,
            Title = title,
            PositiveButton = "Ok",
            NegativeButton = "Cancel",
            DefaultInt = def,
            Type = DialogRequestType.PromptInt
        };

        PublishRequest( request );
        return await request.AsInt!.Task;
    }

    public async Task<IDialogResult<int>> GetIntAsync( string title, int low, int high, int @default = 0 )
    {
        var clampedDefault = Math.Clamp( @default, low, high );
        var message = string.Format(
            CultureInfo.InvariantCulture,
            "Enter a number between {0} and {1}. Default: {2}.",
            low,
            high,
            clampedDefault );

        var request = new DialogRequest
        {
            Message = message,
            Title = title,
            PositiveButton = "Ok",
            NegativeButton = "Cancel",
            MinInt = low,
            MaxInt = high,
            DefaultInt = clampedDefault,
            Type = DialogRequestType.PromptInt
        };

        PublishRequest( request );
        return await request.AsInt!.Task;
    }

    public async Task<IDialogResult<bool>> GetBooleanAsync( string message, string title, string confirmButton = "OK", string labelCancel = "Cancel" )
    {
        var request = new DialogRequest
        {
            Message = message,
            Title = title,
            PositiveButton = confirmButton,
            NegativeButton = labelCancel,
            Type = DialogRequestType.Confirm
        };

        PublishRequest( request );
        return await request.AsBool!.Task;
    }

    public async Task<IDialogResult<string>> GetStringAsync( string message, string title, string labelOk = "OK", string labelCancel = "Cancel" )
    {
        var request = new DialogRequest
        {
            Message = message,
            Title = title,
            PositiveButton = labelOk,
            NegativeButton = labelCancel,
            Type = DialogRequestType.Prompt
        };

        PublishRequest( request );
        return await request.AsString!.Task;
    }

    public async Task<IDialogResult<T>> InputBoxAsync<T>( string title, IEnumerable<T> items )
    {
        var request = new DialogRequest
        {
            Items = items.Cast<object>().ToList(),
            Title = title,
            Type = DialogRequestType.Selection
        };

        PublishRequest( request );

        var result = await request.AsSelection!.Task;
        if ( !result.Ok || result.Result is null )
        {
            return new DialogResult<T>();
        }

        if ( result.Result is not T typedResult )
        {
            throw new InvalidCastException( $"Selection result could not be cast to {typeof( T ).Name}." );
        }

        return new DialogResult<T>
        {
            Result = typedResult,
            Ok = true
        };
    }

    public async Task ToastAsync( string message )
    {
        await NotificationAsync( message );
    }
}
