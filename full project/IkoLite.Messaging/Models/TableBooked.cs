using System;
namespace ikoLite.Messaging.Models
{
	public class TableBooked : ITableBooked
	{
		public TableBooked(Guid orderId, Guid clientId, bool success, Dish? preOrder = null)
		{
			OrderId = orderId;
			ClientId = clientId;
			PreOrder = preOrder;
			Success = success;
		}



		public Guid OrderId { get; }

		public Guid ClientId { get; }

		public Dish? PreOrder { get; }

		public bool Success { get; }
	}
}

