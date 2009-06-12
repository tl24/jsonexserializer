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
    }
}
