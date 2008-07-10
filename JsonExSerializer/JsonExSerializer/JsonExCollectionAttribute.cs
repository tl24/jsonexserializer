using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited=true)]
    public class JsonExCollectionAttribute : System.Attribute
    {
        private Type _collectionHandlerType;
        private string _collectionHandlerTypeName;

        /// <summary>
        /// Assign the CollectionHandler to this class.  The class will then be treated as a JSON array.
        /// </summary>
        /// <param name="CollectionHandlerType">The type for the CollectionHandler</param>
        /// <see cref="http://code.google.com/p/jsonexserializer/wiki/Collections"/>
        public JsonExCollectionAttribute(Type CollectionHandlerType)
        {
            if (CollectionHandlerType == null)
                throw new ArgumentNullException("CollectionHandlerType can not be null for JsonExCollectionAttribute");
            _collectionHandlerType = CollectionHandlerType;
        }

        /// <summary>
        /// Assign the CollectionHandler to this class.  The class will then be treated as a JSON array.
        /// </summary>
        /// <param name="CollectionHandlerType">The fully-qualified type and assembly name for the CollectionHandler</param>
        /// <see cref="http://code.google.com/p/jsonexserializer/wiki/Collections"/>
        public JsonExCollectionAttribute(string CollectionHandlerType)
        {
            if (string.IsNullOrEmpty(CollectionHandlerType))
                throw new ArgumentException("CollectionHandlerType can not be blank for JsonExCollectionAttribute");

            _collectionHandlerTypeName = CollectionHandlerType;
        }

        public Type GetCollectionHandlerType()
        {
            if (_collectionHandlerType != null)
                return _collectionHandlerType;
            else
                return Type.GetType(_collectionHandlerTypeName);
        }
    }
}
