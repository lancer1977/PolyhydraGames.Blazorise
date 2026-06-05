using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using PolyhydraGames.BlazorComponents.Components;

namespace BlazorComponents.Tests;

public class ComponentRenderCoverageTests : BunitContext
{
    public ComponentRenderCoverageTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Busy_RendersMessageWhenBusy()
    {
        var cut = Render<Busy>(parameters => parameters
            .Add(p => p.IsBusy, true)
            .Add(p => p.Message, "Working..."));

        Assert.Contains("Working...", cut.Markup);
        Assert.Contains("fa-spinner", cut.Markup);
    }

    [Fact]
    public void Busy_RendersChildContentWhenNotBusy()
    {
        var cut = Render<Busy>(parameters => parameters
            .Add(p => p.IsBusy, false)
            .Add<RenderFragment>(p => p.ChildContent, builder => builder.AddContent(0, "Ready content")));

        Assert.Contains("Ready content", cut.Markup);
        Assert.DoesNotContain("fa-spinner", cut.Markup);
    }

    [Fact]
    public void NavItem_RendersConfiguredTitleAndHref()
    {
        var cut = Render<NavItem>(parameters => parameters
            .Add(p => p.Title, "Dashboard")
            .Add(p => p.Uri, "/dashboard")
            .Add(p => p.Icon, "dashboard-icon"));

        Assert.Contains("Dashboard", cut.Markup);
        Assert.Contains("/dashboard", cut.Markup);
    }

    [Fact]
    public void Picker_RendersFallbackChildContentWhenItemsAreNull()
    {
        var cut = Render<Picker<string>>(parameters => parameters
            .Add<RenderFragment>(p => p.ChildContent, builder => builder.AddContent(0, "No picker yet")));

        Assert.Contains("No picker yet", cut.Markup);
    }

}
