namespace RoboFactory.Api.Services;
public class Quote
{
    public Tuple<RoboHeadOption, double> Head { get; }
    public Tuple<RoboBodyOption, double> Body { get; set; }

    public class PricedPart<T> where T : RoboPart
    {
        public T Part { get; set; }
        public double Price { get; set; }
    }

    public Quote(PricedPart<RoboHead> pricedHead, PricedPart<RoboBody> pricedBody)
    {
        Head = new Tuple<RoboHeadOption, double>(pricedHead.Part.Option, pricedHead.Price);
        Body = new Tuple<RoboBodyOption, double>(pricedBody.Part.Option, pricedBody.Price);
    }

    public override bool Equals(object? obj)
    {
        return obj is Quote quote 
            && EqualityComparer<Tuple<RoboHeadOption, double>>.Default.Equals(Head, quote.Head)
            && EqualityComparer<Tuple<RoboBodyOption, double>>.Default.Equals(Body, quote.Body);
    }
}