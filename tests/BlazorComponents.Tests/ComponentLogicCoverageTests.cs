using System.Reflection;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PolyhydraGames.BlazorComponents.Components;
using PolyhydraGames.BlazorComponents.Dialog;

namespace BlazorComponents.Tests;

public class ComponentLogicCoverageTests
{
    [Fact]
    public void Picker_DefaultTitleUsesPlaceholderForNullValue()
    {
        var picker = new Picker<string> { PlaceHolder = "Pick one" };

        Assert.Equal("Pick one", picker.GetTitle(null));
    }

    [Fact]
    public void Picker_OnParametersSetCopiesBoundValue()
    {
        var picker = new Picker<string> { Value = "alpha" };

        Invoke(picker, "OnParametersSet");

        Assert.Equal("alpha", GetField<string>(picker, "_value"));
    }

    [Fact]
    public async Task Picker_OnValueChangedUpdatesValueAndRaisesCallbackOnlyForChanges()
    {
        var observed = new List<string?>();
        var picker = new Picker<string>
        {
            Value = "alpha",
            ValueChanged = EventCallback.Factory.Create<string?>(this, value => observed.Add(value))
        };
        Invoke(picker, "OnParametersSet");

        await InvokeAsync(picker, "OnValueChanged", "alpha");
        await InvokeAsync(picker, "OnValueChanged", "beta");

        Assert.Equal("beta", picker.Value);
        Assert.Equal(new[] { "beta" }, observed);
    }

    [Theory]
    [InlineData("not-an-int", 7)]
    [InlineData("4", 7)]
    [InlineData("12", 7)]
    [InlineData("9", 9)]
    public void MudPromptDialog_IntegerSubmitClosesWithParsedOrDefaultValue(string input, int expected)
    {
        var dialog = new MudPromptDialog
        {
            InitialValue = input,
            IsInteger = true,
            MinInt = 5,
            MaxInt = 10,
            DefaultInt = 7
        };
        var proxy = DialogProxy.Create();
        SetDialog(dialog, proxy.Instance);
        Invoke(dialog, "OnParametersSet");

        Invoke(dialog, "Submit");

        Assert.Equal("Close", proxy.LastMethodName);
        Assert.Equal(expected, proxy.LastDialogResult?.Data);
    }

    [Fact]
    public void MudPromptDialog_StringSubmitClosesWithEnteredValue()
    {
        var dialog = new MudPromptDialog { InitialValue = "hello" };
        var proxy = DialogProxy.Create();
        SetDialog(dialog, proxy.Instance);
        Invoke(dialog, "OnParametersSet");

        Invoke(dialog, "Submit");

        Assert.Equal("Close", proxy.LastMethodName);
        Assert.Equal("hello", proxy.LastDialogResult?.Data);
    }

    [Fact]
    public void MudPromptDialog_CancelCancelsDialog()
    {
        var dialog = new MudPromptDialog();
        var proxy = DialogProxy.Create();
        SetDialog(dialog, proxy.Instance);

        Invoke(dialog, "Cancel");

        Assert.Equal("Cancel", proxy.LastMethodName);
    }

    [Fact]
    public void MudSelectionDialog_SelectClosesWithSelectedItemAndCancelCancels()
    {
        var dialog = new MudSelectionDialog();
        var proxy = DialogProxy.Create();
        SetDialog(dialog, proxy.Instance);
        var selected = new object();

        Invoke(dialog, "Select", selected);
        Assert.Equal("Close", proxy.LastMethodName);
        Assert.Same(selected, proxy.LastDialogResult?.Data);

        Invoke(dialog, "Cancel");
        Assert.Equal("Cancel", proxy.LastMethodName);
    }

    private static void SetDialog(object component, IMudDialogInstance instance)
    {
        var property = component.GetType().GetProperty("MudDialog", BindingFlags.Instance | BindingFlags.NonPublic)!;
        property.SetValue(component, instance);
    }

    private static void Invoke(object target, string methodName, params object?[] args)
    {
        var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!;
        method.Invoke(target, args);
    }

    private static async Task InvokeAsync(object target, string methodName, params object?[] args)
    {
        var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!;
        var task = (Task)method.Invoke(target, args)!;
        await task;
    }

    private static T? GetField<T>(object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (T?)field.GetValue(target);
    }

    private class DialogProxy : DispatchProxy
    {
        public string? LastMethodName { get; private set; }
        public DialogResult? LastDialogResult { get; private set; }
        public IMudDialogInstance Instance { get; private set; } = default!;

        public static DialogProxy Create()
        {
            var instance = DispatchProxy.Create<IMudDialogInstance, DialogProxy>();
            var proxy = (DialogProxy)(object)instance;
            proxy.Instance = instance;
            return proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            LastMethodName = targetMethod?.Name;
            if (LastMethodName == "Close" && args?.Length > 0)
            {
                LastDialogResult = args[0] as DialogResult;
            }

            if (targetMethod?.ReturnType == typeof(Task))
            {
                return Task.CompletedTask;
            }

            if (targetMethod?.ReturnType == typeof(string))
            {
                return string.Empty;
            }

            if (targetMethod?.ReturnType == typeof(Guid))
            {
                return Guid.Empty;
            }

            return null;
        }
    }
}
