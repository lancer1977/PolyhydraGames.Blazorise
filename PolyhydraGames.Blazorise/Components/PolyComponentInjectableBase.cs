using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace PolyhydraGames.BlazorComponents.Components;

public class PolyComponentInjectableBase<T> : PolyComponentBase<T>
    where T : class, INotifyPropertyChanged
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