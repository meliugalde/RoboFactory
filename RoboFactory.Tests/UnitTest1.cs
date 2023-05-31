using RoboFactory.Api.Services;

namespace RoboFactory.Tests;

public class UnitTest1
{
    

    [Fact]
    public void CalculateCost()
    {
        CostService costService = new CostService();
        Quote expectedQuote = new Quote( RoboHead.InfraredVision, 1);
       
        var quote = costService.CalculateCost(RoboHead.InfraredVision);

        Assert.Equal(expectedQuote, quote);
    }
}