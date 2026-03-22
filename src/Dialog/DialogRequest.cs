namespace PolyhydraGames.BlazorComponents.Dialog;

 

public class DialogRequest
{
    private DialogRequestType _type;
    public TaskCompletionSource<DialogResult<string>>? AsString { get; private set; }
    public TaskCompletionSource<DialogResult<object>>? AsSelection { get; private set; }
    public TaskCompletionSource<DialogResult<int>>? AsInt { get; private set; }
    public TaskCompletionSource<DialogResult<bool>>? AsBool { get; private set; }
    public TaskCompletionSource? AsTask { get; private set; }
    public Task Task { get; private set; } = Task.CompletedTask;

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<object> Items { get; set; } = [];
    public string PositiveButton { get; set; } = string.Empty;
    public string NegativeButton { get; set; } = string.Empty;
    public int? MinInt { get; set; }
    public int? MaxInt { get; set; }
    public int? DefaultInt { get; set; }

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
                case DialogRequestType.Selection:
                    AsSelection = new TaskCompletionSource<DialogResult<object>>();
                    Task = AsSelection.Task;
                    break;
                default:
                    throw new ArgumentOutOfRangeException( nameof( value ), value, null );
            }
        }
    }
     
    public void SetSelection( object value )
    {
        if ( AsSelection is null )
            throw new InvalidOperationException( "Selection result requested for non-selection dialog type." );

        AsSelection.TrySetResult( new DialogResult<object>( value ) );
    }
    public void SetResult( string value )
    {
        if ( AsString is null )
            throw new InvalidOperationException( "String result requested for non-prompt dialog type." );

        AsString.TrySetResult( new DialogResult<string>( value ) );
    }

    public void SetResult( bool value )
    {
        if ( AsBool is null )
            throw new InvalidOperationException( "Boolean result requested for non-confirm dialog type." );

        AsBool.TrySetResult( new DialogResult<bool>( value ) );
    }

    public void SetResult( int value,bool cancelled = false )
    {
        if ( AsInt is null )
            throw new InvalidOperationException( "Int result requested for non-int dialog type." );

        if ( cancelled )
        {
            AsInt.TrySetResult( new DialogResult<int> { Result = value, Ok = false } );
        }
        else
        {
            AsInt.TrySetResult( new DialogResult<int>( value ) );
        }
     
    }

    

    public void SetResult()
    {
        if ( AsTask is null )
            throw new InvalidOperationException( "Void result requested for non-alert dialog type." );

        AsTask.TrySetResult();
    }
    public void Cancel()
    {
        AsTask?.TrySetResult();
        AsBool?.TrySetResult( new DialogResult<bool>() );
        AsString?.TrySetResult( new DialogResult<string>() );
        AsInt?.TrySetResult( new DialogResult<int>() );
        AsSelection?.TrySetResult( new DialogResult<object>() );
    }
}
