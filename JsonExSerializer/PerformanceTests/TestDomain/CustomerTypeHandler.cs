using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.MetaData;
using JsonExSerializer;

namespace PerformanceTests.TestDomain
{
    public class CustTypeHandlerFactory : TypeDataRepository
    {
        public CustTypeHandlerFactory(IConfiguration config)
            : base(config)
        {
        }

        protected override TypeData CreateNew(Type forType)
        {
            if (forType == typeof(Customer))
                return new CustomerTypeHandler(forType, this.Config);
            else
                return base.CreateNew(forType);
        }
    }
    public class CustomerTypeHandler : TypeData
    {
        public CustomerTypeHandler(Type t, IConfiguration config)
            : base(t, config)
        {
        }

        protected override IList<IPropertyData> ReadDeclaredProperties()
        {
            IList<IPropertyData> properties = new List<IPropertyData>();
            properties.Add(new FirstPH(this.ForType, this));
            properties.Add(new LastPH(this.ForType, this));
            properties.Add(new PhonePH(this.ForType, this));
            properties.Add(new SSNPH(this.ForType, this));
            properties.Add(new AgePH(this.ForType, this));
            return properties;
        }
    }

    public abstract class CustomerPHBase : AbstractPropertyData
    {
        private string _name;

        public CustomerPHBase(Type forType, TypeData parent, string name) : base(forType, parent) {
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
        public FirstPH(Type forType, TypeData parent)
            : base(forType, parent, "FirstName")
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
        public LastPH(Type forType, TypeData parent)
            : base(forType, parent, "LastName")
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
        public PhonePH(Type forType, TypeData parent)
            : base(forType, parent, "Phone")
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
        public SSNPH(Type forType, TypeData parent)
            : base(forType, parent, "Ssn")
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
        public AgePH(Type forType, TypeData parent)
            : base(forType, parent, "Age")
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
