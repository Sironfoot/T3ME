using System;
using AutoMapper;

namespace MvcLibrary.AutoMapper
{
    /// <summary>
    ///     An improvement on AutoMapper's TypeConverter type to include access to an existing Destination type
    /// </summary>
    /// <typeparam name="TSource">The Source type being converted from</typeparam>
    /// <typeparam name="TDestination">The Destination type being converted to</typeparam>
    public abstract class AdvancedTypeConverter<TSource, TDestination> : ITypeConverter<TSource, TDestination>
    {
        private ResolutionContext Context = null;

        public TDestination Convert(ResolutionContext context)
        {
            Context = context;

            if (Context.SourceValue != null && !(Context.SourceValue is TSource))
            {
                string message = "Value supplied is of type {0} but expected {1}.\n" +
                                 "Change the type converter source type, or redirect the source value supplied to the value resolver using FromMember.";

                throw new AutoMapperMappingException(Context, string.Format(message, typeof(TSource), Context.SourceValue.GetType()));
            }

            return ConvertCore((TSource)Context.SourceValue);
        }

        protected TDestination ExistingDestination
        {
            get
            {
                if (Context == null)
                {
                    string message = "ResolutionContext is not yet set. Only call this property inside the 'ConvertCore' method.";
                    throw new InvalidOperationException(message);
                }

                if (Context.DestinationValue != null && !(Context.DestinationValue is TDestination))
                {
                    string message = "Destination Value is of type {0} but expected {1}.";

                    throw new AutoMapperMappingException(Context, string.Format(message, typeof(TDestination), Context.DestinationValue.GetType()));
                }

                return (TDestination)Context.DestinationValue;
            }
        }

        protected abstract TDestination ConvertCore(TSource source);
    }
}