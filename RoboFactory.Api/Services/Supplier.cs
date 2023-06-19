namespace RoboFactory.Api.Services;

public interface Supplier
{
    double GetPrice(RoboPart part);
    Boolean HasPart(RoboPart part);
}