using System;
using System.Security.Cryptography.X509Certificates;
using ikoLite.Messaging;
using ikoLite.Messaging.Models;
using ikoLite.Models;
using ikoLite.Models.Interfaces;
using ikoLite.Services.Saga.Cancellation;
using ikoLite.Services.Saga.Exp;
using MassTransit;
using MassTransit.Middleware;

namespace ikoLite.Services.Saga
{
	public class RestaurantBookingSaga : MassTransitStateMachine<RestaurantBooking>
	{
        public State AwaitingBookingApproved { get; private set; }

        public Event<IBookingRequest> BookingRequest { get; private set; }

        public Event<ITableBooked> TableBooked { get; private set; }

        public Event<IKitchenReady> KitchenReady { get; private set; }

        public Event<Fault<IBookingRequest>> BookingRequestFault { get; private set; }

        public Schedule<RestaurantBooking, IBookingExp> BookingExp { get; private set; }

        public Event BookingApproved { get; private set; }



        public RestaurantBookingSaga()
		{
			InstanceState(x => x.CurrentState);

			Event(() => BookingRequest, x =>
			x.CorrelateById(y => y.Message.OrderId)
			.SelectId(y => y.Message.OrderId));

			Event(() => TableBooked, x =>
			x.CorrelateById(y => y.Message.OrderId));

            Event(() => KitchenReady, x =>
            x.CorrelateById(y => y.Message.OrderId));

			CompositeEvent(() => BookingApproved, x =>
			x.ReadyEventStatus, KitchenReady, TableBooked);

			Event(() => BookingRequestFault, x =>
			x.CorrelateById(y => y.Message.Message.OrderId));

			Schedule(() => BookingExp, x =>
			x.ExpId, x =>
			{
				x.Delay = TimeSpan.FromSeconds(3);
				x.Received = z => z.CorrelateById(y => y.Message.OrderId);
			});

			Initially(
				When(BookingRequest).Then(x =>
				{
					x.Instance.CorrelationId = x.Data.OrderId;
					x.Instance.OrderId = x.Data.OrderId;
					x.Instance.ClientId = x.Data.ClientId;
					Console.WriteLine("Saga message:" + x.Data.CreationDate);
				})
				.Schedule(BookingExp,
					x => new BookingExp(x.Instance),
					x => TimeSpan.FromSeconds(1))
				.TransitionTo(AwaitingBookingApproved)
			);

			During(AwaitingBookingApproved,
				When(BookingApproved)
				.Unschedule(BookingExp)
				.Publish(x => (INotify) new Notify(x.Instance.OrderId, x.Instance.ClientId, "The table was booked successfully"))
				.Finalize(),
				
				When(BookingRequestFault)
				.Then(x => Console.WriteLine("Some exception was happened"))
				.Publish(x => (INotify)new Notify(x.Instance.OrderId, x.Instance.ClientId, "The table wasn't booked"))
				.Publish(x => (IBookingCancellation)new BookingCancellation(x.Data.Message.OrderId, x.Data.Message.ClientId, DateTime.Now))
				.Finalize(),

				When(BookingExp.Received)
				.Then( x => Console.WriteLine($"Order cancelled: {x.Instance.OrderId}"))
				.Finalize()
				);

			SetCompletedWhenFinalized();
        }
    }
}

