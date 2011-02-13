using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Repositories;
using T3ME.Domain.Models;
using NHibernate.Criterion;
using MvcLibrary.NHibernate.StrongTyping;
using MvcLibrary.NHibernate.SessionManagement;

namespace T3ME.Domain.NHibernate.Repositories
{
    public class ApplicationRepository : NHibernateRepository<Application>, IApplicationRepository
    {
        public ApplicationRepository(ISessionProviderFactory sessionProviderFactory) : base(sessionProviderFactory) { }

        public Application GetApplicationFromUrl(string url)
        {
            return Session
                .CreateCriteria<Application>()
                .Add(RestrictionsFor<Application>.Eq(a => a.Url, url))
                .SetMaxResults(1)
                .UniqueResult<Application>();
        }

        public IList<Application> ListApplications()
        {
            return Session.CreateCriteria<Application>()
                .AddOrder(OrderFor<Application>.Asc(a => a.Title))
                .List<Application>();
        }
    }
}