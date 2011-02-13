using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.Models.Components
{
    public class Noun
    {
        protected Noun() { }

        public Noun(string singular, string plural)
        {
            this.Singular = singular;
            this.Plural = plural;
        }

        public string Singular { get; protected set; }
        public string Plural { get; protected set; }
    }
}