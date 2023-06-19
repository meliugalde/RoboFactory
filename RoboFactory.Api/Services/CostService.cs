using static System.Double;

namespace RoboFactory.Api.Services;
public class CostService
{
    private readonly List<Supplier> _suppliers;

    public CostService(List<Supplier> suppliers)
    {
        _suppliers = suppliers;
    }

    public Quote CalculateCost(RoboHead head, RoboBody body)
    {
        double cheapestHead = GetCheapestPrice(head);
        double cheapestBody = GetCheapestPrice(body);

        return new Quote(
            new Quote.PricedPart<RoboHead>
            {
                Part = head,
                Price = cheapestHead
            },
            new Quote.PricedPart<RoboBody>
            {
                Part = body,
                Price = cheapestBody
            }
        );
    }

    private double GetCheapestPrice(RoboPart part)
    {
        double cheapest = PositiveInfinity;
        foreach (var supplier in _suppliers)
        {
            if (supplier.HasPart(part))
            {
                cheapest = Math.Min(cheapest, supplier.GetPrice(part));
            }
        }

        if (Double.IsPositiveInfinity(cheapest))
        {
            throw new Exception("No supplier for the Head robot part");
        }

        return cheapest;
    }
}