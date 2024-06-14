using System.Reactive.Linq;
using System.Reactive.Subjects;
using PolyhydraGames.BlazorComponents.Dialog;

namespace PolyhydraGames.BlazorComponents.SimpleDialogs;

public class SimpleDialogService : IBlazorDialogService
{
    public IObservable<SimpleDialogRequest> OnDialogRequest { get; }
    private Subject<SimpleDialogRequest> DialogRequest { get; } = new();
    public SimpleDialogService()
    {
        OnDialogRequest = DialogRequest.AsObservable();
    }

    public async Task Alert(string message, string title)
    {
        var request = new SimpleDialogRequest()
        {
            Message = message,
            Title = title,
            PositiveButton = "OK",
            Type = DialogRequestType.Alert

        };
        DialogRequest.OnNext(request);
        await request.TCS.Task;
        //await _helper.Alert(title + ": " + message);
    }

    public async Task<bool?> Confirm(string message, string title, string confirmButton, string denyButton)
    {
        var request = new SimpleDialogRequest()
        {
            Message = message,
            Title = title,
            PositiveButton = confirmButton,
            NegativeButton = denyButton,
            Type = DialogRequestType.Confirm

        };
        DialogRequest.OnNext(request);
        return await request.TCSBool.Task;
    }

    public async Task<string?> Prompt(string message, string title, string confirmButton, string cancelButton)
    {
        var request = new SimpleDialogRequest()
        {
            Message = message,
            Title = title,
            PositiveButton = confirmButton,
            NegativeButton = cancelButton,
            Type = DialogRequestType.Prompt

        };
        DialogRequest.OnNext(request);
        return await request.TCSString.Task;
    }
}