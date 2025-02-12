using Microsoft.Extensions.DependencyInjection;
using PolyhydraGames.BlazorComponents.Dialog;
using PolyhydraGames.Core.Interfaces;


namespace PolyhydraGames.BlazorComponents;

/// <summary>
/// Extension methods for building the blazorise options.
/// </summary>
public static class Config
{
    /// <summary>
    /// Register blazorise and configures the default behaviour.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddPolyDialogs( this IServiceCollection serviceCollection )
    {
        serviceCollection.AddSingleton<IDialogService, ObservableDialogService>();
        return serviceCollection;
    }
}