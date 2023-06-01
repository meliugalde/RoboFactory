namespace RoboFactory.Api.Services;

public interface Supplier
{
    double GetPrice(RoboHead head);
    Boolean HasPart(RoboHead head);
}