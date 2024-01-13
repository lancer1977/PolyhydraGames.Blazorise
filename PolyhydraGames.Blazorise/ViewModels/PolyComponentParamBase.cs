using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace PolyhydraGames.Core.AspNet.ViewModels;
public class PolyComponentParamBase<T> : PolyComponentBase<T> 
    where T : class, INotifyPropertyChanged
{
    private T _viewModel;
    /// <inheritdoc />
    [Parameter]
    public override T ViewModel
    {
        get => _viewModel;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_viewModel, value))
            {
                return;
            }

            _viewModel = value;
            OnPropertyChanged();
        }
    }

}
 