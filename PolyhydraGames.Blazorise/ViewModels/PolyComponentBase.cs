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
using PolyhydraGames.Core.ReactiveUI;

namespace PolyhydraGames.BlazorComponents.ViewModels;

public abstract class ComponentViewModelBase : ViewModelAsyncBase
{
    protected virtual void OnInitialized() { }

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    ///
    /// Override this method if you will perform an asynchronous operation and
    /// want the component to refresh when that operation is completed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task OnInitializedAsync() => Task.CompletedTask;

    /// <summary>
    /// Method invoked when the component has received parameters from its parent in
    /// the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    protected virtual void OnParametersSet() { }

    /// <summary>
    /// Method invoked when the component has received parameters from its parent in
    /// the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task OnParametersSetAsync() => Task.CompletedTask;


    /// <summary>
    /// Method invoked after each time the component has rendered interactively and the UI has finished
    /// updating (for example, after elements have been added to the browser DOM). Any <see cref="ElementReference" />
    /// fields will be populated by the time this runs.
    ///
    /// This method is not invoked during prerendering or server-side rendering, because those processes
    /// are not attached to any live browser DOM and are already complete before the DOM is updated.
    /// </summary>
    /// <param name="firstRender">
    /// Set to <c>true</c> if this is the first time <see cref="OnAfterRender(bool)"/> has been invoked
    /// on this component instance; otherwise <c>false</c>.
    /// </param>
    /// <remarks>
    /// The <see cref="OnAfterRender(bool)"/> and <see cref="OnAfterRenderAsync(bool)"/> lifecycle methods
    /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
    /// Use the <paramref name="firstRender"/> parameter to ensure that initialization work is only performed
    /// once.
    /// </remarks>
    protected virtual void OnAfterRender( bool firstRender ) { }

    /// <summary>
    /// Method invoked after each time the component has been rendered interactively and the UI has finished
    /// updating (for example, after elements have been added to the browser DOM). Any <see cref="ElementReference" />
    /// fields will be populated by the time this runs.
    ///
    /// This method is not invoked during prerendering or server-side rendering, because those processes
    /// are not attached to any live browser DOM and are already complete before the DOM is updated.
    ///
    /// Note that the component does not automatically re-render after the completion of any returned <see cref="Task"/>,
    /// because that would cause an infinite render loop.
    /// </summary>
    /// <param name="firstRender">
    /// Set to <c>true</c> if this is the first time <see cref="OnAfterRender(bool)"/> has been invoked
    /// on this component instance; otherwise <c>false</c>.
    /// </param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    /// <remarks>
    /// The <see cref="OnAfterRender(bool)"/> and <see cref="OnAfterRenderAsync(bool)"/> lifecycle methods
    /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
    /// Use the <paramref name="firstRender"/> parameter to ensure that initialization work is only performed
    /// once.
    /// </remarks>
    protected virtual Task OnAfterRenderAsync( bool firstRender ) => Task.CompletedTask;

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

            ObservableMixins.WhereNotNull( viewModelChanged )
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

