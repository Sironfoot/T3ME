using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.Models
{
    public class Language : Entity
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsRecognised { get; set; }
    }
}