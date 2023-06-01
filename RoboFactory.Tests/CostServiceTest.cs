using Moq;
using RoboFactory.Api.Services;

namespace RoboFactory.Tests;

public class CostServiceTest
{
    readonly Mock<Supplier> mockSupplier = new();
    readonly Mock<Supplier> mockLuxurySupplier = new();
    
    [Fact]
    public void CalculateCost()
    {
        var suppliers = new List<Supplier>(){ mockSupplier.Object };
        CostService costService = new CostService(suppliers);
        Quote expectedQuote = new Quote( RoboHead.InfraredVision, 1);
       
        var quote = costService.CalculateCost(RoboHead.InfraredVision);

        Assert.Equal(expectedQuote, quote);
    }


    [Fact]
    public void CalculateCostIsQueryingSupplier()
    {
        var suppliers = new List<Supplier>(){ mockSupplier.Object };
        CostService costService = new CostService(suppliers);

        costService.CalculateCost(RoboHead.InfraredVision);
        
        mockSupplier.Verify(supplier => supplier.GetPrice(It.IsAny<RoboHead>()));
    }

    [Fact]
    public void GetTheCheapestPartFromSuppliers()
    {

        mockLuxurySupplier.Setup(luxurySupplier => luxurySupplier.GetPrice(It.IsAny<RoboHead>())).Returns(40);
        mockSupplier.Setup(supplier => supplier.GetPrice(It.IsAny<RoboHead>())).Returns(20);

        var suppliers = new List<Supplier>(){ mockSupplier.Object, mockLuxurySupplier.Object };
        CostService costService = new CostService(suppliers);
        
        var currentQuote = costService.CalculateCost(RoboHead.InfraredVision);
        var (_, currentPrice) = currentQuote.Head;
        
        Assert.Equal(20, currentPrice);
    }

}