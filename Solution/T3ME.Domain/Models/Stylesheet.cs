using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.Models
{
    public class Stylesheet : Entity
    {
        protected Stylesheet() { }

        public Stylesheet(Application app)
        {
            this.App = app;
        }

        public virtual string Filename { get; set; }
        public virtual bool IE8 { get; set; }
        public virtual bool IsPrint { get; set; }
        public virtual int OrderIndex { get; set; }

        public virtual Application App { get; protected set; }
    }
}