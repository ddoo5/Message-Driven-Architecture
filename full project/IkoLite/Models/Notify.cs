using ikoLite.Models.Interfaces;

namespace ikoLite.Models
{
    public class Notify : INotify
    {
        public Guid OrderId { get; set; }
        public Guid ClientId { get; set; }
        public string Message { get; set; }


        public Notify(Guid orderId, Guid clientId, string message)
        {
            OrderId = orderId;
            ClientId = clientId;
            Message = message;
        }
    }
}