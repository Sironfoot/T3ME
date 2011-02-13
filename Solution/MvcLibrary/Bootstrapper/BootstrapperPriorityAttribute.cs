using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcLibrary.Bootstrapper
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BootstrapperPriorityAttribute : Attribute
    {
        private int _priority = 0;

        public BootstrapperPriorityAttribute() { }

        public BootstrapperPriorityAttribute(int priority)
        {
            _priority = priority;
        }

        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
    }
}
