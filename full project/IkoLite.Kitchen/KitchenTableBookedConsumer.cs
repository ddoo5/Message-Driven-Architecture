using System;
using ikoLite.Messaging;
using ikoLite.Messaging.Models;
using MassTransit;

namespace ikoLite.Kitchen
{
	public class KitchenTableBookedConsumer : IConsumer<ITableBooked>
	{
		private readonly Manager _manager;



		public KitchenTableBookedConsumer(Manager manager)
		{
			_manager = manager;
		}



		public Task Consume(ConsumeContext<ITableBooked> context)
		{
			var result = context.Message.Success;

			if (result)
				_manager.CheckKitchenready(context.Message.OrderId, context.Message.PreOrder);

			return context.ConsumeCompleted;
		}
	}
}

