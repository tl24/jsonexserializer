using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.MetaData;
using JsonExSerializer;

namespace PerformanceTests.TestDomain
{
    public class CustTypeHandlerFactory : TypeDataRepository
    {
        public CustTypeHandlerFactory(SerializationContext Context)
            : base(Context)
        {
        }

        protected override TypeData CreateNew(Type forType)
        {
            if (forType == typeof(Customer))
                return new CustomerTypeHandler(forType, this.Context);
            else
                return base.CreateNew(forType);
        }
    }
    public class CustomerTypeHandler : TypeData
    {
        public CustomerTypeHandler(Type t, SerializationContext ctx)
            : base(t, ctx)
        {
        }

        protected override IList<IPropertyData> ReadProperties()
        {
            IList<IPropertyData> properties = new List<IPropertyData>();
            properties.Add(new FirstPH(this.ForType));
            properties.Add(new LastPH(this.ForType));
            properties.Add(new PhonePH(this.ForType));
            properties.Add(new SSNPH(this.ForType));
            properties.Add(new AgePH(this.ForType));
            return properties;
        }
    }

    public abstract class CustomerPHBase : AbstractPropertyData
    {
        private string _name;

        public CustomerPHBase(Type forType, string name) : base(forType) {
            _name = name;
        }

        public override string Name { get { return _name; } }
        public override bool Ignored
        {
            get { return false; }
            set { ; }
        }

        protected override JsonExSerializer.TypeConversion.IJsonTypeConverter CreateTypeConverter()
        {
            return null;
        }
    }

    public class FirstPH : CustomerPHBase
    {
        public FirstPH(Type forType)
            : base(forType, "FirstName")
        {
        }

        public override object GetValue(object instance)
        {
            return ((Customer)instance).FirstName;
        }
        public override Type PropertyType
        {
            get { return typeof(string); }
        }
        public override void SetValue(object instance, object value)
        {
            ((Customer)instance).FirstName = (string)value;
        }
    }
    public class LastPH : CustomerPHBase
    {
        public LastPH(Type forType)
            : base(forType, "LastName")
        {
        }

        public override object GetValue(object instance)
        {
            return ((Customer)instance).LastName;
        }
        public override Type PropertyType
        {
            get { return typeof(string); }
        }
        public override void SetValue(object instance, object value)
        {
            ((Customer)instance).LastName = (string)value;
        }
    }
    public class PhonePH : CustomerPHBase
    {
        public PhonePH(Type forType)
            : base(forType, "Phone")
        {
        }

        public override object GetValue(object instance)
        {
            return ((Customer)instance).Phone;
        }
        public override Type PropertyType
        {
            get { return typeof(string); }
        }
        public override void SetValue(object instance, object value)
        {
            ((Customer)instance).Phone = (string)value;
        }
    }
    public class SSNPH : CustomerPHBase
    {
        public SSNPH(Type forType)
            : base(forType, "Ssn")
        {
        }

        public override object GetValue(object instance)
        {
            return ((Customer)instance).Ssn;
        }
        public override Type PropertyType
        {
            get { return typeof(string); }
        }
        public override void SetValue(object instance, object value)
        {
            ((Customer)instance).Ssn = (string)value;
        }
    }
    public class AgePH : CustomerPHBase
    {
        public AgePH(Type forType)
            : base(forType, "Age")
        {
        }

        public override object GetValue(object instance)
        {
            return ((Customer)instance).Age;
        }
        public override Type PropertyType
        {
            get { return typeof(int); }
        }
        public override void SetValue(object instance, object value)
        {
            ((Customer)instance).Age = (int)value;
        }
    }

}
