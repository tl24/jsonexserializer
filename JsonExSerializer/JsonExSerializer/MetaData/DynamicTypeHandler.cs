using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace JsonExSerializer.MetaData
{
    public class DynamicTypeHandler : TypeHandler
    {
        private delegate object ConstructDelegate();
        private ConstructDelegate _noArgConstructor;

        public DynamicTypeHandler(Type t, SerializationContext Context)
            : base(t, Context)
        {
        }

        protected override PropertyHandler CreatePropertyHandler(PropertyInfo Property)
        {
            return new DynamicPropertyHandler(Property);
        }

        public override object CreateInstance(object[] args)
        {
            if ((args == null || args.Length == 0) && !this.ForType.IsValueType)
                return DynamicConstruct();
            else
                return base.CreateInstance(args);
        }

        private object DynamicConstruct()
        {
            if (_noArgConstructor == null)
            {
                _noArgConstructor = BuildObjectConstructor();
            }
            return _noArgConstructor();
        }

        private ConstructDelegate BuildObjectConstructor()
        {
            ConstructorInfo cInfo = this.ForType.GetConstructor(Type.EmptyTypes);
            DynamicMethod method = new DynamicMethod(string.Concat("_ctor", this.ForType.Name, "_"), typeof(object), Type.EmptyTypes, this.ForType);
            ILGenerator generator = method.GetILGenerator();
            // declare return value
            generator.DeclareLocal(typeof(object));
            generator.Emit(OpCodes.Newobj, cInfo);
            //generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Ret);

            return (ConstructDelegate) method.CreateDelegate(typeof(ConstructDelegate));
        }
    }
}
