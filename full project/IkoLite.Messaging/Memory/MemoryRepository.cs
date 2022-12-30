using System;
using System.Collections.Concurrent;

namespace ikoLite.Messaging.Memory
{
	public class MemoryRepository<T> : IMemoryRepository<T> where T : class
	{
        private readonly ConcurrentBag<T> _repo = new();


        public void Add(T entity)
        {
            _repo.Add(entity);
        }


        public IEnumerable<T> Get()
        {
            return _repo;
        }


        public void Update(T entity)
        {
            _repo.Add(entity);
        }
    }
}

