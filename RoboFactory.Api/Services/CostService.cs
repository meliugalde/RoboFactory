using static System.Double;

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
        double cheapest = GetCheapestPrice(head.Option);

        return new Quote(head.Option, cheapest);
    }

    private double GetCheapestPrice(RoboHeadOption head)
    {
        double cheapest = PositiveInfinity;
        foreach (var supplier in _suppliers)
        {
            if (supplier.HasPart(head))
            {
                cheapest = Math.Min(cheapest, supplier.GetPrice(head));
            }
        }

        if (Double.IsPositiveInfinity(cheapest))
        {
            throw new Exception("No supplier for the Head robot part");
        }

        return cheapest;
    }
}