using System.Globalization;
using Microsoft.Extensions.Logging;
using MudBlazor;
using PolyhydraGames.Core.Interfaces;
using CoreDialogService = PolyhydraGames.Core.Interfaces.IDialogService;
using MudDialogService = MudBlazor.IDialogService;

namespace PolyhydraGames.BlazorComponents.Dialog;

public class MudDialogShimService : CoreDialogService
{
    private readonly MudDialogService _mudDialogs;
    private readonly ILogger<MudDialogShimService>? _logger;

    private static readonly DialogOptions DefaultOptions = new()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
        BackdropClick = true
    };

    public MudDialogShimService( MudDialogService mudDialogs, ILogger<MudDialogShimService>? logger = null )
    {
        _mudDialogs = mudDialogs;
        _logger = logger;
    }

    public async Task NotificationAsync( string message, string title = "", string button = "OK" )
    {
        await _mudDialogs.ShowMessageBoxAsync(
            title: title,
            message: message,
            yesText: button,
            options: DefaultOptions );
    }

    public async Task<IDialogResult<int>> GetIntAsync( string title, string message = "", int def = 0 )
    {
        var parameters = new DialogParameters
        {
            { nameof(MudPromptDialog.Message), message },
            { nameof(MudPromptDialog.InitialValue), def.ToString( CultureInfo.InvariantCulture ) },
            { nameof(MudPromptDialog.IsInteger), true },
            { nameof(MudPromptDialog.DefaultInt), def },
            { nameof(MudPromptDialog.PositiveButton), "Ok" },
            { nameof(MudPromptDialog.NegativeButton), "Cancel" }
        };

        var dialog = await _mudDialogs.ShowAsync<MudPromptDialog>( title, parameters, DefaultOptions );
        var result = await dialog.Result;
        if ( result is null || result.Canceled || result.Data is not int intValue )
        {
            return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<int>();
        }

        return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<int>( intValue );
    }

    public async Task<IDialogResult<int>> GetIntAsync( string title, int low, int high, int @default = 0 )
    {
        var clampedDefault = Math.Clamp( @default, low, high );
        var message = string.Format(
            CultureInfo.InvariantCulture,
            "Enter a number between {0} and {1}. Default: {2}.",
            low,
            high,
            clampedDefault );

        var parameters = new DialogParameters
        {
            { nameof(MudPromptDialog.Message), message },
            { nameof(MudPromptDialog.InitialValue), clampedDefault.ToString( CultureInfo.InvariantCulture ) },
            { nameof(MudPromptDialog.IsInteger), true },
            { nameof(MudPromptDialog.MinInt), low },
            { nameof(MudPromptDialog.MaxInt), high },
            { nameof(MudPromptDialog.DefaultInt), clampedDefault },
            { nameof(MudPromptDialog.PositiveButton), "Ok" },
            { nameof(MudPromptDialog.NegativeButton), "Cancel" }
        };

        var dialog = await _mudDialogs.ShowAsync<MudPromptDialog>( title, parameters, DefaultOptions );
        var result = await dialog.Result;
        if ( result is null || result.Canceled || result.Data is not int intValue )
        {
            return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<int>();
        }

        return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<int>( intValue );
    }

    public async Task<IDialogResult<bool>> GetBooleanAsync( string message, string title, string confirmButton = "OK", string labelCancel = "Cancel" )
    {
        var result = await _mudDialogs.ShowMessageBoxAsync(
            title: title,
            message: message,
            yesText: confirmButton,
            cancelText: labelCancel,
            options: DefaultOptions );

        if ( result == true )
        {
            return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<bool>( true );
        }

        if ( result == false )
        {
            return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<bool>( false );
        }

        return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<bool>();
    }

    public async Task<IDialogResult<string>> GetStringAsync( string message, string title, string labelOk = "OK", string labelCancel = "Cancel" )
    {
        var parameters = new DialogParameters
        {
            { nameof(MudPromptDialog.Message), message },
            { nameof(MudPromptDialog.InitialValue), string.Empty },
            { nameof(MudPromptDialog.IsInteger), false },
            { nameof(MudPromptDialog.PositiveButton), labelOk },
            { nameof(MudPromptDialog.NegativeButton), labelCancel }
        };

        var dialog = await _mudDialogs.ShowAsync<MudPromptDialog>( title, parameters, DefaultOptions );
        var result = await dialog.Result;
        if ( result is null || result.Canceled || result.Data is not string value )
        {
            return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<string>();
        }

        return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<string>( value );
    }

    public async Task<IDialogResult<T>> InputBoxAsync<T>( string title, IEnumerable<T> items )
    {
        var list = items.Cast<object>().ToList();
        var parameters = new DialogParameters
        {
            { nameof(MudSelectionDialog.Items), list },
            { nameof(MudSelectionDialog.NegativeButton), "Cancel" }
        };

        var dialog = await _mudDialogs.ShowAsync<MudSelectionDialog>( title, parameters, DefaultOptions );
        var result = await dialog.Result;
        if ( result is null || result.Canceled || result.Data is null )
        {
            return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<T>();
        }

        if ( result.Data is not T typed )
        {
            _logger?.LogError( "Selection result type mismatch. Expected={ExpectedType}, Actual={ActualType}", typeof( T ).Name, result.Data.GetType().Name );
            throw new InvalidCastException( $"Selection result could not be cast to {typeof( T ).Name}." );
        }

        return new global::PolyhydraGames.BlazorComponents.Dialog.DialogResult<T>( typed );
    }

    public async Task ToastAsync( string message )
    {
        await NotificationAsync( message );
    }
}
