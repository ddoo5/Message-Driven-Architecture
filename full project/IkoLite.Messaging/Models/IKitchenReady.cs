using System;
namespace ikoLite.Messaging.Models
{
	public interface IKitchenReady
	{
		public Guid OrderId { get; }

		public bool Ready { get; }
	}
}

