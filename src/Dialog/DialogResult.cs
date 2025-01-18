using PolyhydraGames.Core.Interfaces;

namespace PolyhydraGames.BlazorComponents.Dialog;

public class DialogResult<T> : IDialogResult<T>
{
    public DialogResult()
    {
        Ok = false;
    }
    public DialogResult( T value )
    {
        Result = value;
        Ok = true;
    }
    public T Result { get; set; }
    public bool Ok { get; set; }
}