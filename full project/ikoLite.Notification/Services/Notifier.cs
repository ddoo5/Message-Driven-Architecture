using System;
using System.Collections.Concurrent;

namespace ikoLite.Notification.Services
{
	public class Notifier
	{
        private readonly ConcurrentDictionary<Guid, Tuple<Guid?, Accepted>> _state = new();



        public void Accept(Guid orderId, Accepted accepted, Guid? clientId = null)
        {
            _state.AddOrUpdate(orderId, new Tuple<Guid?, Accepted>(clientId, accepted),
                (Guid, oldValue) => new Tuple<Guid?, Accepted>(oldValue.Item1 ?? clientId, oldValue.Item2 | accepted));

            Notify(orderId, clientId, "");
        }


        public void Notify(Guid orderId, Guid? ClientId, string Message)
        {
            var booking = _state[orderId];

            switch (booking.Item2)
            {

                case Accepted.All:
                    Console.WriteLine($"[Order ID: {orderId}][Client ID: {ClientId}] [Message: {Message}]");
                    _state.Remove(orderId, out _);
                    break;

                case Accepted.Kitchen:
                    Console.WriteLine($"[Order ID: {orderId}] Kitchen is ready");
                    break;

                case Accepted.Booking:
                    Console.WriteLine($"[Order ID: {orderId}] We have free tables");
                    break;

                case Accepted.Rejected:
                    Console.WriteLine($"[Order ID: {orderId}] Kitchen is not ready or all tables are busy");
                    _state.Remove(orderId, out _);
                    break;

                case Accepted.RejectedBooking:
                    Console.WriteLine($"[Order ID: {orderId}] We can't approve order because all tables are busy");
                    break;

                case Accepted.RejectedKitchen:
                    Console.WriteLine($"[Order ID: {orderId}] We can't approve order because kitchen is busy");
                    break;

                default:
                    Console.WriteLine("[Error: Something wrong]");
                    break;
            }
        }
    }
}

