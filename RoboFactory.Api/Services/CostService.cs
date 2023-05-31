namespace RoboFactory.Api.Services;
public class CostService
{
    public Quote? CalculateCost(RoboHead head)
    {
        return new Quote( RoboHead.InfraredVision, 1);
    }

}