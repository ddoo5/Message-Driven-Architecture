using System;
namespace ikoLite.Messaging
{
	public interface IMemoryRepository<T> where T : class
	{
		public void Add(T entity);
		public void Update(T entity);
		public IEnumerable<T> Get();
	}
}

