using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using System.Linq.Expressions;
using MvcLibrary.Utils;

namespace MvcLibrary.NHibernate.StrongTyping
{
    public class OrderFor<T> where T : class
    {
        public static Order Desc(Expression<Func<T, object>> expression)
        {
            string propertyName = Typed.PropertyName.For<T>(expression);
            return Order.Desc(propertyName);
        }

        public static Order Asc(Expression<Func<T, object>> expression)
        {
            string propertyName = Typed.PropertyName.For<T>(expression);
            return Order.Asc(propertyName);
        }
    }
}