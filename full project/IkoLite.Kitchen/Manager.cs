using System;
using ikoLite.Messaging;
using ikoLite.Messaging.Models;
using MassTransit;

namespace ikoLite.Kitchen
{
	public class Manager
	{
		private readonly IBus _bus;


		public Manager(IBus bus)
		{
			_bus = bus;
		}


		public void CheckKitchenready(Guid guid, Dish? dish)
        {
			Random rnd = new();
			bool res = false;
			if (rnd.Next(0, 2) == 1)
				res = true;

			_bus.Publish<IKitchenReady>(new KitchenReady(guid, res));
		}
	}
}

