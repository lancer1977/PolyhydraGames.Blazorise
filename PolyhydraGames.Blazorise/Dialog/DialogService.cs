using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using PolyhydraGames.Core.Interfaces;

namespace PolyhydraGames.BlazorComponents.Dialog;

public class DialogService : IObservableDialogService, IDialogService
{
    public IObservable<DialogRequest> OnDialogRequest { get; }
    private Subject<DialogRequest> DialogResultRequest { get; } = new();
    public DialogService( ILogger<DialogService>? logger = null )
    {
        OnDialogRequest = DialogResultRequest.AsObservable();
        if ( logger == null )
        {
            return;
        }

        logger.LogInformation( "DialogService Created" );
        OnDialogRequest.Subscribe( x =>
        {
            logger.LogInformation( x.Message );
        }
        );
    }



    public async Task NotificationAsync( string message, string title = "", string button = "OK" )
    {
        var request = new DialogRequest()
        {
            Message = message,
            Title = title,
            PositiveButton = "OK",
            Type = DialogRequestType.Alert

        };
        DialogResultRequest.OnNext( request );
        await request.AsTask.Task;
        //await _helper.Alert(title + ": " + message);
    }

    public async Task<IDialogResult<int>> GetIntAsync( string title, string message = "", int def = 0 )
    {
        var request = new DialogRequest()
        {
            Message = message,
            Title = title,
            PositiveButton = "Ok",
            NegativeButton = "Cancel",
            Type = DialogRequestType.Confirm

        };
        DialogResultRequest.OnNext( request );
        return await request.AsInt.Task;
    }

    public async Task<IDialogResult<int>> GetIntAsync( string title, int low, int high, int @default = 0 )
    {
        var request = new DialogRequest()
        {
            Title = title,
            PositiveButton = "Ok",
            NegativeButton = "Cancel",
            Type = DialogRequestType.Confirm

        };
        DialogResultRequest.OnNext( request );
        return await request.AsInt.Task;
    }

    public async Task<IDialogResult<bool>> GetBooleanAsync( string message, string title, string confirmButton = "OK", string labelCancel = "Cancel" )
    {
        var request = new DialogRequest()
        {
            Message = message,
            Title = title,
            PositiveButton = confirmButton,
            NegativeButton = labelCancel,
            Type = DialogRequestType.Confirm

        };
        DialogResultRequest.OnNext( request );
        return await request.AsBool.Task;
    }

    public async Task<IDialogResult<string>> GetStringAsync( string message, string title, string labelOk = "OK", string labelCancel = "Cancel" )
    {
        var request = new DialogRequest()
        {
            Message = message,
            Title = title,
            PositiveButton = labelOk,
            NegativeButton = labelCancel,
            Type = DialogRequestType.Confirm

        };
        DialogResultRequest.OnNext( request );
        return await request.AsString.Task;
    }

    public async Task<IDialogResult<T>> InputBoxAsync<T>( string title, IEnumerable<T> items )
    {
        var request = new DialogRequest()
        {
            Items = items.Cast<object>().ToList(),
            //Message = message,
            Title = title,
            Type = DialogRequestType.Selection

        };
        DialogResultRequest.OnNext( request );
        var result = await request.AsSelection.Task;
        var resultOfT = new DialogResult<T>
        {
            Result = (T)result.Result,
            Ok = result.Ok
        };

        return resultOfT;
    }

    public async Task ToastAsync( string message )
    {
        await NotificationAsync( message );
    }
}