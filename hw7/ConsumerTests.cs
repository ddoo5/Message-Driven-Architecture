using ikoLite.Services.Booking;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using ikoLite.Models;
using ikoLite.Messaging.Models;
using ikoLite.Models.Interfaces;
using ikoLite.Services.Saga.Requests;
using ikoLite.Services.Consumers;
using ikoLite.Messaging.Memory;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace ikoLite.Tests
{
	public class ConsumerTests
	{

		private ServiceProvider _provider;
		private ITestHarness _harness;



		[OneTimeSetUp]
		public async Task Init()
		{
			_provider = new ServiceCollection()
				.AddMassTransitTestHarness(confg =>
				{
					confg.AddConsumer<RestaurantBookingRequestConsumer>();
				})
				.AddLogging()
				.AddTransient<BookingService>()
				.AddSingleton<IMemoryRepository<BookingRequestModel>, MemoryRepository<BookingRequestModel>>()
				.BuildServiceProvider(true);

			_harness = _provider.GetTestHarness();

			await _harness.Start();
		}


		[OneTimeTearDown]
		public async Task TearDown()
		{
			await _harness.OutputTimeline(TestContext.Out, opt => opt.Now().IncludeAddress());
			await _provider.DisposeAsync();
		}


		[Test(Author = "DD", Description = "check on any booking request consume")]
        [Retry(tryCount: 10)]
        public async Task AnyBookingRequestConsumed()
		{
            await _harness.Bus.Publish(new BookingRequest(Guid.NewGuid(), Guid.NewGuid(),Dish.nothing, DateTime.Now));

            Assert.That(await _harness.Consumed.Any<IBookingRequest>());
            await _harness.OutputTimeline(TestContext.Out, options => options.Now().IncludeAddress());
        }


        [Test(Author = "DD", Description = "check on booking table")]
        [Retry(tryCount: 10)]
        public async Task BookingRequestConsumerTableBooked()
        {

            var consumer = _provider.GetRequiredService<IConsumerTestHarness<RestaurantBookingRequestConsumer>>();
            var orderId = NewId.NextGuid();

            var bus = _provider.GetRequiredService<IBus>();

            await bus.Publish<IBookingRequest>(new BookingRequest(orderId, orderId, Dish.nothing, DateTime.Now));

            Assert.That(consumer.Consumed.Select<IBookingRequest>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);

            Assert.That(_harness.Published.Select<ITableBooked>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);

            await _harness.OutputTimeline(TestContext.Out, options => options.Now().IncludeAddress());
        }
    }
}

