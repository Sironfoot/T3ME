using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Repositories;
using T3ME.Domain.Models;
using NHibernate;
using NHibernate.Linq;
using MvcLibrary.NHibernate.SessionManagement;

namespace T3ME.Domain.NHibernate.Repositories
{
    public abstract class NHibernateRepository<T> : IRepository<T> where T : Entity
    {
        protected readonly ISessionProviderFactory SessionProviderFactory = null;

        public NHibernateRepository(ISessionProviderFactory sessionProviderFactory)
        {
            this.SessionProviderFactory = sessionProviderFactory;
        }

        protected ISession Session
        {
            get { return SessionProviderFactory.GetSessionProvider().GetSession(); }
        }

        public T FromId(long id)
        {
            return Session.Get<T>(id);
        }

        public void Save(T entity)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(entity);
                transaction.Commit();
            }
        }

        public void Delete(T entity)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.Delete(entity);
                transaction.Commit();
            }
        }

        public IQueryable<T> Query()
        {
            return Session.Query<T>();
        }

        public IQueryable<E> Query<E>() where E : Entity
        {
            return Session.Query<E>();
        }
    }
}