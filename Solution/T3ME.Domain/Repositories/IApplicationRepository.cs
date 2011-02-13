using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Models;

namespace T3ME.Domain.Repositories
{
    public interface IApplicationRepository : IRepository<Application>
    {
        Application GetApplicationFromUrl(string url);
        IList<Application> ListApplications();
    }
}