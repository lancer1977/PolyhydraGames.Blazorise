using PolyhydraGames.Core.Interfaces;

namespace PolyhydraGames.BlazorComponents.Dialog;

public interface IObservableDialogService : IDialogService
{
    public IObservable<DialogRequest> OnDialogRequest { get; }
}