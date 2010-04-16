using System;
using System.Collections.Generic;
using System.Text;
using JsonExSerializer;

namespace JsonExSerializerTests.Mocks
{
    public class MockCallbackObject : ISerializationCallback, IDeserializationCallback
    {
        private string _name;
        private int _beforeSerializeCount = 0;
        private int _afterSerializeCount = 0;
        private int _afterDeserializeCount = 0;

        #region ISerializationCallback Members

        public void OnBeforeSerialization()
        {
            _beforeSerializeCount++;
        }

        public void OnAfterSerialization()
        {
            _afterSerializeCount++;
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnAfterDeserialization()
        {
            if (_name == null)
            {
                throw new ApplicationException("OnAfterDeserialization called too early");
            }
            _afterDeserializeCount++;
        }


        #endregion

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public int BeforeSerializeCount
        {
            get { return this._beforeSerializeCount; }
        }

        public int AfterSerializeCount
        {
            get { return this._afterSerializeCount; }
        }

        public int AfterDeserializeCount
        {
            get { return this._afterDeserializeCount; }
        }
    }
}
