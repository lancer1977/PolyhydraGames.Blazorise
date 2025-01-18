using Microsoft.AspNetCore.Components;
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