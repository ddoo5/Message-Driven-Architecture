using System;
using ikoLite.Messaging;
using ikoLite.Messaging.Memory;
using ikoLite.Messaging.Models;
using ikoLite.Models;
using ikoLite.Models.Interfaces;
using ikoLite.Services.Booking;
using MassTransit;

namespace ikoLite.Services.Consumers
{
	public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequest>
	{
		private readonly BookingService _restaurant;
		private readonly IMemoryRepository<BookingRequestModel> _repository;



		public RestaurantBookingRequestConsumer(BookingService restaurant, IMemoryRepository<BookingRequestModel> repository)
		{
			_restaurant = restaurant;
			_repository = repository;
		}



		public async Task Consume(ConsumeContext<IBookingRequest> context)
		{
			var model = _repository.Get().FirstOrDefault(x => x.OrderId == context.Message.OrderId);

            if (model != null)
            {
                if (model.CheckMessageId(context.Message.OrderId.ToString()))
                {
                    Console.WriteLine($"[Message Id: {context.MessageId.ToString()}] Second time");
                    return;
                }
            }

            var requestModel = new BookingRequestModel(context.Message.OrderId, context.Message.ClientId, context.Message.PreOrder, context.Message.CreationDate, context.MessageId.ToString());

            Console.WriteLine($"[Message Id: {context?.MessageId.ToString()}] First time");

			var resultModel = model?.Update(requestModel, context.MessageId.ToString()) ?? requestModel;

			_repository.Add(resultModel);

            var result = await _restaurant.BookFreeTableAsync(1);

            if (result == null)
            {
                throw new ArgumentOutOfRangeException("[Notification: We dont have free tables for order]");
            }

            await context?.Publish<ITableBooked>(new TableBooked(context.Message.OrderId, context.Message.ClientId, result ?? false));

            Console.WriteLine($"[OrderId: {context?.Message.OrderId}] approved");
        }
	}
}

