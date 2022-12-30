using ikoLite.Models.Rules;

namespace ikoLite.Models
{
	public class Table
	{
		public State _state { get; set; }
		public int _seatsCount { get; }
		public int _id { get; }
		private readonly Random _rnd = new();



        public Table(int id)
		{
			_id = id;
			_state = State.Free;
			_seatsCount = _rnd.Next(2, 6);
		}



		public bool SetState(State state)
		{
			if (_state == state)
				return false;

			_state = state;

			return true;
		}
	}
}

