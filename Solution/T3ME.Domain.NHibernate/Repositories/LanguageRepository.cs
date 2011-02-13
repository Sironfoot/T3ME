using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Models;
using T3ME.Domain.Repositories;
using MvcLibrary.NHibernate.SessionManagement;
using NHibernate.Linq;

namespace T3ME.Domain.NHibernate.Repositories
{
    public class LanguageRepository : NHibernateRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(ISessionProviderFactory sessionProviderFactory) : base(sessionProviderFactory) { }

        public IList<Language> ListLanguages(bool includeUnrecognised)
        {
            var query = Session
                .Query<Language>();

            if(!includeUnrecognised)
            {
                query = query.Where(l => l.IsRecognised);
            }

            return query.OrderBy(l => l.Name)
                .ToList();
        }
    }
}