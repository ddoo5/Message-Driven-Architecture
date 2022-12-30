using System;
using ikoLite.Messaging;
using ikoLite.Messaging.Models;

namespace ikoLite.Models
{
	public class BookingRequestModel
	{
		private List<string> _messageIds = new();
		public Guid OrderId { get; private set; }
        public Guid ClientId { get; private set; }
        public Dish? PreOrder { get; private set; }
        public DateTime CreationTime { get; private set; }



        public BookingRequestModel(Guid orderId, Guid clientId, Dish? preOrder, DateTime creationDate, string messageId)
		{
			_messageIds.Add(messageId);

			OrderId = orderId;
			ClientId = clientId;
			PreOrder = preOrder;
			CreationTime = creationDate;
		}



		public BookingRequestModel Update(BookingRequestModel model, string messageId)
		{
            _messageIds.Add(messageId);

            OrderId = model.OrderId;
            ClientId = model.ClientId;
            PreOrder = model.PreOrder;
            CreationTime = model.CreationTime;

			return this;
        }


		public bool CheckMessageId(string messageId)
		{
			return _messageIds.Contains(messageId);
		}
	}
}

