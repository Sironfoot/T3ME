using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.Models
{
    public class PersistentLogin : Entity
    {
        protected PersistentLogin() { }

        public PersistentLogin(Tweeter tweeter)
        {
            this.Tweeter = tweeter;
        }

        public virtual Tweeter Tweeter { get; protected set; }

        public virtual string SecureKey { get; set; }
        public virtual DateTime LastLoginDate { get; set; }
    }
}