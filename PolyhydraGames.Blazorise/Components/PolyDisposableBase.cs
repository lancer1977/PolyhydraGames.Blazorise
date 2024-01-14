using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;
using PolyhydraGames.Core.Interfaces;

namespace PolyhydraGames.BlazorComponents.Components;

public abstract class PolyDisposableBase : ComponentBase, IDisposable
{


    public CompositeDisposable Disposables { get;   } = new CompositeDisposable();

    public void Dispose()
    {
        Disposables.Dispose();
    }
}