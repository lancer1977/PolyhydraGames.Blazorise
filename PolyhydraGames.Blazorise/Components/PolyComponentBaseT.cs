using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using PolyhydraGames.BlazorComponents.QueryStrings;

namespace PolyhydraGames.BlazorComponents.Components;

public abstract class QueryPolyComponentBase<T> : PolyComponentBase<T>
    where T : class, INotifyPropertyChanged
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    public override Task SetParametersAsync( ParameterView parameters )
    {
        // 👇 Read the value of each property decorated by [QueryStringParameter] from the query string
        this.SetParametersFromQueryString( NavigationManager );
        return base.SetParametersAsync( parameters );
    }
}


// Requires Microsoft.AspNetCore.WebUtilities to edit the query string
// <PackageReference Include="Microsoft.AspNetCore.WebUtilities" Version="2.2.0" />