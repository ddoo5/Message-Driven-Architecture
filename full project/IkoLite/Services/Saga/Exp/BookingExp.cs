using System;
using ikoLite.Models;
using ikoLite.Models.Interfaces;

namespace ikoLite.Services.Saga.Exp
{
	public class BookingExp :IBookingExp
	{
        private readonly RestaurantBooking _instance;


        public BookingExp(RestaurantBooking restaurant)
        {
            _instance = restaurant;
        }


        public Guid OrderId => _instance.OrderId;
    }
}

