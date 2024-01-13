using System.Reactive.Linq;

namespace PolyhydraGames.BlazorComponents;

public static class ReactiveExtensions
{
    public static IObservable<T> WhereNotNull<T>( this IObservable<T?> source ) where T : class
    {
        return source.Where(x => x != null)!;
    }
}