using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using JsonExSerializer.TypeConversion;
using JsonExSerializer.Framework;

namespace JsonExSerializer.MetaData
{
    /// <summary>
    /// Base class for Properties and Fields of a Type
    /// </summary>
    public abstract class MemberInfoPropertyDataBase : AbstractPropertyData
    {
        protected MemberInfo member;
        private Func<object, object> _getter;
        private Action<object, object> _setter;
        protected MemberInfoPropertyDataBase(MemberInfo member, TypeData parent)
            : base(member.DeclaringType, parent)
        {
            this.member = member;
        }
        
        /// <summary>
        ///  The name of the property
        /// </summary>
        public override string Name
        {
            get { return member.Name; }
        }

        public override object GetValue(object instance)
        {
            return (_getter ?? (_getter = TypeData.GetCompiledGetter(member)))(instance);
        }

        public override void SetValue(object instance, object value)
        {
            (_setter ?? (_setter = TypeData.GetCompiledSetter(member)))(instance, value);
        }
    }
}
