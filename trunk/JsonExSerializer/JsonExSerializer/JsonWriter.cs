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
        protected enum ModeType
        {
            InitialMode,
            ObjectMode,
            KeyMode,
            ArrayMode,
            ConstructorMode,
            DoneMode,
            /// <summary>
            /// Not really a valid mode, just signals a stack pop
            /// </summary>
            PreviousMode,
            /// <summary>
            /// Signals to stay in the current mode
            /// </summary>
            CurrentMode
        }

        protected enum OpType
        {
            CtorStart,
            CtorEnd,
            ObjStart,
            ObjEnd,
            ArrStart,
            ArrEnd,
            OpKey,
            OpValue,
            OpCast
        }

        protected ModeType currentMode = ModeType.InitialMode;
        protected bool needComma = false;
        protected short indentLevel = 0;
        protected short indentSize = 4;

        protected Stack<ModeType> _modeStack;
        /// <summary>
        /// Stores a combination of modes and operations.  If an
        /// operation is valid for a given mode that will be a key in
        /// the dictionary, the value will be the new mode for the operation.
        /// </summary>
        protected static IDictionary<ModeOp, ModeType> _validOps;

        public JsonWriter(TextWriter writer, bool indent) {
            this._writer = writer;
            _modeStack = new Stack<ModeType>();
            if (!indent)
                indentSize = 0;
        }

        static JsonWriter()
        {
            _validOps = new Dictionary<ModeOp, ModeType>();
            // initial mode
            _validOps[new ModeOp(ModeType.InitialMode, OpType.ArrStart)] = ModeType.ArrayMode;
            _validOps[new ModeOp(ModeType.InitialMode, OpType.CtorStart)] = ModeType.ConstructorMode;
            _validOps[new ModeOp(ModeType.InitialMode, OpType.ObjStart)] = ModeType.ObjectMode;
            _validOps[new ModeOp(ModeType.InitialMode, OpType.OpCast)] = ModeType.CurrentMode;
            _validOps[new ModeOp(ModeType.InitialMode, OpType.OpValue)] = ModeType.DoneMode;

            _validOps[new ModeOp(ModeType.ArrayMode, OpType.ArrStart)] = ModeType.ArrayMode;
            _validOps[new ModeOp(ModeType.ArrayMode, OpType.CtorStart)] = ModeType.ConstructorMode;
            _validOps[new ModeOp(ModeType.ArrayMode, OpType.ObjStart)] = ModeType.ObjectMode;
            _validOps[new ModeOp(ModeType.ArrayMode, OpType.OpCast)] = ModeType.CurrentMode;
            _validOps[new ModeOp(ModeType.ArrayMode, OpType.ArrEnd)] = ModeType.PreviousMode;
            _validOps[new ModeOp(ModeType.ArrayMode, OpType.OpValue)] = ModeType.CurrentMode;

            _validOps[new ModeOp(ModeType.ObjectMode, OpType.ObjEnd)] = ModeType.PreviousMode;
            _validOps[new ModeOp(ModeType.ObjectMode, OpType.OpKey)] = ModeType.KeyMode;

            _validOps[new ModeOp(ModeType.KeyMode, OpType.OpValue)] = ModeType.PreviousMode;
            _validOps[new ModeOp(ModeType.KeyMode, OpType.ArrStart)] = ModeType.ArrayMode;
            _validOps[new ModeOp(ModeType.KeyMode, OpType.CtorStart)] = ModeType.ConstructorMode;
            _validOps[new ModeOp(ModeType.KeyMode, OpType.ObjStart)] = ModeType.ObjectMode;
            _validOps[new ModeOp(ModeType.KeyMode, OpType.OpCast)] = ModeType.CurrentMode;


        }
        private void PreWrite(OpType operation) {
            try
            {
                ModeType newMode = _validOps[new ModeOp(currentMode, operation)];
                switch (newMode)
                {
                    case  ModeType.PreviousMode:
                        indentLevel--;
                        if (currentMode != ModeType.KeyMode && operation != OpType.OpValue)
                        {
                            WriteLineBreak();
                            WriteIndent();
                        }
                        currentMode = _modeStack.Pop();
                        // keys need an extra pop when coming back from array or object modes
                        if (currentMode == ModeType.KeyMode)
                            currentMode = _modeStack.Pop();

                        // never return to initial mode, after the first expression we're done
                        if (currentMode == ModeType.InitialMode)
                            currentMode = ModeType.DoneMode;

                        needComma = true;
                        break;
                    case ModeType.CurrentMode:
                        if (needComma == true)
                        {
                            _writer.Write(", ");
                        }
                        WriteLineBreak();
                        WriteIndent();
                        needComma = true;
                        break;
                    default:
                        if (needComma == true)
                        {
                            _writer.Write(", ");                            
                        }
                        if (currentMode != ModeType.InitialMode && currentMode != ModeType.KeyMode)
                        {
                            WriteLineBreak();
                            WriteIndent();
                        }
                        if (currentMode != ModeType.KeyMode)
                            indentLevel++;

                        needComma = false;
                        _modeStack.Push(currentMode);
                        
                        currentMode = newMode;
                        break;
                }
                
            }
            catch (KeyNotFoundException e) {
                throw new InvalidOperationException(string.Format("Invalid operation {0} for current state {1}", operation, currentMode));
            }
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
        public IJsonWriter ConstructorStart(Type constructorType)
        {
            PreWrite(OpType.CtorStart);
            throw new Exception("The method or operation is not implemented.");
        }

        public IJsonWriter ConstructorEnd()
        {
            PreWrite(OpType.CtorEnd);
            throw new Exception("The method or operation is not implemented.");
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
            needComma = false;
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

        public IJsonWriter Cast(Type castedType)
        {
            if (castedType != typeof(string))
            {
                PreWrite(OpType.OpCast);
                _writer.Write('(');
                //TODO: Write simple type name for primitive types 
                // such as "int" instead of "System.Int32"
                WriteTypeInfo(castedType);
                _writer.Write(')');
            }
            return this;
        }

        public abstract IJsonWriter WriteObject(object value);


        public void Dispose()
        {
            _writer.Dispose();
        }

        /// <summary>
        /// struct to hold mode operation pair to store in the
        /// dictionary for valid operations for a given state
        /// </summary>
        protected struct ModeOp
        {
            public ModeType Mode;
            public OpType Operation;
            public ModeOp(ModeType Mode, OpType Operation)
            {
                this.Mode = Mode;
                this.Operation = Operation;
            }

            public override bool Equals(object obj)
            {
                if (obj is ModeOp)
                {
                    ModeOp other = (ModeOp)obj;
                    return other.Operation == this.Operation
                        && other.Mode == this.Mode;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (((int)Mode) << 5) | ((int)Operation);
            }
        }
    }
}
