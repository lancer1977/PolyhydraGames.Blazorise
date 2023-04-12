namespace PolyhydraGames.Blazor.Data.CurrentPage;

public interface ICurrentPage
{
    void SetName(string name);
    IObservable<string> NameChanged { get; }
}