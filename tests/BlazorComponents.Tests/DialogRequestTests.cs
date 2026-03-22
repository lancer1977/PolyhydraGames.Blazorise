using PolyhydraGames.BlazorComponents.Dialog;

namespace BlazorComponents.Tests.Dialog;

public class DialogRequestTests
{
    [Fact]
    public async Task SetResult_WithCancelledInt_SetsOkFalse()
    {
        var request = new DialogRequest { Type = DialogRequestType.PromptInt };

        request.SetResult( 5, cancelled: true );

        var result = await request.AsInt!.Task;
        Assert.False( result.Ok );
        Assert.Equal( 5, result.Result );
    }

    [Fact]
    public async Task SetResult_WithConfirmedInt_SetsOkTrue()
    {
        var request = new DialogRequest { Type = DialogRequestType.PromptInt };

        request.SetResult( 9, cancelled: false );

        var result = await request.AsInt!.Task;
        Assert.True( result.Ok );
        Assert.Equal( 9, result.Result );
    }

    [Fact]
    public async Task Cancel_ForSelectionDialog_ReturnsNotOk()
    {
        var request = new DialogRequest { Type = DialogRequestType.Selection };

        request.Cancel();

        var result = await request.AsSelection!.Task;
        Assert.False( result.Ok );
    }

    [Fact]
    public async Task Cancel_ForConfirmDialog_ReturnsNotOkFalseValue()
    {
        var request = new DialogRequest { Type = DialogRequestType.Confirm };

        request.Cancel();

        var result = await request.AsBool!.Task;
        Assert.False( result.Ok );
        Assert.False( result.Result );
    }
}
