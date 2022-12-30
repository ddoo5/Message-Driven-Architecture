using System;
namespace ikoLite.Notification
{
    public interface INotify
    {
        public Guid OrderId { get; }

        public Guid ClientId { get; }

        public string Message { get; set; }
    }
}

