using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Collections;

namespace JsonExSerializer.Expression
{
    public class CollectionBuilderEvaluator : IEvaluator
    {
        private ExpressionBase _expression;
        private SerializationContext _context;
        private IEvaluator _defaultEvaluator;
        private ICollectionBuilder _converter;
        private bool _isEvaluating = false;
        private ICollectionBuilder _builder;
        private Type _itemType;
        private object _result;

        public CollectionBuilderEvaluator(ExpressionBase expression) 
        {
            _expression = expression;
        }


        private void ConstructBuilder()
        {
            ListExpression list = (ListExpression)Expression;
            TypeHandler handler = Context.GetTypeHandler(list.ResultType);
            _itemType = handler.GetCollectionItemType();
            _builder = handler.GetCollectionBuilder(list.Items.Count);
        }


        public object Evaluate()
        {
            if (_result == null)
            {
                if (_builder == null)
                    ConstructBuilder();

                ListExpression list = (ListExpression)Expression;
                foreach (ExpressionBase item in list.Items)
                {
                    item.SetResultTypeIfNotSet(_itemType);
                    object itemResult = item.Evaluate(Context);
                    _builder.Add(itemResult);
                }
                _result = _builder.GetResult();
                _builder = null;
                if (_result is IDeserializationCallback)
                {
                    ((IDeserializationCallback)_result).OnAfterDeserialization();
                }
            }
            return _result;
        }

        public object GetReference()
        {
            if (_result == null)
            {
                if (_builder == null)
                    ConstructBuilder();
                return _builder.GetReference();
            }
            else
            {
                return _result;
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
