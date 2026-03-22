using PolyhydraGames.BlazorComponents.Dialog;

namespace BlazorComponents.Tests.Dialog;

public class ObservableDialogServiceTests
{
    [Fact]
    public async Task GetIntAsync_WithBounds_SetsRangeDefaultsInRequest()
    {
        var service = new ObservableDialogService();
        DialogRequest? captured = null;

        using var subscription = service.OnDialogRequest.Subscribe( request =>
        {
            captured = request;
            request.SetResult( request.DefaultInt ?? 0, cancelled: true );
        } );

        var result = await service.GetIntAsync( "Range test", 1, 10, 999 );

        Assert.NotNull( captured );
        Assert.Equal( DialogRequestType.PromptInt, captured!.Type );
        Assert.Equal( 1, captured.MinInt );
        Assert.Equal( 10, captured.MaxInt );
        Assert.Equal( 10, captured.DefaultInt );
        Assert.Contains( "between 1 and 10", captured.Message );
        Assert.False( result.Ok );
    }

    [Fact]
    public async Task NotificationAsync_UsesRequestedButtonText()
    {
        var service = new ObservableDialogService();
        DialogRequest? captured = null;

        using var subscription = service.OnDialogRequest.Subscribe( request =>
        {
            captured = request;
            request.SetResult();
        } );

        await service.NotificationAsync( "hello", "title", "Close" );

        Assert.NotNull( captured );
        Assert.Equal( DialogRequestType.Alert, captured!.Type );
        Assert.Equal( "Close", captured.PositiveButton );
    }
}
