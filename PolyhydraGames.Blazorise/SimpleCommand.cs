using System.Windows.Input;

namespace PolyhydraGames.Blazorise;

public class SimpleCommand : ICommand
{
    private readonly Action _act;
    private readonly Func<Task> _asyncTask;
    public SimpleCommand(Action act)
    {
        _act = act;
    }
    public SimpleCommand(  Func<Task> asyncTask)  
    {
        _asyncTask = asyncTask;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _act?.Invoke();
        _asyncTask?.Invoke();
    }

    public event EventHandler? CanExecuteChanged;
}