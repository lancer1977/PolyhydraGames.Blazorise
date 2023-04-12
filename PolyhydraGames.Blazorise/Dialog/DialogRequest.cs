namespace PolyhydraGames.BlazorComponents.Dialog;

 

public class DialogRequest
{
    private DialogRequestType _type;
    public TaskCompletionSource<DialogResult<string>> AsString { get; private set; }
    public TaskCompletionSource<DialogResult<int>> AsInt { get; private set; }
    public TaskCompletionSource<DialogResult<bool>> AsBool { get; private set; }
    public TaskCompletionSource AsTask { get; private set; }
    public Task Task { get; private set; }


    public DialogRequestType Type
    {
        get => _type;
        set
        {
            _type = value;
            switch ( value )
            {
                case DialogRequestType.Alert:
                    AsTask = new TaskCompletionSource();
                    Task = AsTask.Task;
                    break;
                case DialogRequestType.Confirm:
                    AsBool = new TaskCompletionSource<DialogResult<bool>>();
                    Task = AsBool.Task;
                    break;
                case DialogRequestType.Prompt:
                    AsString = new TaskCompletionSource<DialogResult<string>>();
                    Task = AsString.Task;
                    break;
                case DialogRequestType.PromptInt:
                    AsInt = new TaskCompletionSource<DialogResult<int>>();
                    Task = AsInt.Task;
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof( value ), value, null );
            }
        }
    }

    public string Title { get; set; }
    public string Message { get; set; }
    public string PositiveButton { get; set; }
    public string NegativeButton { get; set; }
    public void SetResult( string value )
    {
        AsString.SetResult( new DialogResult<string>( value ) );
    }

    public void SetResult( bool value )
    {
        AsBool?.SetResult( new DialogResult<bool>( value ) );
    }
    public void SetResult( int value )
    {
        AsInt.SetResult( new DialogResult<int>( value ) );
    }

    public void SetResult()
    {
        AsTask.SetResult();
    }
    public void Cancel()
    {
        AsTask?.SetResult();
        AsBool?.SetResult( new DialogResult<bool>() );
        AsString?.SetResult( new DialogResult<string>() );
        AsInt?.SetResult( new DialogResult<int>() );
    }
}