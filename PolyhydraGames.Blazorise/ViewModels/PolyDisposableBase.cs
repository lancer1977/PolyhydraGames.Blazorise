using System.Reactive.Disposables; 
using Microsoft.AspNetCore.Components; 
using PolyhydraGames.Core.Interfaces;

namespace PolyhydraGames.Core.AspNet.ViewModels;

public abstract class PolyDisposableBase : ComponentBase, ICompositeDisposable, IDisposable
{
    public PolyDisposableBase()
    {
        Disposables = new CompositeDisposable();
    }
    public void Dispose()
    {
        (Disposables as CompositeDisposable).Dispose();
        
    }

    public ICollection<IDisposable> Disposables { get; init; }


}