namespace RoboFactory.Api.Services;

public interface Supplier
{
    double GetPrice(RoboHeadOption head);
    Boolean HasPart(RoboHeadOption head);
}