using PolyhydraGames.Core.Interfaces;

namespace PolyhydraGames.BlazorComponents.Components;

public abstract class PolyComponentBase : PolyComponentBase<IViewModelAsync>
{
    public abstract override IViewModelAsync ViewModel { get; set; }
}