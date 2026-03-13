# Using Dialogs in Core.Blazorise

The Core.Blazorise library provides a reactive dialog system for Blazor applications. This guide covers setup and usage for alert, confirm, and prompt dialogs.

## Installation

Add the NuGet package to your Blazor project:

```bash
dotnet add package PolyhydraGames.BlazorComponents
```

## Service Registration

Register the dialog service in your `Program.cs` or `Startup.cs`:

```csharp
using PolyhydraGames.BlazorComponents.Dialog;

// In your service registration
services.AddPolyDialogs();
```

This registers `IObservableDialogService` as a scoped service.

## Adding the Dialog Component

Place the `RichDialogs` component at the top level of your application, typically in `MainLayout.razor` or `App.razor`:

```razor
@layout MainLayout

<RichDialogs />

<!-- rest of your app -->
```

The `RichDialogs` component subscribes to `OnDialogRequest` and displays modal dialogs as needed.

## Using the Dialog Service

Inject `IObservableDialogService` where you need dialogs:

```csharp
@inject IObservableDialogService DialogService
```

### Alert Dialog

Show a simple notification:

```csharp
await DialogService.NotificationAsync("Operation completed successfully!");
await DialogService.NotificationAsync("File saved.", title: "Success");
```

### Confirm Dialog

Get user confirmation before an action:

```csharp
var result = await DialogService.GetBooleanAsync(
    message: "Are you sure you want to delete this item?",
    title: "Confirm Delete",
    confirmButton: "Delete",
    labelCancel: "Cancel"
);

if (result.Ok)
{
    // User confirmed - proceed with deletion
    await DeleteItemAsync();
}
```

### Prompt for Text

Ask the user for a text input:

```csharp
var result = await DialogService.GetStringAsync(
    message: "Enter your name:",
    title: "Name Required",
    labelOk: "Submit",
    labelCancel: "Skip"
);

if (result.Ok)
{
    var name = result.Result;
    // Use the entered name
}
```

### Prompt for Integer

Get an integer value from the user:

```csharp
var result = await DialogService.GetIntAsync(
    title: "Quantity",
    message: "Enter the quantity:",
    def: 1
);

if (result.Ok)
{
    var quantity = result.Result;
}
```

### Selection Dialog

Let the user pick from a list of options:

```csharp
var options = new[] { "Option A", "Option B", "Option C" };
var result = await DialogService.InputBoxAsync("Choose an option", options);

if (result.Ok)
{
    var selected = result.Result;
}
```

## Dialog Request Types

The `DialogRequestType` enum defines available dialog types:

| Type | Description |
|------|-------------|
| `Alert` | Simple notification with OK button |
| `Confirm` | Yes/No confirmation |
| `Prompt` | Text input with OK/Cancel |
| `PromptInt` | Integer input with OK/Cancel |
| `Selection` | Pick from a list of items |

## Integration with MVVM

The dialog service works well with ReactiveUI and MVVM patterns:

```csharp
public class MyViewModel : ReactiveObject
{
    private readonly IObservableDialogService _dialogs;
    
    public MyViewModel(IObservableDialogService dialogs)
    {
        _dialogs = dialogs;
    }
    
    public async Task DeleteItemAsync()
    {
        var confirm = await _dialogs.GetBooleanAsync(
            "This action cannot be undone.",
            "Confirm Delete"
        );
        
        if (confirm.Ok)
        {
            // Proceed with deletion
        }
    }
}
```

## Error Handling

Wrap dialog calls in try/catch for robust error handling:

```csharp
try
{
    await DialogService.NotificationAsync("Welcome!");
}
catch (Exception ex)
{
    Logger.LogError(ex, "Dialog failed");
}
```

## See Also

- `IObservableDialogService` interface - for reactive dialog handling
- `ObservableDialogService` class - default implementation
- `DialogRequestType` enum - available dialog types
- `RichDialogs` component - Blazor component for rendering dialogs