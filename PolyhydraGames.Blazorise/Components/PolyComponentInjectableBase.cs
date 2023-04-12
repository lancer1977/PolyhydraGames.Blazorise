using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using PolyhydraGames.Core.Interfaces;

namespace PolyhydraGames.BlazorComponents.Components;

public class PolyComponentInjectableBase<T> : PolyComponentBase<T>
    where T : class, INotifyPropertyChanged, IViewModelAsync
{
    private T _viewModel;
    /// <inheritdoc />
    [Inject]
    public override T ViewModel
    {
        get => _viewModel;
        set
        {
            if ( EqualityComparer<T>.Default.Equals( _viewModel, value ) )
            {
                return;
            }

            _viewModel = value;
            OnPropertyChanged();
        }
    }

}