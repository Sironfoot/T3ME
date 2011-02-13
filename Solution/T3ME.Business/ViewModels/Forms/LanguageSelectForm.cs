using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Models;

namespace T3ME.Business.ViewModels.Forms
{
    public class LanguageSelectForm
    {
        public IList<Language> Languages { get; set; }
        public int? SelectedLanguage { get; set; }
    }
}