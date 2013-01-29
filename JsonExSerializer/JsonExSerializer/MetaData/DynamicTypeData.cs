using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace JsonExSerializer.MetaData
{
    public class DynamicTypeData : TypeData
    {
        private delegate object ConstructDelegate();
        private ConstructDelegate _noArgConstructor;

        public DynamicTypeData(Type t, ISerializerSettings config)
            : base(t, config)
        {
        }

        protected override PropertyData CreatePropertyHandler(PropertyInfo Property)
        {
            return new DynamicPropertyData(Property, this);
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
