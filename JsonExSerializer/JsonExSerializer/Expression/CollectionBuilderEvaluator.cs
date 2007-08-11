using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer.Collections;

namespace JsonExSerializer.Expression
{
    public class CollectionBuilderEvaluator : IEvaluator {
        protected ExpressionBase _expression;
        protected SerializationContext _context;
        private IEvaluator _defaultEvaluator;
        private ICollectionBuilder _converter;
        private bool _isEvaluating = false;
        // the result (cached for repeated calls)
        protected object _result;  


        public CollectionBuilderEvaluator(ExpressionBase expression)
        {
            _expression = expression;
        }



        #region IEvaluator Members

        public object Evaluate()
        {
            if (_result == null)
            {
                ListExpression _list = (ListExpression)_expression;
                TypeHandler handler = Context.GetTypeHandler(_list.ResultType);
                Type itemType = handler.GetCollectionItemType();
                ICollectionBuilder builder = handler.GetCollectionBuilder();
                foreach (ExpressionBase item in _list.Items)
                {
                    item.SetResultTypeIfNotSet(itemType);
                    object itemResult = item.Evaluate(Context);
                    builder.Add(itemResult);
                }
                _result = builder.GetResult();

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
                throw new InvalidOperationException("Collections built using CollectionBuilders can't be referenced during their creation.");
            }
            else
            {
                return _result;
            }
        }

        public ExpressionBase Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        public SerializationContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        #endregion
    }
}
