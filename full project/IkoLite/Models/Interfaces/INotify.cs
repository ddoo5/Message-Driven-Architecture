namespace ikoLite.Models.Interfaces
{
    internal interface INotify
    {
        public Guid OrderId { get; }

        public Guid ClientId { get; }

        public string Message { get; set; }
    }
}