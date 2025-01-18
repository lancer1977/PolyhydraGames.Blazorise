using PolyhydraGames.BlazorComponents.Dialog;

namespace PolyhydraGames.BlazorComponents.SimpleDialogs;

public class SimpleDialogRequest
{
    private DialogRequestType _type;
    public TaskCompletionSource<string?> TCSString { get; private set; }
    public TaskCompletionSource<bool?> TCSBool { get; private set; }
    public TaskCompletionSource TCS { get; private set; }
    public Task Task { get; private set; }


    public DialogRequestType Type
    {
        get => _type;
        set
        {
            _type = value;
            switch (value)
            {
                case DialogRequestType.Alert:
                    TCS = new TaskCompletionSource();
                    Task = TCS.Task;
                    break;
                case DialogRequestType.Confirm:
                    TCSBool = new TaskCompletionSource<bool?>();
                    Task = TCSBool.Task;
                    break;
                case DialogRequestType.Prompt:
                    TCSString = new TaskCompletionSource<string>();
                    Task = TCSString.Task;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    public string Title { get; set; }
    public string Message { get; set; }
    public string PositiveButton { get; set; }
    public string NegativeButton { get; set; }
    public void SetResult(string value)
    {
        TCSString.SetResult(value);
    }

    public void SetResult(bool value)
    {
        TCSBool.SetResult(value);
    }

    public void SetResult()
    {
        TCS.SetResult();
    }
    public void Cancel()
    {
        TCS?.SetResult();
        TCSBool?.SetResult(null);
        TCSString?.SetResult(null);
    }
}