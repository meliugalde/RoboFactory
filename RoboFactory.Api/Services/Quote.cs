namespace RoboFactory.Api.Services;
public class Quote
{
    public Tuple<RoboHead, double> Head { get; }

    public Quote(RoboHead robohead, double price)
    {
        Head = new Tuple<RoboHead, double>(robohead, price);
    }

    public override bool Equals(object? obj)
    {
        return obj is Quote quote &&
               EqualityComparer<Tuple<RoboHead, double>>.Default.Equals(Head, quote.Head);
    }
}