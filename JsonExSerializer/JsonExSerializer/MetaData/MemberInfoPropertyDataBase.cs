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
        protected MemberInfoPropertyDataBase(MemberInfo member)
            : base(member.DeclaringType)
        {
            this.member = member;
        }

        protected MemberInfoPropertyDataBase(MemberInfo member, int position)
            : this(member)
        {
            this.position = position;
        }

        protected MemberInfoPropertyDataBase(MemberInfo member, string constructorParameterName)
            : this(member)
        {
            this.constructorParameterName = constructorParameterName;
        }

        protected MemberInfoPropertyDataBase(MemberInfo member, bool isConstructorParameter)
            : base(member.DeclaringType)
        {
            this.member = member;
            if (isConstructorParameter)
                this.constructorParameterName = member.Name;
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
