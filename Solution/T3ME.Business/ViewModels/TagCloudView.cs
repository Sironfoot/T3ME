using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Business.ViewModels
{
    public class TagCloudView
    {
        public enum TagStrenth
        {
            VerySmall,
            Small,
            Medium,
            Large,
            VeryLarge
        }

        public class Tag
        {
            public string Name { get; set; }
            public TagStrenth Strenth { get; set; }
        }

        public TagCloudView()
        {
            Tags = new List<TagCloudView.Tag>();
        }

        public IList<TagCloudView.Tag> Tags { get; protected set; }
    }
}