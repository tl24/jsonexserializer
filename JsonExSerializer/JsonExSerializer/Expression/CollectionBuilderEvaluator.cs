/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Collections;
using JsonExSerializer.MetaData;

namespace JsonExSerializer.Expression
{
    sealed class CollectionBuilderEvaluator : IEvaluator
    {
        private ExpressionBase _expression;
        private SerializationContext _context;
        private IEvaluator _defaultEvaluator;
        private ICollectionBuilder _converter;
        private bool _isEvaluating = false;
        private ICollectionBuilder _builder;
        private Type _itemType;
        private object _result;
        private bool _constructedEventSent = false;
        public CollectionBuilderEvaluator(ExpressionBase expression) 
        {
            _expression = expression;
        }

        public void SetResult(object value)
        {
            if (_builder != null)
                throw new InvalidOperationException("The collection result can not be set because it has already been evaluated");
            ConstructBuilder(value);
        }

        private void ConstructBuilder(object collection)
        {
            ListExpression list = (ListExpression)Expression;
            TypeHandler typeHandler = Context.GetTypeHandler(list.ResultType);
            CollectionHandler collHandler = typeHandler.GetCollectionHandler();
            _itemType = collHandler.GetItemType(typeHandler.ForType);
            if (collection != null)
                _builder = collHandler.ConstructBuilder(collection);
            else
                _builder = collHandler.ConstructBuilder(typeHandler.ForType, list.Items.Count);
            
        }


        public object Evaluate()
        {
            if (_result == null)
            {
                ICollectionBuilder builder = this.Builder;
                ListExpression list = (ListExpression)Expression;
                try
                {
                    _result = builder.GetReference();
                    list.OnObjectConstructed(_result);
                    _constructedEventSent = true;
                }
                catch
                {
                    // this might fail if the builder's not ready
                }
                foreach (ExpressionBase item in list.Items)
                {
                    item.SetResultTypeIfNotSet(_itemType);
                    object itemResult = item.Evaluate(Context);
                    _builder.Add(itemResult);
                }
                _result = _builder.GetResult();
                if (!_constructedEventSent)
                    list.OnObjectConstructed(_result);
                builder = null;
                if (_result is IDeserializationCallback)
                {
                    ((IDeserializationCallback)_result).OnAfterDeserialization();
                }
            }
            return _result;
        }

        private ICollectionBuilder Builder
        {
            get
            {
                if (_builder == null)
                    ConstructBuilder(null);
                return _builder;
            }
        }
        /// <summary>
        /// The expression being evaluated
        /// </summary>
        public ExpressionBase Expression
        {
            get { return this._expression; }
            set { this._expression = value; }
        }

        public SerializationContext Context
        {
            get { return this._context; }
            set { this._context = value; }
        }

    }
}
