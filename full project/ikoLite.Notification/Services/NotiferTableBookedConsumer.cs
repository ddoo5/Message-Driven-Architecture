using System;
using ikoLite.Messaging;
using ikoLite.Messaging.Models;
using MassTransit;

namespace ikoLite.Notification.Services
{
	public class NotiferTableBookedConsumer : IConsumer<ITableBooked>
	{
		private readonly Notifier _notifier;



		public NotiferTableBookedConsumer(Notifier notifier)
		{
			_notifier = notifier;
		}



		public Task Consume(ConsumeContext<ITableBooked> context)
		{
			var result = context.Message.Success;

			_notifier.Accept(context.Message.OrderId, result ? Accepted.Booking : Accepted.Rejected, context.Message.ClientId);

			return Task.CompletedTask;
		}
	}
}

