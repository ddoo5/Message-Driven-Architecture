using ikoLite.Models;

namespace ikoLite.Services.Messages.UI
{
	public class MessagesUI
	{
		public static void GreetingBooking(int checkMethod)
		{
			if(checkMethod == 1)  //sync
                Console.WriteLine("Please wait, I'll search a table for you and approve your booking");
            if (checkMethod == 2)//async
            {
                //Thread.Sleep(4000);
                Console.WriteLine("Welcome! Please wait, I'll search a table for you and send you message");
            }
            if (checkMethod == 3) //sync
                Console.WriteLine("Please wait, I'll search your table and remove your booking");
            if (checkMethod == 4)//async
            {
                Thread.Sleep(4000);
                Console.WriteLine("Welcome! Please wait, I'll search your book and send you message");
            }
        }


		public static void AddBookSync(Table? table)
		{
            Console.WriteLine(table is null ? "Sorry, all tables are busy now" :
				$"Your table number {table._id}");
        }


		public static void AddBookAsync(Table? table)
		{
            Thread.Sleep(4000);

            Console.WriteLine(table is null
				? "Notification: Sorry, all tables are busy now"
				: $"Notification: Your table number {table._id}");
        }


        public static void RemoveBookSync(Table? table)
        {
            Console.WriteLine(table is null ? "Sorry, all tables are free now" :
    $"Your booked table with number {table._id} was removed");
        }


        public static void RempveBookAsync(Table? table)
        {
            Thread.Sleep(4000);

            Console.WriteLine(table is null
                        ? "Notification: Sorry, all tables free now"
                        : $"Notification: Your booked table with number {table._id} was removed");
        }


        public static void sytemInfoAutoDelete(IEnumerable<Table> table)
		{
            Console.WriteLine($"{table.Select(x => x._state == Models.Rules.State.Booked).Count()} books removed");
        }
    }
}

