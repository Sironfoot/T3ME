using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Models;

namespace T3ME.Domain.Repositories
{
    public interface ILanguageRepository : IRepository<Language>
    {
        /// <summary>
        ///     Returns a complete list of all the Languages in the database,
        ///     by ascending alphebetical order of their 'Name'
        /// </summary>
        /// <param name="includeUnrecognised">
        ///     Include unrecognised Languages
        /// </param>
        IList<Language> ListLanguages(bool includeUnrecognised);
    }
}