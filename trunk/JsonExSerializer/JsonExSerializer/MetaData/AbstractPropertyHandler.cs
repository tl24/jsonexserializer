using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Base Implementation of an object property
    /// </summary>
    public abstract class AbstractPropertyHandler : MemberHandlerBase, IPropertyHandler
    {
        protected bool _ignored;
        protected int _position = -1;

        protected AbstractPropertyHandler(Type ForType)
            : base(ForType)
        {
        }

        public abstract string Name { get; }

        public virtual int Position {
            get { return _position; }
        }

        public abstract Type PropertyType { get; }

        public abstract object GetValue(object instance);
        public abstract void SetValue(object instance, object value);


        public virtual bool IsConstructorArgument
        {
            get { return this.Position != -1; }
        }


        public virtual bool Ignored
        {
            get { return _ignored; }
            set { _ignored = value; }
        }

    }
}
