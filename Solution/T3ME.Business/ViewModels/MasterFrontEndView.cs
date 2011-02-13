using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T3ME.Domain.Models;
using T3ME.Business.ViewModels.Forms;

namespace T3ME.Business.ViewModels
{
    public class MasterFrontEndView
    {
        public MasterFrontEndView(Application app, LanguageSelectForm languages, TagCloudView tagCloud)
        {
            this.App = app;
            this.Languages = languages;
            this.TagCloud = tagCloud;
        }

        public Application App { get; protected set; }
        public LanguageSelectForm Languages { get; protected set; }
        public TagCloudView TagCloud { get; protected set; }

        public Language DefaultLanguage { get; set; }
        public TweeterPanelView TweeterPanel { get; set; }
    }
}