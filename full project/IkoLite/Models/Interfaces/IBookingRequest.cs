using ikoLite.Messaging;
using ikoLite.Messaging.Models;

namespace ikoLite.Models.Interfaces
{
    public interface IBookingRequest
    {
        public Guid OrderId { get; }

        public Guid ClientId { get; }

        public Dish? PreOrder { get; }

        public DateTime CreationDate { get; set; }
    }
}