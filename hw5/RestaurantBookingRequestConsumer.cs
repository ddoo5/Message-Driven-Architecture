using System;
using ikoLite.Messaging;
using ikoLite.Services.Booking;
using MassTransit;

namespace ikoLite
{
	public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequest>
	{
		private readonly BookingService _restaurant;



		public RestaurantBookingRequestConsumer(BookingService restaurant)
		{
			_restaurant = restaurant;
		}



		public async Task Consume(ConsumeContext<IBookingRequest> context)
		{
			var result = await _restaurant.BookFreeTableAsync(1);

            if (result == null)
            {
                throw new ArgumentOutOfRangeException("[Notification: We dont have free tables for order]");
            }

            await context.Publish<ITableBooked>(new TableBooked(context.Message.OrderId, context.Message.ClientId, result ?? false));

            Console.WriteLine($"[OrderId: {context.Message.OrderId}] approved");
        }
	}
}

