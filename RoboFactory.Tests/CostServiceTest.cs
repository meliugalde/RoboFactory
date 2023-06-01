using Moq;
using RoboFactory.Api.Services;

namespace RoboFactory.Tests;

public class CostServiceTest
{
    readonly Mock<Supplier> mockSupplier = new();
    readonly Mock<Supplier> mockLuxurySupplier = new();
    readonly CostService costService;

    public CostServiceTest()
    {
        var suppliers = new List<Supplier>(){ mockSupplier.Object, mockLuxurySupplier.Object };
        costService = new CostService(suppliers);
    }

    [Fact]
    public void CalculateCostForTheSpecificParts()
    {
        mockLuxurySupplier.Setup(luxurySupplier => luxurySupplier.HasPart(It.IsAny<RoboHead>())).Returns(true);
        GivenPriceForPart(mockLuxurySupplier, 15);
        mockSupplier.Setup(supplier => supplier.HasPart(It.IsAny<RoboHead>())).Returns(true);
        GivenPriceForPart(mockSupplier, 65);
        
        var expectedInfraredQuote = new Quote(RoboHead.InfraredVision, 15);
       
        var infraredQuote = costService.CalculateCost(RoboHead.InfraredVision);
        var standardQuote = costService.CalculateCost(RoboHead.StandardVision);

        Assert.Equal(expectedInfraredQuote, infraredQuote);
        Assert.NotEqual(standardQuote, infraredQuote);
    }

    private void GivenPriceForPart(Mock<Supplier> mockedSupplier, int price)
    {
        mockedSupplier.Setup(supplier => supplier.GetPrice(It.IsAny<RoboHead>())).Returns(price);
    }

    [Fact]
    public void ThrowsAnErrorWhenNoSupplierProvidesASpecificPart()
    {
        Assert.Throws<Exception>(() => costService.CalculateCost(RoboHead.InfraredVision));
    }

    [Fact]
    public void CalculateCostIsQueryingSupplier()
    {
        mockLuxurySupplier.Setup(luxurySupplier => luxurySupplier.HasPart(It.IsAny<RoboHead>())).Returns(true);
        mockLuxurySupplier.Setup(luxurySupplier => luxurySupplier.GetPrice(It.IsAny<RoboHead>())).Returns(50);
        mockSupplier.Setup(supplier => supplier.HasPart(It.IsAny<RoboHead>())).Returns(true);
        mockSupplier.Setup(supplier => supplier.GetPrice(It.IsAny<RoboHead>())).Returns(35);

        costService.CalculateCost(RoboHead.InfraredVision);
        
        mockSupplier.Verify(supplier => supplier.GetPrice(It.IsAny<RoboHead>()));
    }

    [Fact]
    public void GetTheCheapestPartFromSuppliers()
    {
        mockLuxurySupplier.Setup(luxurySupplier => luxurySupplier.HasPart(It.IsAny<RoboHead>())).Returns(true);
        mockLuxurySupplier.Setup(luxurySupplier => luxurySupplier.GetPrice(It.IsAny<RoboHead>())).Returns(40);
        mockSupplier.Setup(supplier => supplier.HasPart(It.IsAny<RoboHead>())).Returns(true);
        mockSupplier.Setup(supplier => supplier.GetPrice(It.IsAny<RoboHead>())).Returns(20);

        var currentQuote = costService.CalculateCost(RoboHead.InfraredVision);
        
        AssertQuotePriceEquals(currentQuote, 20);
    }

    [Fact]
    public void SkipTheSupplierIfDoesNotProvideSpecificPart()
    {
        mockLuxurySupplier.Setup(luxurySupplier => luxurySupplier.HasPart(It.IsAny<RoboHead>())).Returns(true);
        mockLuxurySupplier.Setup(luxurySupplier => luxurySupplier.GetPrice(It.IsAny<RoboHead>())).Returns(15);
        mockSupplier.Setup(supplier => supplier.HasPart(It.IsAny<RoboHead>())).Returns(false);

        var currentQuote = costService.CalculateCost(RoboHead.InfraredVision);

        mockLuxurySupplier.Verify(supplier => supplier.GetPrice(It.IsAny<RoboHead>()), Times.Once);
        mockSupplier.Verify(supplier => supplier.GetPrice(It.IsAny<RoboHead>()), Times.Never);
        AssertQuotePriceEquals(currentQuote, 15);
    }

    private static void AssertQuotePriceEquals(Quote currentQuote, int expected)
    {
        var (_, currentPrice) = currentQuote.Head;
        Assert.Equal(expected, currentPrice);
    }
}