using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace JsonExSerializer.MetaData
{
    public class DynamicPropertyHandler : PropertyHandler
    {
        private DynamicMethodUtil.GenericGetter _getter;
        private DynamicMethodUtil.GenericSetter _setter;

        public DynamicPropertyHandler(PropertyInfo PropertyInfo)
            : base(PropertyInfo)
        {
            Initialize();
        }

        public DynamicPropertyHandler(PropertyInfo PropertyInfo, int Position)
            : base(PropertyInfo, Position)
        {
            Initialize();
        }

        private void Initialize()
        {
            _getter = FirstCallGetter;
            _setter = FirstCallSetter;
        }

        public override object GetValue(object instance)
        {
            return _getter(instance);
        }

        
        public override void SetValue(object instance, object value)
        {
            _setter(instance, value);
        }

        private object FirstCallGetter(object instance)
        {
            _getter = DynamicMethodUtil.CreatePropertyGetter(this.Property);
            return _getter(instance);
        }

        private void FirstCallSetter(object instance, object value)
        {
            _setter = DynamicMethodUtil.CreatePropertySetter(this.Property);
            _setter(instance, value);
        }

    }
}
