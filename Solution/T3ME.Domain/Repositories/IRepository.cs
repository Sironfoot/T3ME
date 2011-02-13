using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Models;

namespace T3ME.Domain.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        T FromId(long id);
        void Save(T entity);
        void Delete(T entity);

        IQueryable<T> Query();
        IQueryable<E> Query<E>() where E : Entity;
    }
}