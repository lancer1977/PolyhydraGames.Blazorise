using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PolyhydraGames.Blazorise;

public class DialogService : IDialogService
{
    public IObservable<DialogRequest> OnDialogRequest { get; }
    private Subject<DialogRequest> DialogRequest { get; } = new Subject<DialogRequest>();
    public DialogService()
    {
        OnDialogRequest = DialogRequest.AsObservable();
    }

    public async Task Alert(string message, string title)
    {
        var request = new DialogRequest()
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
        var request = new DialogRequest()
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
        var request = new DialogRequest()
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