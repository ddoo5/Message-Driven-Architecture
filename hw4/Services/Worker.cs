﻿using System;
using ikoLite.Messaging;
using ikoLite.Services.Booking;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace ikoLite
{
	public class Worker : BackgroundService
	{
		private readonly IBus _bus;
		private readonly BookingService _restaurant;



        public Worker(IBus bus, BookingService booking)
		{
			_bus = bus;
			_restaurant = booking;
		}



		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(3000, stoppingToken);
                await _bus.Publish(new BookingRequest(NewId.NextGuid(), NewId.NextGuid(), DateTime.UtcNow), stoppingToken);
            }
		}
	}
}

