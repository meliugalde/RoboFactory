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

        GivenPriceForPart(mockLuxurySupplier, 15);
        GivenPriceForPart(mockSupplier, 65);
        
        var expectedInfraredQuote = new Quote(infraredVisionHead.Option, 15);
       
        var infraredQuote = costService.CalculateCost(infraredVisionHead);
        var standardQuote = costService.CalculateCost(standardVisionHead);

        Assert.Equal(expectedInfraredQuote, infraredQuote);
        Assert.NotEqual(standardQuote, infraredQuote);
    }

    [Fact]
    public void ThrowsAnErrorWhenNoSupplierProvidesASpecificPart()
    {
        RoboHead infraredVisionHead = new RoboHead{ Option = RoboHeadOption.InfraredVision };
        Assert.Throws<Exception>(() => costService.CalculateCost(infraredVisionHead));
    }

    [Fact]
    public void CalculateCostIsQueryingSupplier()
    {
        GivenPriceForPart(mockLuxurySupplier, 50);
        GivenPriceForPart(mockSupplier, 35);

        RoboHead infraredVisionHead = new RoboHead{ Option = RoboHeadOption.NightVision };
        costService.CalculateCost(infraredVisionHead);
        
        ThenSupplierHasBeenQueriedForPrice(mockSupplier);
    }

    [Fact]
    public void GetTheCheapestPartFromSuppliers()
    {
        GivenPriceForPart(mockLuxurySupplier, 40);
        GivenPriceForPart(mockSupplier, 20);

        RoboHead infraredVisionHead = new RoboHead{ Option = RoboHeadOption.InfraredVision };
        var currentQuote = costService.CalculateCost(infraredVisionHead);
        
        AssertQuotePriceEquals(currentQuote, 20);
    }

    [Fact]
    public void SkipTheSupplierIfDoesNotProvideSpecificPart()
    {
        GivenPriceForPart(mockLuxurySupplier, 15);
        GivenPriceForPartNotAvailable(mockSupplier);

        RoboHead infraredVisionHead = new RoboHead{ Option = RoboHeadOption.InfraredVision };
        var currentQuote = costService.CalculateCost(infraredVisionHead);

        ThenSupplierHasBeenQueriedForPrice(mockLuxurySupplier);
        ThenSupplierHasNotBeenQueriedForPrice(mockSupplier);
        AssertQuotePriceEquals(currentQuote, 15);
    }


    // [Fact]
    // public void CalculateTotalCostOfGivenParts()
    // {
    //     GivenPriceForPart(mockLuxurySupplier, 50);
    //     GivenPriceForPart(mockSupplier, 35);

    //     costService.CalculateCost(RoboHead.InfraredVision, RoboBody.Square);

    //     mockSupplier.Verify(supplier => supplier.GetPrice(It.IsAny<RoboHead>()));
    // }

    private static void AssertQuotePriceEquals(Quote currentQuote, int expected)
    {
        var (_, currentPrice) = currentQuote.Head;
        Assert.Equal(expected, currentPrice);
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
        mockedSupplier.Verify(supplier => supplier.GetPrice(It.IsAny<RoboPart>()), Times.Once);
    }
}