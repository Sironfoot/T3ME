using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using System.Linq.Expressions;
using MvcLibrary.Utils;

namespace MvcLibrary.NHibernate.StrongTyping
{
    public static class RestrictionsFor<T> where T : class
    {
        public static ICriterion Eq(Expression<Func<T, object>> expression, object value)
        {
            string propertyName = Typed.PropertyName.For<T>(expression);
            return Restrictions.Eq(propertyName, value);
        }

        public static ICriterion Gt(Expression<Func<T, object>> expression, object value)
        {
            string propertyName = Typed.PropertyName.For<T>(expression);
            return Restrictions.Gt(propertyName, value);
        }
    }
}