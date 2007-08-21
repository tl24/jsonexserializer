using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace JsonExSerializer
{
    public abstract class JsonWriter : IJsonWriter
    {
        protected TextWriter _writer;

        protected enum OpType
        {
            CtorStart,
            CtorArgStart,
            CtorArgEnd,
            CtorEnd,
            ObjStart,
            ObjEnd,
            ArrStart,
            ArrEnd,
            OpKey,
            OpValue,
            OpCast
        }

        protected short indentLevel = 0;
        protected short indentSize = 4;
        private IState _currentState;

        public JsonWriter(TextWriter writer, bool indent) {
            this._writer = writer;
            _currentState = new InitialState(this);
            if (!indent)
                indentSize = 0;
        }

        private void PreWrite(OpType operation) {
            _currentState.PreWrite(operation);
            return;
        }

        protected void WriteIndent()
        {
            if (indentSize > 0)
                _writer.Write("".PadRight(indentSize * indentLevel));
        }

        protected void WriteLineBreak()
        {
            if (indentSize > 0)
                _writer.Write("\r\n");
        }

        public IJsonWriter ConstructorStart(string NamespaceAndClass)
        {
            PreWrite(OpType.CtorStart);
            _writer.Write("new ");
            WriteTypeInfo(NamespaceAndClass);

            return this;
        }

        public IJsonWriter ConstructorStart(string NamespaceAndClass, string Assembly)
        {
            PreWrite(OpType.CtorStart);
            _writer.Write("new ");
            WriteTypeInfo(NamespaceAndClass, Assembly);

            return this;
        }

        public IJsonWriter ConstructorStart(Type constructorType)
        {
            PreWrite(OpType.CtorStart);
            _writer.Write("new ");
            WriteTypeInfo(constructorType);

            return this;
        }

        public IJsonWriter ConstructorArgsStart()
        {
            PreWrite(OpType.CtorArgStart);
            _writer.Write("(");
            return this;
        }

        public IJsonWriter ConstructorArgsEnd()
        {
            PreWrite(OpType.CtorArgEnd);
            _writer.Write(")");
            return this;
        }

        public IJsonWriter ConstructorEnd()
        {
            PreWrite(OpType.CtorEnd);
            return this;
        }

        public IJsonWriter ObjectStart()
        {
            PreWrite(OpType.ObjStart);
            _writer.Write('{');
            return this;
        }

        public IJsonWriter Key(string key)
        {
            PreWrite(OpType.OpKey);
            WriteQuotedString(key);
            _writer.Write(':');
            return this;
        }

        public IJsonWriter ObjectEnd()
        {
            PreWrite(OpType.ObjEnd);
            _writer.Write('}');
            return this;
        }

        public IJsonWriter ArrayStart()
        {
            PreWrite(OpType.ArrStart);
            _writer.Write('[');
            return this;
        }

        public IJsonWriter ArrayEnd()
        {
            PreWrite(OpType.ArrEnd);
            _writer.Write(']');
            return this;
        }

        public IJsonWriter Value(bool value)
        {
            PreWrite(OpType.OpValue);
            _writer.Write(value);
            return this;
        }

        public IJsonWriter Value(long value)
        {
            PreWrite(OpType.OpValue);
            _writer.Write(value);
            return this;
        }

        public IJsonWriter Value(double value)
        {
            PreWrite(OpType.OpValue);
            _writer.Write(value.ToString("R"));
            return this;
        }

        public IJsonWriter Value(float value)
        {
            PreWrite(OpType.OpValue);
            _writer.Write(value.ToString("R"));
            return this;
        }

        public IJsonWriter QuotedValue(string value)
        {
            PreWrite(OpType.OpValue);
            WriteQuotedString(value);
            return this;
        }

        public IJsonWriter SpecialValue(string value)
        {
            PreWrite(OpType.OpValue);
            _writer.Write(value);
            return this;
        }

        public IJsonWriter Comment(string comment)
        {
            // no prewrite, a comment is valid anywhere at any time
            // and does not alter the state
            _writer.Write(comment);
            return this;
        }

        protected void WriteQuotedString(string value)
        {
            _writer.Write('"');
            _writer.Write(EscapeString(value));
            _writer.Write('"');
        }

        private static string EscapeString(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\"", "\\\"");
        }

        /// <summary>
        /// Writes out the type for an object in regular C# code syntax
        /// </summary>
        /// <param name="t">the type to write</param>
        protected abstract void WriteTypeInfo(Type t);

        /// <summary>
        /// Writes out the type info specified by the NamespaceAndClass string.
        /// </summary>
        /// <param name="NamespaceAndClass">the fully-qualified type with namespace and class, but not assembly</param>
        protected virtual void WriteTypeInfo(string NamespaceAndClass)
        {
            _writer.Write(NamespaceAndClass);
        }

        /// <summary>
        /// Writes out the type info specified by the NamespaceAndClass string and assembly.
        /// </summary>
        /// <param name="NamespaceAndClass">the fully-qualified type with namespace and class, but not assembly</param>
        /// <param name="assembly">The assembly for the type</param>
        protected virtual void WriteTypeInfo(string NamespaceAndClass, string assembly)
        {
            string fullTypeName = NamespaceAndClass + ", " + assembly;
            WriteQuotedString(fullTypeName);
        }

        public IJsonWriter Cast(Type castedType)
        {
            if (castedType != typeof(string))
            {
                PreWrite(OpType.OpCast);
                _writer.Write('(');
                WriteTypeInfo(castedType);
                _writer.Write(')');
            }
            return this;
        }

        public IJsonWriter Cast(string NamespaceAndClass)
        {
            PreWrite(OpType.OpCast);
            _writer.Write('(');
            _writer.Write(NamespaceAndClass);
            _writer.Write(')');
            return this;
        }

        public IJsonWriter Cast(string NamespaceAndClass, string Assembly)
        {
            string fullTypeName = NamespaceAndClass + ", " + Assembly;
            PreWrite(OpType.OpCast);
            _writer.Write('(');
            WriteQuotedString(fullTypeName);
            _writer.Write(')');
            return this;
        }

        public abstract IJsonWriter WriteObject(object value);


        public void Dispose()
        {
            _writer.Dispose();
        }

        private interface IState
        {
            JsonWriter Outer { get; set; }

            /// <summary>
            /// Reference to the previous state, this should be
            /// set when a new state is created
            /// </summary>
            IState PreviousState { get; set; }

            /// <summary>
            /// Called before a write operation occurs
            /// </summary>
            /// <param name="operation"></param>
            void PreWrite(OpType operation);

            /// <summary>
            /// Called when control is returned back to a prior state. 
            /// The current state implementing the transition should pass itself
            /// as the "other" state.
            /// </summary>
            /// <param name="otherState">the state that is returning control back to the previous state</param>
            /// <param name="operation">the current operation that is causing control to return</param>
            void ReturnFrom(IState otherState, OpType operation);
        }

        /// <summary>
        /// Base class for states, implements helper functions for transitions
        /// </summary>
        private class StateBase : IState
        {
            protected IState _previousState;
            protected JsonWriter _outer;
            protected bool needComma;

            public StateBase(JsonWriter outer) {
                this._outer = outer;
                needComma = false;
            }

            public StateBase() : this(null) {
            }

            public virtual JsonWriter Outer {
                get { return _outer; }
                set { _outer = value; }
            }

            public virtual IState PreviousState
            {
                get { return _previousState; }
                set { _previousState = value; }
            }

            /// <summary>
            /// Called before write operations to check for
            /// valid states and to transition to new states
            /// </summary>
            /// <param name="operation"></param>
            public virtual void PreWrite(OpType operation)
            {
                switch (operation)
                {
                    case OpType.ArrStart:
                        NewState(new ArrayState());
                        break;
                    case OpType.CtorStart:
                        NewState(new CtorState());
                        break;
                    case OpType.ObjStart:
                        NewState(new ObjectState());
                        break;
                    case OpType.OpCast:
                        Current(operation);
                        needComma = false;
                        break;
                    default:
                        InvalidState(operation);
                        break;
                }
            }


            public virtual void ReturnFrom(IState otherState, OpType operation)
            {

            }



            /// <summary>
            /// The current operation is invalid for the given state, throw an exception
            /// </summary>
            /// <param name="operation">the operation</param>
            protected void InvalidState(OpType operation)
            {
                throw new InvalidOperationException(string.Format("Invalid operation {0} for current state {1}", operation, this.GetType().Name));
            }

            /// <summary>
            /// Return control to the previous state with formatting performed
            /// </summary>
            /// <param name="operation">the current operation</param>
            protected virtual void ReturnToPrevious(OpType operation)
            {
                ReturnToPrevious(operation, true);
            }

            /// <summary>
            /// Return control to the previous state
            /// </summary>
            /// <param name="operation">the current operation</param>
            /// <param name="doFormatting">flag to indicate whether formatting options such as indenting should be performed</param>
            protected virtual void ReturnToPrevious(OpType operation, bool doFormatting) {
                if (doFormatting)
                {
                    Outer.indentLevel--;
                    if (!(this is KeyState) && operation != OpType.OpValue)
                    {
                        Outer.WriteLineBreak();
                        Outer.WriteIndent();
                    }
                }
                Outer._currentState = PreviousState;
                if (PreviousState == null)
                    throw new InvalidOperationException("Attempt to return to previous state when there is no previous state");

                PreviousState.ReturnFrom(this, operation);
            }

            /// <summary>
            /// Transition to a new state, the current state will be set as
            /// the PreviousState property of the newState.  Formatting will be performed
            /// </summary>
            /// <param name="newState">the new state to transition to</param>
            protected virtual void NewState(IState newState)
            {
                NewState(newState, true);
            }

            /// <summary>
            /// Transition to a new state, the current state will be set as
            /// the PreviousState property of the newState.
            /// </summary>
            /// <param name="newState">the new state to transition to</param>
            /// <param name="doFormatting">flag to indicate whether formatting options such as indenting should be performed</param>
            protected virtual void NewState(IState newState, bool doFormatting)
            {
                // this is necessary to function correctly so don't put inside doFormatting check
                if (needComma == true)
                {
                    Outer._writer.Write(", ");
                } 
                if (doFormatting)
                {
                    if (!(this is InitialState) && !(this is KeyState))
                    {
                        Outer.WriteLineBreak();
                        Outer.WriteIndent();
                    }
                    if (!(this is KeyState))
                        Outer.indentLevel++;
                }
                newState.PreviousState = this;
                newState.Outer = Outer;
                Outer._currentState = newState;
                needComma = true;
            }

            /// <summary>
            /// Stay on the current state
            /// </summary>
            /// <param name="operation">the current operation</param>
            protected virtual void Current(OpType operation)
            {
                Current(operation, true);
            }

            /// <summary>
            /// Stay on the current state
            /// </summary>
            /// <param name="operation">the current operation</param>
            /// <param name="doFormatting">flag to indicate whether formatting options such as indenting should be performed</param>
            protected virtual void Current(OpType operation, bool doFormatting)
            {
                if (needComma == true)
                {
                    Outer._writer.Write(", ");
                }
                if (doFormatting)
                {
                    Outer.WriteLineBreak();
                    Outer.WriteIndent();
                }
                needComma = true;
            }
        }

        /// <summary>
        /// The initial state of the writer
        /// </summary>
        private class InitialState : StateBase
        {
            public InitialState(JsonWriter outer) : base(outer) {
            }


            public override void PreWrite(OpType operation)
            {
                switch (operation)
                {
                    case OpType.ArrStart:
                        NewState(new ArrayState());
                        break;
                    case OpType.CtorStart:
                        NewState(new CtorState());
                        break;
                    case OpType.ObjStart:
                        NewState(new ObjectState());
                        break;
                    case OpType.OpCast:
                        // do nothing
                        break;
                    case OpType.OpValue:
                        NewState(new DoneState());
                        break;
                    default:
                        InvalidState(operation);
                        break;
                }
            }

            public override void ReturnFrom(IState otherState, OpType operation)
            {
                // only one expression can be written then we're done
                // so never return to initial state
                Outer._currentState = new DoneState();
                Outer._currentState.Outer = Outer;
            }
        }

        /// <summary>
        /// State when an array is in progress
        /// </summary>
        private class ArrayState : StateBase {

            public override void PreWrite(OpType operation)
            {
                switch (operation)
                {
                    case OpType.ArrStart:
                    case OpType.CtorStart:
                    case OpType.ObjStart:
                    case OpType.OpCast:
                        base.PreWrite(operation);
                        break;
                    case OpType.OpValue:
                        Current(operation);
                        break;
                    case OpType.ArrEnd:
                        ReturnToPrevious(operation);
                        break;
                    default:
                        InvalidState(operation);
                        break;
                }
            }
        }

        /// <summary>
        /// State when a constructor is in progress
        /// </summary>
        private class CtorState : StateBase {
            private enum CtorStateType
            {
                Initial,
                Initializer,
                Done
            }

            private CtorStateType stateType = CtorStateType.Initial;

            public override void PreWrite(OpType operation)            
            {
                switch (stateType)
                {
                    case CtorStateType.Initial:
                        switch (operation)
                        {
                            case OpType.CtorArgStart:
                                NewState(new CtorArgsState(), false);
                                stateType = CtorStateType.Initializer;
                                needComma = false;
                                break;
                            default:
                                InvalidState(operation);
                                break;
                        }
                        break;
                    case CtorStateType.Initializer:
                        switch (operation)
                        {
                            case OpType.CtorEnd:
                                ReturnToPrevious(operation, false);
                                break;
                            case OpType.ObjStart:
                                base.PreWrite(operation);
                                stateType = CtorStateType.Done;
                                break;
                            case OpType.OpCast:
                                base.PreWrite(operation);
                                break;
                            default:
                                InvalidState(operation);
                                break;

                        }
                        break;
                    case CtorStateType.Done:
                        switch (operation)
                        {
                            case OpType.CtorEnd:
                                ReturnToPrevious(operation, false);
                                break;
                            default:
                                InvalidState(operation);
                                break;
                        }
                        break;

                }
            }
        }

        /// <summary>
        /// State when an constructor args are in progress
        /// </summary>
        private class CtorArgsState : StateBase
        {

            public override void PreWrite(OpType operation)
            {
                switch (operation)
                {
                    case OpType.ArrStart:
                        NewState(new ArrayState(), false);
                        break;
                    case OpType.CtorStart:
                        NewState(new CtorState(), false);
                        break;
                    case OpType.ObjStart:
                        NewState(new ObjectState(), false);
                        break;
                    case OpType.OpCast:
                        Current(operation, false);
                        needComma = false;
                        break;
                    case OpType.OpValue:
                        Current(operation, false);
                        break;
                    case OpType.CtorArgEnd:
                        ReturnToPrevious(operation, false);
                        break;
                    default:
                        InvalidState(operation);
                        break;
                }
            }
        }

        /// <summary>
        /// State when a javascript object is in progress
        /// </summary>
        private class ObjectState : StateBase {

            private bool key;
            public ObjectState()
                : base()
            {
                key = true; // look for key
            }
            public override void PreWrite(OpType operation)
            {
                if (key)
                {
                    switch (operation)
                    {
                        case OpType.OpKey:
                            key = false;
                            Current(operation);
                            needComma = false;
                            break;
                        case OpType.ObjEnd:
                            ReturnToPrevious(operation);
                            break;
                        default:
                            InvalidState(operation);
                            break;
                    }
                }
                else
                {
                    switch (operation)
                    {

                        case OpType.OpValue:
                            key = true;
                            needComma = true;
                            break;
                        case OpType.OpCast:
                            // nothing to do
                            break;
                        default:
                            base.PreWrite(operation);
                            // wrote a value, look for a key again
                            key = true;
                            needComma = true;
                            break;
                    }
                }
            }
        }

        private class DoneState : StateBase {
            public override void PreWrite(OpType operation)
            {
                InvalidState(operation);
            }
        }

        private class KeyState : StateBase {
            public override void PreWrite(OpType operation)
            {
                switch (operation)
                {
                    case OpType.OpValue:
                        ReturnToPrevious(operation);
                        break;
                    default:
                        base.PreWrite(operation);
                        break;
                }
            }
            public override void ReturnFrom(IState otherState, OpType operation)
            {
                Outer._currentState = PreviousState;
            }
        }
    }
}
