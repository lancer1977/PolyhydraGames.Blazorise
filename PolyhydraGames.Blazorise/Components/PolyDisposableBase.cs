using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;
using PolyhydraGames.Core.Interfaces;

namespace PolyhydraGames.BlazorComponents.Components;

public abstract class PolyDisposableBase : ComponentBase, ICompositeDisposable
{
    public PolyDisposableBase()
    {
        Disposables = new CompositeDisposable();
    }
    public void Dispose()
    {
        ( Disposables as CompositeDisposable ).Dispose();

    }

    public ICollection<IDisposable> Disposables { get; init; }


}