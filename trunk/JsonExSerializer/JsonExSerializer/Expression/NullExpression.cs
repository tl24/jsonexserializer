/*
 * Copyright (c) 2007, Ted Elliott
 * Code licensed under the New BSD License:
 * http://code.google.com/p/jsonexserializer/wiki/License
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExSerializer.Expression
{
    sealed class NullExpression : ExpressionBase
    {

        public override Type DefaultType
        {
            get { return typeof(object); }
        }
    }
}
