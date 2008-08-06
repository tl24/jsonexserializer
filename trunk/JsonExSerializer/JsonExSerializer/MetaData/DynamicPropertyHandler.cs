using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace JsonExSerializer.MetaData
{
    public class DynamicPropertyHandler : PropertyHandler
    {
        protected delegate void GenericSetter(object target, object value);
        protected delegate object GenericGetter(object target);

        private GenericGetter _getter;
        private GenericSetter _setter;

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
            // on first call we generate the delegate
            if (this.Property.DeclaringType.IsValueType)
                _getter = CreateStructGetMethod(this.Property);
            else
                _getter = CreateClassGetMethod(this.Property);
            return _getter(instance);
        }

        private void FirstCallSetter(object instance, object value)
        {
            _setter = CreateSetMethod(this.Property);
            _setter(instance, value);
        }

        /// <summary>
        /// Creates a dynamic getter for the property
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static GenericGetter CreateClassGetMethod(PropertyInfo propertyInfo)
        {
            /*
             * If there's no getter return null
             */
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
                return null;

            /*
             * Create the dynamic method
             */
            Type[] arguments = new Type[1];
            arguments[0] = typeof(object);

            DynamicMethod getter = new DynamicMethod(
                String.Concat("_Get", propertyInfo.Name, "_"),
                typeof(object), arguments, propertyInfo.DeclaringType);
            
            ILGenerator generator = getter.GetILGenerator();
            
            generator.DeclareLocal(typeof(object));
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
            generator.EmitCall(OpCodes.Callvirt, getMethod, null);

            if (!propertyInfo.PropertyType.IsClass)
                generator.Emit(OpCodes.Box, propertyInfo.PropertyType);

            generator.Emit(OpCodes.Ret);

            /*
             * Create the delegate and return it
             */
            return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
        }

        /// <summary>
        /// Creates a dynamic getter for the property
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static GenericGetter CreateStructGetMethod(PropertyInfo propertyInfo)
        {
            /*
             * If there's no getter return null
             */
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
                return null;

            /*
             * Create the dynamic method
             */
            Type[] arguments = new Type[1];
            arguments[0] = typeof(object);

            string mName = propertyInfo.GetGetMethod().Name;
            DynamicMethod getter = new DynamicMethod(
                String.Concat("_Get", propertyInfo.Name, "_"),
                typeof(object), arguments, propertyInfo.DeclaringType);
            ILGenerator generator = getter.GetILGenerator();
            generator.DeclareLocal(typeof(object)); // return value
            generator.DeclareLocal(propertyInfo.DeclaringType); // struct instance

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Unbox_Any, propertyInfo.DeclaringType);
            generator.Emit(OpCodes.Stloc_1); // get the unboxed value from the stack
            //generator.Emit(OpCodes.Ldloc_1);
            generator.Emit(OpCodes.Ldloca_S, 1);
            generator.EmitCall(OpCodes.Call, getMethod, null);

            if (!propertyInfo.PropertyType.IsClass)
                generator.Emit(OpCodes.Box, propertyInfo.PropertyType);

            generator.Emit(OpCodes.Ret);

            /*
             * Create the delegate and return it
             */
            return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
        }

        /// <summary>
        /// Creates a dynamic setter for the property
        /// </summary>
        /// <param name="propertyInfo"></param>
        private static GenericSetter CreateSetMethod(PropertyInfo propertyInfo)
        {
            /*
             * If there's no setter return null
             */
            MethodInfo setMethod = propertyInfo.GetSetMethod();
            if (setMethod == null)
                return null;

            /*
             * Create the dynamic method
             */
            Type[] arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            DynamicMethod setter = new DynamicMethod(
                String.Concat("_Set", propertyInfo.Name, "_"),
                typeof(void), arguments, propertyInfo.DeclaringType);
            ILGenerator generator = setter.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            if (propertyInfo.DeclaringType.IsClass)
                generator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
            else
                generator.Emit(OpCodes.Unbox_Any, propertyInfo.DeclaringType);

            generator.Emit(OpCodes.Ldarg_1);

            if (propertyInfo.PropertyType.IsClass)
                generator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
            else
                generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);

            generator.EmitCall(OpCodes.Callvirt, setMethod, null);
            generator.Emit(OpCodes.Ret);

            /*
             * Create the delegate and return it
             */
            return (GenericSetter)setter.CreateDelegate(typeof(GenericSetter));
        }


    }
}
