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
    public abstract class PropertyHandlerBase : AbstractPropertyHandler
    {
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
        public override string Name
        {
            get { return _member.Name; }
        }

        protected override IJsonTypeConverter CreateTypeConverter()
        {
            return CreateTypeConverter(_member);
        }
    }
}
