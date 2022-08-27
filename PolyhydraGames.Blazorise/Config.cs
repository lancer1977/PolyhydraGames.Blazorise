using Microsoft.Extensions.DependencyInjection;
 

namespace PolyhydraGames.BlazorComponents
{
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
        public static IServiceCollection AddBlazorComponents( this IServiceCollection serviceCollection  )
        {  
            serviceCollection.AddSingleton<IDialogService, DialogService>();
            return serviceCollection;
        }
    }
}
