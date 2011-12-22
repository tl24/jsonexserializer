using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Framework.Expressions;

namespace JsonExSerializer.Framework.ExpressionHandlers
{
    /// <summary>
    /// A default instance of IExpressionHandler to use as a base class.
    /// </summary>
    public abstract class ExpressionHandlerBase : IExpressionHandler, IConfigurationAware
    {
        private IConfiguration _config;

        /// <summary>
        /// Initializes a default instance without a Serialization Context.  Protected since the class is abstract.
        /// </summary>
        protected ExpressionHandlerBase()
        {
        }

        /// <summary>
        /// Initializes an instance with a Serialization Context.
        /// </summary>
        /// <param name="Context">the Serialization Context</param>
        protected ExpressionHandlerBase(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Gets/sets the Serialization Context, which can be used to retrieve type information, serialization options, etc.
        /// </summary>
        public virtual IConfiguration Config
        {
            get { return _config; }
            set { _config = value; }
        }

        /// <summary>
        /// Take an object and convert it to an expression to be serialized.  Child names/indexes should be appended to the
        /// currentPath before serializing child objects.
        /// </summary>
        /// <param name="data">the object to convert</param>
        /// <param name="CurrentPath">the current path to this object from the root, used for tracking references</param>
        /// <param name="serializer">serializer instance for serializing child objects</param>
        /// <returns>an expression which represents a json structure</returns>
        public abstract Expression GetExpression(object data, JsonPath currentPath, IExpressionBuilder serializer);

        /// <summary>
        /// Checks to see if this handler is able to convert an this object type to an expression
        /// </summary>
        /// <param name="objectType">the object type that will be serialized</param>
        /// <returns>true if this handler can handle the type</returns>
        public abstract bool CanHandle(Type objectType);

        /// <summary>
        /// Checks to see if this handler is able to convert the expression back into an object.
        /// </summary>
        /// <param name="expression">the expression that will be deserialized</param>
        /// <returns>true if this handler can handle the expression</returns>
        public virtual bool CanHandle(Expression expression)
        {
            return false;
        }

        /// <summary>
        /// Convert the expression into an object by creating a new instance of the desired type and
        /// populating it with any necessary values.
        /// </summary>
        /// <param name="expression">the epxression to deserialize</param>
        /// <param name="deserializer">deserializer instance to use to deserialize any child expressions</param>
        /// <returns>a fully deserialized object</returns>
        public abstract object Evaluate(Expression expression, IDeserializerHandler deserializer);

        /// <summary>
        /// Convert the expression into an object by populating an existing object with any necessary values.
        /// The existingObject will usually come from the get method of a property on an object that doesn't
        /// allow writing to the property.
        /// </summary>
        /// <param name="expression">the epxression to deserialize</param>
        /// <param name="existingObject">an existing object to populate</param>
        /// <param name="deserializer">deserializer instance to use to deserialize any child expressions</param>
        /// <returns>a fully deserialized object</returns>
        public abstract object Evaluate(Expression expression, object existingObject, IDeserializerHandler deserializer);


        /// <summary>
        /// Check to see if the value to be handled by this handler should be treated as a reference type.  This method
        /// will only be called for this handler if it returns true from CanHandle.  If this is a referenceable type,
        /// then reference-handling logic will occur based on the ReferenceOption settings.
        /// </summary>
        /// <param name="value">the value to check</param>
        /// <returns>true if the value is a type that should be treated as a reference type by the serializer</returns>
        public virtual bool IsReferenceable(object value)
        {
            if (value == null)
                return false;
            return this.Config.IsReferenceableType(value.GetType());
        }

        protected T CastExpression<T>(Expression expr) where T : Expression
        {
            T result = expr as T;
            if (result == null)
            {
                throw new InvalidOperationException("Expecting " + typeof(T).Name + " but received " + expr.GetType().Name + ".  Perhaps a type converter declaration is missing");
            }
            return result;
        }
    }
}
