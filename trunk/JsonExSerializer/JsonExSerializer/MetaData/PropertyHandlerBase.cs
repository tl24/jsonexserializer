using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Base class for Properties and Fields of a Type
    /// </summary>
    public class PropertyHandlerBase : MemberHandlerBase
    {
        protected int _position = -1;
        protected MemberInfo _member;
        public PropertyHandlerBase(MemberInfo member)
            : base(member.DeclaringType)
        {
            _member = member;
        }

        public PropertyHandlerBase(MemberInfo member, int position)
            : this(member)
        {
            _position = position;
        }

        /// <summary>
        ///  The name of the property
        /// </summary>
        public string Name
        {
            get { return _member.Name; }
        }

        public int Position
        {
            get { return _position; }
        }

        public virtual bool IsConstructorArgument
        {
            get { return _position >= 0; }
        }

        protected override IJsonTypeConverter CreateTypeConverter()
        {
            return CreateTypeConverter(_member);
        }
    }
}
