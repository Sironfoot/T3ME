using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T3ME.Domain.Models
{
    public abstract class Entity
    {
        public virtual long Id { get; protected set; }

        public virtual bool IsTransient()
        {
            return this.Id == 0;
        }

        public override bool Equals(object obj)
        {
            Entity otherEntity = obj as Entity;

            if (otherEntity == null)
            {
                return false;
            }

            if (this.IsTransient() && otherEntity.IsTransient())
            {
                return Object.ReferenceEquals(this, otherEntity);
            }

            return this.Id == otherEntity.Id;
        }

        public override int GetHashCode()
        {
            if (this.IsTransient())
            {
                return base.GetHashCode();
            }
            else
            {
                return this.Id.GetHashCode();
            }
        }
    }
}
