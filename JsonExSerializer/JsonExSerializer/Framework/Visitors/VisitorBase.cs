/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace JsonExSerializer.Framework.Visitors
{
    public class VisitorBase : IVisitor
    {
        private IDictionary<Type, MethodInfo> _methodCache;
        public void Visit(object o)
        {
            if (_methodCache == null)
                BuildMethodCache();

            Type t = o.GetType();
            Visit(t, o);
        }

        protected void Visit(Type t, object o)
        {
            if (t == null || t == typeof(object))
            {
                VisitorNotFound(t, o);
                return;
            }
            if (_methodCache.ContainsKey(t))
                _methodCache[t].Invoke(this, new object[] { o });
            else
                Visit(t.BaseType, o);
        }

        protected virtual void VisitorNotFound(Type t, object o)
        {
        }

        private void BuildMethodCache()
        {
            _methodCache = new Dictionary<Type, MethodInfo>();
            foreach (MethodInfo m in this.GetType().GetMethods())
            {
                if (!m.Name.ToLower().StartsWith("visit"))
                    continue;
                ParameterInfo[] parms = m.GetParameters();
                if (parms.Length != 1)
                    continue;
                if (parms[0].ParameterType == typeof(object))
                    continue;

                _methodCache.Add(parms[0].ParameterType, m);
            }
        }
    }
}
