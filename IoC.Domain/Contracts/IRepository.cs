using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.Domain.Contracts
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();

        T GetById(string id);

        void Insert(T item);

        void Update(T item);

        void DeleteById(string id);
    }
}
