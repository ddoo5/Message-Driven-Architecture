using System;
using ikoLite.Messaging;
using ikoLite.Messaging.Models;
using ikoLite.Services.Booking;
using ikoLite.Services.Saga.Requests;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace ikoLite.Services
{
	public class Worker : BackgroundService
	{
		private readonly IBus _bus;



        public Worker(IBus bus)
		{
			_bus = bus;
		}



		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
            while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(15000, stoppingToken);
				_bus.Publish(new BookingRequest(NewId.NextGuid(), NewId.NextGuid(), Dish.nothing, DateTime.Now), stoppingToken);
			}
		}
	}
}

