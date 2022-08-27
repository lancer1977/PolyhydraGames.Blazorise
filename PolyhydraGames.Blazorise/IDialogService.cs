namespace PolyhydraGames.Blazorise;

public interface IDialogService
{
    IObservable<DialogRequest> OnDialogRequest { get; }
    Task Alert(string message, string title);
    Task<bool?> Confirm(string message, string title, string confirmButton = "Yes", string denyButton = "No");
    Task<string?> Prompt(string message, string title, string confirmButton = "Ok", string cancelButton = "Cancel");
}