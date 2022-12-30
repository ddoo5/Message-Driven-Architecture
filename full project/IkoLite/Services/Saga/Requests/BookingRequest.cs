using ikoLite.Messaging;
using ikoLite.Messaging.Models;
using ikoLite.Models.Interfaces;

namespace ikoLite.Services.Saga.Requests
{
    public class BookingRequest : IBookingRequest
    {
        public Guid OrderId { get; set; }
        public Guid ClientId { get; set; }
        public Dish? PreOrder { get; set; }
        public DateTime CreationDate { get; set; }


        public BookingRequest(Guid guid1, Guid guid2, Dish? preOrder, DateTime utcNow)
        {
            OrderId = guid1;
            ClientId = guid2;
            PreOrder = preOrder;
            CreationDate = utcNow;
        }
    }
}