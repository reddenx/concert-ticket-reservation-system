using Castle.Core.Logging;
using ConcertoReservoApi.Core;
using ConcertoReservoApi.Infrastructure;
using ConcertoReservoApi.Infrastructure.DataRepositories;
using ConcertoReservoApi.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConcertoReservoApi.Services.IShoppingService;

namespace ConcertoReservoTests.Services;

[TestClass]
public class ShoppingServiceTests
{
    [TestMethod]
    public void AttemptPurchase_Failed_CannotAttemptPurchaseIfSessionHasJustStarted()
    {
        //normally I'd have helper methods around setting up mocked DI and some sugar around the mocking, or if the tests are getting a little out of control do something like a subclass sandbox (base class is toolkit, each subclass is an individual test), this is more suited for functional/workflow tests that require large testing methods and elaborate setups.

        var timeService = new Mock<ITimeService>();
        var shoppingRepository = new Mock<IShoppingRepository>();
        var logger = new Mock<ILogger<ShoppingService>>();
        var eventsRepository = new Mock<IEventsRepository>();
        var seatingRepository = new Mock<ISeatingRepository>();
        var paymentService = new Mock<IPaymentService>();

        var shoppingService = new ShoppingService(
            shoppingRepository.Object,
            logger.Object,
            eventsRepository.Object,
            seatingRepository.Object,
            paymentService.Object,
            timeService.Object);

        var venue = new VenueData("TEST_VENUE_ID", "a test venue", "where all the tests go!");
        var seat1 = new VenueSeatingData("TEST_VENUE_ID", "SECTION_ID", "SEAT_ID", "1A", "that first seat", Point.Empty);
        var section1 = new VenueSectionData("TEST_VENUE_ID", "SECTION_ID", "Section 1", "for that first section feeling", [Point.Empty], Point.Empty, [seat1]);

        var sectionConfig = new EventSectionConfigurationData("TEST_SECTION", 25.10m);
        var eventInfo = new EventData("TEST_EVENT_ID", "neato bonito a test event", "[descriptoooo]", EventDataPublishStates.Published, DateTimeOffset.UtcNow.AddDays(30), DateTimeOffset.UtcNow.AddDays(-10), null, null, "TEST_VENUE_ID", [sectionConfig]);

        var sessionId = "SHOPPING_SESSION_ID";

        eventsRepository.Setup(e => e.GetEvent(eventInfo.Id))
            .Returns(eventInfo);
        timeService.Setup(t => t.GetCurrentTime())
            .Returns(DateTimeOffset.UtcNow);
        shoppingRepository.Setup(s => s.CreateShoppingSession(eventInfo.Id))
            .Returns(new ShoppingSession(sessionId, eventInfo.Id));

        shoppingRepository.Setup(s => s.Get(sessionId))
            .Returns(new ShoppingSession(sessionId, eventInfo.Id));

        //normally I'd split this out more but it's already late and just wanted to get an example out there

        var session = shoppingService.StartShopping(eventInfo.Id);
        Assert.IsFalse(session.Error.HasValue);
        Assert.IsTrue(session.Data.CurrentValidationIssues.Any());
        Assert.IsNull(session.Data.Expiration);

        var failedPurchase1 = shoppingService.AttemptPurchase(session.Data.ShoppingSessionId, -1);
        Assert.AreEqual(ShoppingErrors.CannotCheckoutWithValidationIssues, failedPurchase1.Error.Value);

        timeService.VerifyAll();
        shoppingRepository.VerifyAll();
        logger.VerifyAll();
        eventsRepository.VerifyAll();
        seatingRepository.VerifyAll();
        paymentService.VerifyAll();
    }
}
