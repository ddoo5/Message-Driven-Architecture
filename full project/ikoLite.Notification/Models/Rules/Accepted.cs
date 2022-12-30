using System;
namespace ikoLite.Notification
{
	[Flags]
	public enum Accepted
	{
		Rejected = 0,
		Kitchen = 1,
		Booking = 2,
		All = Kitchen | Booking,
        RejectedBooking = 4,
        RejectedKitchen = 5
    }
}

