namespace RoboFactory.Api.Services;
public class Quote
{
    public Tuple<RoboHeadOption, double> Head { get; }

    public Quote(RoboHeadOption robohead, double price)
    {
        Head = new Tuple<RoboHeadOption, double>(robohead, price);
    }

    public override bool Equals(object? obj)
    {
        return obj is Quote quote &&
               EqualityComparer<Tuple<RoboHeadOption, double>>.Default.Equals(Head, quote.Head);
    }
}