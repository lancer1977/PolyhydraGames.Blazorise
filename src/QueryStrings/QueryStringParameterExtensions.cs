using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace PolyhydraGames.BlazorComponents.QueryStrings;
public static class QueryStringParameterExtensions
{
    // Apply the values from the query string to the current component
    public static void SetParametersFromQueryString<T>( this T component, NavigationManager navigationManager )
        where T : ComponentBase
    {
        if ( !Uri.TryCreate( navigationManager.Uri, UriKind.RelativeOrAbsolute, out var uri ) )
            throw new InvalidOperationException( "The current url is not a valid URI. Url: " + navigationManager.Uri );

        // Parse the query string
        Dictionary<string, StringValues> queryString = QueryHelpers.ParseQuery( uri.Query );

        // Enumerate all properties of the component
        foreach ( var property in GetProperties<T>() )
        {
            // Get the name of the parameter to read from the query string
            var parameterName = GetQueryStringParameterName( property );
            if ( parameterName == null )
                continue; // The property is not decorated by [QueryStringParameterAttribute]

            if ( queryString.TryGetValue( parameterName, out var value ) )
            {
                // Convert the value from string to the actual property type
                var convertedValue = ConvertValue( value, property.PropertyType );
                property.SetValue( component, convertedValue );
            }
        }
    }

    // Apply the values from the component to the query string
    public static void UpdateQueryString<T>( this T component, NavigationManager navigationManager )
        where T : ComponentBase
    {
        if ( !Uri.TryCreate( navigationManager.Uri, UriKind.RelativeOrAbsolute, out var uri ) )
            throw new InvalidOperationException( "The current url is not a valid URI. Url: " + navigationManager.Uri );

        // Fill the dictionary with the parameters of the component
        Dictionary<string, StringValues> parameters = QueryHelpers.ParseQuery( uri.Query );
        foreach ( var property in GetProperties<T>() )
        {
            var parameterName = GetQueryStringParameterName( property );
            if ( parameterName == null )
                continue;

            var value = property.GetValue( component );
            if ( value is null )
            {
                parameters.Remove( parameterName );
            }
            else
            {
                var convertedValue = ConvertToString( value );
                parameters[parameterName] = convertedValue;
            }
        }

        // Compute the new URL
        var newUri = uri.GetComponents( UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.UriEscaped );
        foreach ( var parameter in parameters )
        {
            foreach ( var value in parameter.Value )
            {
                if ( value is not null )
                {
                    newUri = QueryHelpers.AddQueryString( newUri, parameter.Key, value );
                }
            }
        }

        navigationManager.NavigateTo( newUri );
    }

    private static object? ConvertValue( StringValues value, Type type )
    {
        // Handle empty or null values
        if (StringValues.IsNullOrEmpty(value))
        {
            // If the type is nullable, return null; otherwise return default
            return type.IsNullableType() ? null : GetDefaultValue(type);
        }

        var stringValue = value[0]!;

        // Handle nullable types - extract the underlying type
        var underlyingType = type.IsNullableType() ? Nullable.GetUnderlyingType(type)! : type;

        // Handle enums
        if (underlyingType.IsEnum)
        {
            return Enum.Parse(underlyingType, stringValue, ignoreCase: true);
        }

        // Handle Guid
        if (underlyingType == typeof(Guid))
        {
            return Guid.Parse(stringValue);
        }

        // Handle other primitives using Convert.ChangeType
        return Convert.ChangeType(stringValue, underlyingType, CultureInfo.InvariantCulture);
    }

    private static object? GetDefaultValue(Type type)
    {
        if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }

    private static bool IsNullableType(this Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    private static string ConvertToString( object? value )
    {
        if (value is null)
            return string.Empty;

        // Handle enums - use the underlying value or the enum name
        var type = value.GetType();
        if (type.IsEnum)
        {
            return value.ToString()!;
        }

        return Convert.ToString( value, CultureInfo.InvariantCulture ) ?? string.Empty;
    }

    private static PropertyInfo[] GetProperties<T>()
    {
        return typeof(T).GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
    }

    private static string? GetQueryStringParameterName( PropertyInfo property )
    {
        var attribute = property.GetCustomAttribute<QueryStringParameterAttribute>();
        if ( attribute == null )
            return null;

        return attribute.Name ?? property.Name;
    }
}
