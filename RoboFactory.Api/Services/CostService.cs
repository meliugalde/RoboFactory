namespace RoboFactory.Api.Services;
public class CostService
{
    private readonly List<Supplier> _suppliers;

    public CostService(List<Supplier> suppliers)
    {
        _suppliers = suppliers;
    }

    public Quote CalculateCost(RoboHead head)
    {
        double cheapest = Double.PositiveInfinity;
        foreach (var supplier in _suppliers)
        {
            cheapest = Math.Min(cheapest, supplier.GetPrice(head));
        }

        return new Quote( RoboHead.InfraredVision, cheapest);
    }

}