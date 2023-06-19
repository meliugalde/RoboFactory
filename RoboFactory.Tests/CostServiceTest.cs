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
        RoboHead infraredVisionHead = new RoboHead{ Option = RoboHeadOption.InfraredVision };
        RoboHead standardVisionHead = new RoboHead{ Option = RoboHeadOption.StandardVision };
        var roboBody = new RoboBody { Option = RoboBodyOption.Triangular };
        
        GivenPriceForPart(mockLuxurySupplier, 15);
        GivenPriceForPart(mockSupplier, 65);
        
        var expectedInfraredQuote = new Quote(
            new Quote.PricedPart<RoboHead>
            {
                Part = infraredVisionHead,
                Price = 15
            },
            new Quote.PricedPart<RoboBody>
            {
                Part = roboBody,
                Price = 15
            }
        );
       
        var infraredQuote = costService.CalculateCost(infraredVisionHead, roboBody);
        var standardQuote = costService.CalculateCost(standardVisionHead, roboBody);

        Assert.Equal(expectedInfraredQuote, infraredQuote);
        Assert.NotEqual(standardQuote, infraredQuote);
    }

    [Fact]
    public void ThrowsAnErrorWhenNoSupplierProvidesASpecificPart()
    {
        RoboHead infraredVisionHead = new RoboHead{ Option = RoboHeadOption.InfraredVision };
        var roboBody = new RoboBody { Option = RoboBodyOption.Round };
        Assert.Throws<Exception>(() => costService.CalculateCost(infraredVisionHead, roboBody));
    }

    [Fact]
    public void CalculateCostIsQueryingSupplier()
    {
        GivenPriceForPart(mockLuxurySupplier, 50);
        GivenPriceForPart(mockSupplier, 35);

        RoboHead infraredVisionHead = new RoboHead{ Option = RoboHeadOption.NightVision };
        var roboBody = new RoboBody { Option = RoboBodyOption.Rectangular };
        costService.CalculateCost(infraredVisionHead, roboBody);
        
        ThenSupplierHasBeenQueriedForPrice(mockSupplier);
    }

    [Fact]
    public void GetTheCheapestPartFromSuppliers()
    {
        GivenPriceForPart(mockLuxurySupplier, 40);
        GivenPriceForPart(mockSupplier, 20);

        RoboHead infraredVisionHead = new RoboHead{ Option = RoboHeadOption.InfraredVision };
        var roboBody = new RoboBody { Option = RoboBodyOption.Rectangular };
        var currentQuote = costService.CalculateCost(infraredVisionHead, roboBody);
        
        AssertQuotePriceEquals(currentQuote, 20);
    }

    [Fact]
    public void SkipTheSupplierIfDoesNotProvideSpecificPart()
    {
        GivenPriceForPart(mockLuxurySupplier, 15);
        GivenPriceForPartNotAvailable(mockSupplier);

        RoboHead infraredVisionHead = new RoboHead{ Option = RoboHeadOption.InfraredVision };
        var roboBody = new RoboBody { Option = RoboBodyOption.Rectangular };
        var currentQuote = costService.CalculateCost(infraredVisionHead, roboBody);

        ThenSupplierHasBeenQueriedForPrice(mockLuxurySupplier);
        ThenSupplierHasNotBeenQueriedForPrice(mockSupplier);
        AssertQuotePriceEquals(currentQuote, 15);
    }

    private static void AssertQuotePriceEquals(Quote currentQuote, int expected)
    {
        var (_, currentHeadPrice) = currentQuote.Head;
        var (_, currentBodyPrice) = currentQuote.Body;
        Assert.Equal(expected, currentHeadPrice);
        Assert.Equal(expected, currentBodyPrice);
    }

    private void GivenPriceForPart(Mock<Supplier> mockedSupplier, int price)
    {
        mockedSupplier.Setup(supplier => supplier.HasPart(It.IsAny<RoboPart>())).Returns(true);
        mockedSupplier.Setup(supplier => supplier.GetPrice(It.IsAny<RoboPart>())).Returns(price);
    }

    private void GivenPriceForPartNotAvailable(Mock<Supplier> mockedSupplier)
    {
        mockedSupplier.Setup(supplier => supplier.HasPart(It.IsAny<RoboPart>())).Returns(false);
    }

    private void ThenSupplierHasNotBeenQueriedForPrice(Mock<Supplier> mockedSupplier)
    {
        mockedSupplier.Verify(supplier => supplier.GetPrice(It.IsAny<RoboPart>()), Times.Never);
    }

    private void ThenSupplierHasBeenQueriedForPrice(Mock<Supplier> mockedSupplier)
    {
        mockedSupplier.Verify(supplier => supplier.GetPrice(It.IsAny<RoboPart>()), Times.AtLeast(1));
    }
}