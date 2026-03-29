using Microsoft.Extensions.DependencyInjection;
using PolyhydraGames.BlazorComponents.Dialog;
using PolyhydraGames.Core.Interfaces;


namespace PolyhydraGames.BlazorComponents;

/// <summary>
/// Extension methods for registering dialog services.
/// </summary>
public static class Config
{
    /// <summary>
    /// Registers the dialog abstractions and default implementation.
    /// </summary>
    public static IServiceCollection AddPolyDialogs( this IServiceCollection serviceCollection )
    {
        serviceCollection.AddScoped<MudDialogShimService>();
        serviceCollection.AddScoped<IDialogService>( x => x.GetRequiredService<MudDialogShimService>() );

        return serviceCollection;
    }
}
