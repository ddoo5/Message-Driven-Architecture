using System;
namespace ikoLite.Models.Interfaces
{
	public interface IBookingCancellation
	{
        public Guid OrderId { get; }

        public Guid ClientId { get; }

        public DateTime CancellationDate { get; set; }
    }
}

