using System.Diagnostics;
using ikoLite.Messaging;
using ikoLite.Messaging.Services;
using ikoLite.Models;
using ikoLite.Models.Rules;
using ikoLite.Services.Messages.UI;

namespace ikoLite.Services.Booking
{
	public class BookingService
	{
		private readonly List<Table> _tables = new();
        private readonly Producer _producer = new("BookingNotification", "mustang.rmq.cloudamqp.com");



		public BookingService()
		{
			for (ushort i = 1; i < 30; i++)
			{
				_tables.Add(new Table(i));
			}
		}



		public void BookFreeTable(int countPersons)
		{
            MessagesUI.GreetingBooking(1);

			var table = _tables.FirstOrDefault(x => x._seatsCount > countPersons && x._state == State.Free);
            table?.SetState(State.Booked);

            Thread.Sleep(1000 * 5);

            MessagesUI.AddBookSync(table);
		}


		public async Task<bool?> BookFreeTableAsync(int countPersons)
		{
            MessagesUI.GreetingBooking(2);

            var table = _tables.FirstOrDefault(x => x._seatsCount > countPersons && x._state == State.Free);

            return table?.SetState(State.Booked);
        }


        public void RemoveBookedTable(int id)
        {
            MessagesUI.GreetingBooking(3);

            var table = _tables.FirstOrDefault(x => x._id > id && x._state == State.Booked);
			table?.SetState(State.Free);

            Thread.Sleep(1000 * 5);

            MessagesUI.RemoveBookSync(table);
        }


        public void RemoveBookedTableAsync(int id)
        {
            MessagesUI.GreetingBooking(4);
            object forLock = new();

                Task.Run(async () =>
                {
                    await Task.Delay(1000 * 5);
                    lock (forLock)
                    {
                        var table = _tables.FirstOrDefault(x => x._id > id && x._state == State.Booked);
                        table?.SetState(State.Free);

                        MessagesUI.RempveBookAsync(table);
                    }
                });
        }


		public void AutoCleanBooks()
		{
            Stopwatch stopwatch = new();
            stopwatch.Start();

            while (true)
            {
                TimeSpan timeSpan = stopwatch.Elapsed;

                if (timeSpan.Seconds == 200)
                {
                    var table = _tables.Where(x => x._state == State.Booked);
                    MessagesUI.sytemInfoAutoDelete(table);
                    foreach (var item in table)
                    {
                        item?.SetState(State.Free);
                    }

                    stopwatch.Restart();
                }
            }
        }
    }
}

