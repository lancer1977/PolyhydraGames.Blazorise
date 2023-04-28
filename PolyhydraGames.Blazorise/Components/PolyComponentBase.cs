using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using PolyhydraGames.Core.Interfaces;
using ReactiveUI;

namespace PolyhydraGames.BlazorComponents.Components;

public abstract class PolyComponentBase<T> : ComponentBase, IViewFor<T>, INotifyPropertyChanged, ICanActivate, IDisposable
    where T : class, INotifyPropertyChanged
{

    private readonly Subject<Unit> _initSubject = new();
    [SuppressMessage( "Design", "CA2213: Dispose object", Justification = "Used for deactivation." )]
    private readonly Subject<Unit> _deactivateSubject = new();
    protected readonly CompositeDisposable CompositeDisposable = new();
    
    
    public ICommand RefreshCommand { get; set; }

    public PolyComponentBase()
    {
        RefreshCommand = ReactiveCommand.CreateFromTask( async () =>
        {
            if ( ViewModel is IViewModelAsync vm )
                await vm.RefreshAsync(); 
        } );
    }
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
        Dispose( true );
        GC.SuppressFinalize( this );
    }


    protected override async Task OnInitializedAsync()
    {
        _initSubject.OnNext( Unit.Default );
        await base.OnInitializedAsync();

    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if ( ViewModel is IViewModelAsync vm )
            await vm.StartAsync();
    }
    
    /// <inheritdoc/>
    protected override void OnAfterRender( bool firstRender )
    {
        if ( firstRender )
        {
            // The following subscriptions are here because if they are done in OnInitialized, they conflict with certain JavaScript frameworks.
            var viewModelChanged =
                this.WhenAnyValue( x => x.ViewModel )
                    .Where( x => x is not null )
                    .Publish()
                    .RefCount( 2 );

            viewModelChanged
                .Subscribe( _ =>
                {
                    InvokeAsync( StateHasChanged );
                } )
                .DisposeWith( CompositeDisposable );

            viewModelChanged
                .WhereNotNull()
                .Select( x => Observable.FromEvent<PropertyChangedEventHandler?, Unit>(
                    eventHandler =>
                    {
                        void Handler( object? sender, PropertyChangedEventArgs e ) => eventHandler( Unit.Default );
                        return Handler;
                    },
                    eh => x.PropertyChanged += eh,
                    eh => x.PropertyChanged -= eh ) )
                .Switch()
                .Subscribe( _ =>
                {
                    Debug.WriteLine( "StateChanged" );
                    InvokeAsync( StateHasChanged );
                } )
                .DisposeWith( CompositeDisposable );

        }

        base.OnAfterRender( firstRender );
    }

    /// <summary>
    /// Invokes the property changed event.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null ) => PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );

    /// <summary>
    /// Cleans up the managed resources of the object.
    /// </summary>
    /// <param name="disposing">If it is getting called by the Dispose() method rather than a finalizer.</param>
    protected virtual void Dispose( bool disposing )
    {
        if ( !_disposedValue )
        {
            if ( disposing )
            {
                _initSubject.Dispose();
                CompositeDisposable.Dispose();
                _deactivateSubject.OnNext( Unit.Default );
            }

            _disposedValue = true;
        }
    }

}

public abstract class PolyComponentBase : PolyComponentBase<IViewModelAsync>
{
    public abstract override IViewModelAsync ViewModel { get; set; }
}