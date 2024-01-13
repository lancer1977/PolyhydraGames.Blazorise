using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using PolyhydraGames.Core.Interfaces;
using ReactiveUI;

namespace PolyhydraGames.Core.AspNet.ViewModels;

public abstract class PolyComponentBase : PolyComponentBase<IViewModelAsync>
{
    public abstract override IViewModelAsync ViewModel { get; set; }
}


public abstract class PolyComponentBase<T> : ComponentBase, IViewFor<T>, INotifyPropertyChanged, ICanActivate, IDisposable
    where T : class, INotifyPropertyChanged
{
    private readonly Subject<Unit> _initSubject = new();
    private readonly Subject<Unit> _deactivateSubject = new();
    private readonly CompositeDisposable _compositeDisposable = new();
    private bool _disposedValue; // To detect redundant calls

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    public abstract T ViewModel { get; set; }

    /// <inheritdoc />
    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (T)value;
    }

    /// <inheritdoc />
    public IObservable<Unit> Activated => _initSubject.AsObservable();

    /// <inheritdoc />
    public IObservable<Unit> Deactivated => _deactivateSubject.AsObservable();

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) below.
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    protected override async Task OnInitializedAsync()
    {
        _initSubject.OnNext(Unit.Default);
        await base.OnInitializedAsync();

    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        switch (ViewModel)
        {
            case null:
                throw new NullReferenceException(nameof(ViewModel));
            case IViewModelAsync vm:
                await vm.StartAsync();
                break;
        }
    }


    /// <inheritdoc/>
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            // The following subscriptions are here because if they are done in OnInitialized, they conflict with certain JavaScript frameworks.
            var viewModelChanged =
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(x => x is not null)
                    .Publish()
                    .RefCount(2);

            viewModelChanged
                .Select(async _ => await InvokeAsync(StateHasChanged))
                .Subscribe()
                .DisposeWith(_compositeDisposable);

            viewModelChanged
                .WhereNotNull()
                .Select(x => Observable.FromEvent<PropertyChangedEventHandler?, Unit>(eventHandler =>
                                                                               {
                                                                                   void Handler(object? sender, PropertyChangedEventArgs e) => eventHandler(Unit.Default);
                                                                                   return Handler;
                                                                               }, eh => x.PropertyChanged += eh, eh => x.PropertyChanged -= eh))
                .Switch().Select(async (_) =>
                {
                    Debug.WriteLine("StateChanged");
                    await InvokeAsync(StateHasChanged);
                })
                .Subscribe()
                .DisposeWith(_compositeDisposable);

        }

        base.OnAfterRender(firstRender);
    }

    /// <summary>
    /// Invokes the property changed event.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Cleans up the managed resources of the object.
    /// </summary>
    /// <param name="disposing">If it is getting called by the Dispose() method rather than a finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            _initSubject.Dispose();
            _compositeDisposable.Dispose();
            _deactivateSubject.OnNext(Unit.Default);
        }

        _disposedValue = true;
    }
}

