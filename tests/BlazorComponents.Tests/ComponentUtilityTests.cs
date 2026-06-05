using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using PolyhydraGames.BlazorComponents;
using PolyhydraGames.BlazorComponents.CurrentPage;
using PolyhydraGames.BlazorComponents.Dialog;
using PolyhydraGames.Core.Interfaces;
using CoreDialogService = PolyhydraGames.Core.Interfaces.IDialogService;

namespace BlazorComponents.Tests;

public sealed class ComponentUtilityTests
{
    [Fact]
    public void DialogResult_DefaultConstructorMarksResultNotOk()
    {
        var result = new DialogResult<string>();

        Assert.False(result.Ok);
        Assert.Null(result.Result);
    }

    [Fact]
    public void DialogResult_ValueConstructorMarksResultOk()
    {
        var result = new DialogResult<string>("accepted");

        Assert.True(result.Ok);
        Assert.Equal("accepted", result.Result);
    }

    [Fact]
    public async Task CurrentPageService_PublishesNamesInOrder()
    {
        var service = new CurrentPageService();
        var received = new List<string>();
        using var subscription = service.NameChanged.Subscribe(received.Add);

        service.SetName("Home");
        service.SetName("Projects");
        await Task.Delay(1);

        Assert.Equal(new[] { "Home", "Projects" }, received);
    }

    [Fact]
    public async Task WhereNotNull_FiltersNullReferenceValues()
    {
        var source = new string?[] { "alpha", null, "beta" }.ToObservable();

        var result = await source.WhereNotNull().ToList();

        Assert.Equal(new[] { "alpha", "beta" }, result);
    }

    [Fact]
    public void AddPolyDialogs_RegistersShimAndCoreDialogAbstraction()
    {
        var services = new ServiceCollection();

        services.AddPolyDialogs();

        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(MudDialogShimService)
            && descriptor.Lifetime == ServiceLifetime.Scoped);
        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(CoreDialogService)
            && descriptor.Lifetime == ServiceLifetime.Scoped
            && descriptor.ImplementationFactory is not null);
    }

    [Fact]
    public async Task DialogRequest_AlertCompletesVoidTask()
    {
        var request = new DialogRequest { Type = DialogRequestType.Alert };

        request.SetResult();

        await request.Task;
        Assert.True(request.Task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task DialogRequest_PromptAndConfirmAndSelectionSetTypedResults()
    {
        var prompt = new DialogRequest { Type = DialogRequestType.Prompt };
        var confirm = new DialogRequest { Type = DialogRequestType.Confirm };
        var selection = new DialogRequest { Type = DialogRequestType.Selection };
        var selected = new object();

        prompt.SetResult("typed");
        confirm.SetResult(true);
        selection.SetSelection(selected);

        Assert.Equal("typed", (await prompt.AsString!.Task).Result);
        Assert.True((await confirm.AsBool!.Task).Result);
        Assert.Same(selected, (await selection.AsSelection!.Task).Result);
    }

    [Theory]
    [InlineData(DialogRequestType.Alert, "Selection result requested")]
    [InlineData(DialogRequestType.Alert, "String result requested")]
    [InlineData(DialogRequestType.Alert, "Boolean result requested")]
    [InlineData(DialogRequestType.Alert, "Int result requested")]
    public void DialogRequest_ThrowsWhenCompletingWrongResultType(DialogRequestType type, string expectedMessage)
    {
        var request = new DialogRequest { Type = type };

        var exception = expectedMessage switch
        {
            "Selection result requested" => Assert.Throws<InvalidOperationException>(() => request.SetSelection(new object())),
            "String result requested" => Assert.Throws<InvalidOperationException>(() => request.SetResult("wrong")),
            "Boolean result requested" => Assert.Throws<InvalidOperationException>(() => request.SetResult(true)),
            _ => Assert.Throws<InvalidOperationException>(() => request.SetResult(1))
        };

        Assert.Contains(expectedMessage, exception.Message);
    }

    [Fact]
    public void DialogRequest_ThrowsForUndefinedDialogType()
    {
        var request = new DialogRequest();

        Assert.Throws<ArgumentOutOfRangeException>(() => request.Type = (DialogRequestType)999);
    }
}
