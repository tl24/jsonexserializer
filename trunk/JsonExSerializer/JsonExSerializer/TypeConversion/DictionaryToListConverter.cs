/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using JsonExSerializer.MetaData;

namespace JsonExSerializer.TypeConversion
{
    /// <summary>
    /// Converts a dictionary of objects to a list.  On Deserialization, a property
    /// of the value type is used as the key.
    /// </summary>
    public class DictionaryToListConverter : IJsonTypeConverter
    {
        private Type _sourceType;
        private string _context;

        #region IJsonTypeConverter Members

        public Type GetSerializedType(Type sourceType)
        {
            return typeof(Object);
        }

        public object ConvertFrom(object item, SerializationContext serializationContext)
        {
            IDictionary dictionary = (IDictionary)item;
            return dictionary.Values;
        }

        public object ConvertTo(object item, Type sourceType, SerializationContext serializationContext)
        {
            IDictionary dictionary = (IDictionary) Activator.CreateInstance(sourceType);
            AbstractPropertyHandler propHandler = null;
            ICollection coll = (ICollection)item;
            foreach (object colItem in coll)
            {
                if (propHandler == null)
                {
                    propHandler = serializationContext.GetTypeHandler(_sourceType).FindProperty(_context);
                    if (propHandler == null)
                    {
                        throw new MissingMemberException("Type: " + item.GetType().Name + " does not have an accessible property: " + _context);
                    }
                }

                dictionary[propHandler.GetValue(colItem)] = colItem;
            }
            return dictionary;
        }

        public object Context
        {
            set { _context = value != null ? value.ToString() : ""; }
        }

        #endregion
    }
}
