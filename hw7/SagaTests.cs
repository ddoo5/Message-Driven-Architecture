using ikoLite.Messaging.Memory;
using ikoLite.Messaging.Models;
using ikoLite.Models;
using ikoLite.Models.Interfaces;
using ikoLite.Services;
using ikoLite.Services.Booking;
using ikoLite.Services.Consumers;
using ikoLite.Services.Saga;
using ikoLite.Services.Saga.Requests;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ikoLite.Tests
{
    public class SagaTests
    {
        private ServiceProvider _provider;
        private ITestHarness _harness;

        [OneTimeSetUp]
        public async Task Init()
        {
            _provider = new ServiceCollection().AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<RestaurantBookingRequestConsumer>();
                cfg.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>().Endpoint(e => e.Temporary = true)
                .InMemoryRepository();
                cfg.AddSagaStateMachineContainerTestHarness<RestaurantBookingSaga, RestaurantBooking>();
                cfg.AddDelayedMessageScheduler();
            })
            .AddLogging()
            .AddTransient<BookingService>()
            .AddSingleton<IMemoryRepository<BookingRequest>, MemoryRepository<BookingRequest>>()
            .AddTransient<RestaurantBooking>()
            .AddTransient<RestaurantBookingSaga>()
            .AddHostedService<Worker>()
            .BuildServiceProvider(true);

            _harness = _provider.GetTestHarness();

            await _harness.Start();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _harness.OutputTimeline(TestContext.Out, options => options.Now().IncludeAddress());
            await _provider.DisposeAsync();
        }


        [Test(Author = "DD", Description = "check saga")]
        [Retry(tryCount: 10)]
        public async Task Easy()
        {
            var orderId = NewId.NextGuid();
            var clientId = NewId.NextGuid();

            await _harness.Bus.Publish((IBookingRequest)new BookingRequest(orderId, clientId, Dish.nothing, DateTime.Now));
            Assert.That(await _harness.Published.Any<IBookingRequest>());
            Assert.That(await _harness.Consumed.Any<IBookingRequest>());

            var sagaHarness = _provider.GetRequiredService<IStateMachineSagaTestHarness<RestaurantBooking, RestaurantBookingSaga>>();

            Assert.That(await sagaHarness.Consumed.Any<IBookingRequest>());
            Assert.That(await sagaHarness.Created.Any(x => x.CorrelationId == orderId));

            var saga = sagaHarness.Created.Contains(orderId);

            Assert.That(saga, Is.Not.Null);
            Assert.That(saga.ClientId, Is.EqualTo(clientId));

            await _harness.OutputTimeline(TestContext.Out, configure: options => options.Now().IncludeAddress());
        }
    }
}

