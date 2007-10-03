using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;
using System.Reflection;
using JsonExSerializer.TypeConversion;

namespace JsonExSerializer
{
    public class XmlConfigurator
    {
        //private XmlNode configXml;
        private SerializationContext context;
        private XmlReader reader;
        private delegate void MapHandler();
        private Dictionary<string, MapHandler> handlers = new Dictionary<string, MapHandler>();
        private string sectionName;

        private XmlConfigurator(XmlReader reader, SerializationContext context, string sectionName)
        {
            this.reader = reader;
            this.context = context;
            this.sectionName = sectionName;

            handlers["IsCompact"] = delegate() { context.IsCompact = reader.ReadElementContentAsBoolean(); };
            handlers["OutputTypeComment"] = delegate() { context.OutputTypeComment = reader.ReadElementContentAsBoolean(); };
            handlers["OutputTypeInformation"] = delegate() { context.OutputTypeInformation = reader.ReadElementContentAsBoolean(); };
            handlers["ReferenceWritingType"] = new MapHandler(HandleReferenceWritingType);
            handlers["TypeBindings"] = new MapHandler(HandleTypeBindings);
            handlers["TypeConverters"] = new MapHandler(HandleTypeConverters);
            handlers["TypeConverterFactories"] = new MapHandler(HandleTypeConverterFactories);
        }

        public static void Configure(SerializationContext context, string configSection)
        {
            XmlConfigSection section = (XmlConfigSection)ConfigurationManager.GetSection(configSection);
            if (section == null && configSection != "JsonExSerializer")
                throw new Exception("Unable to find config section " + configSection);
            if (section == null)
                return;

            string xml = section.RawXml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            Configure(context, XmlReader.Create(new StringReader(xml)), configSection);
        }

        public static void Configure(SerializationContext context, XmlReader reader, string sectionName)
        {
            new XmlConfigurator(reader, context, sectionName).Configure();
        }

        private void Configure()
        {
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (handlers.ContainsKey(reader.Name))
                            handlers[reader.Name]();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error configuring serializer from config section " + sectionName + ". " + e.Message, e);
            }
        }

        /// <summary>
        /// Handles the "TypeBindings" tag of the config
        /// </summary>
        private void HandleTypeBindings()
        {
            AddRemoveClearHandler handler = new AddRemoveClearHandler(reader, "alias type", "alias type", "", "TypeBindings");
            foreach(ARCRec record in handler.GetTags()) {
                switch (record.opName) {
                    case "add":
                        if (string.IsNullOrEmpty(record.values["alias"]))
                            throw new Exception("Must specify 'alias' for TypeBinding add");
                        if (string.IsNullOrEmpty(record.values["type"]))
                            throw new Exception("Must specify 'type' for TypeBinding add");

                        context.AddTypeBinding(Type.GetType(record.values["type"]), record.values["alias"]);
                        break;
                    case "remove":
                        if (!string.IsNullOrEmpty(record.values["type"]))
                            context.RemoveTypeBinding(Type.GetType(record.values["type"]));
                        else if (!string.IsNullOrEmpty(record.values["alias"]))
                            context.RemoveTypeBinding(record.values["alias"]);
                        else
                            throw new Exception("Must specify either alias or type argument to remove TypeBinding");
                        break;
                    case "clear":
                        context.ClearTypeBindings();
                        break;
                    default:
                        throw new Exception("Unrecognized element " + record.opName + " within TypeBinding tag");
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the configuration of Type Converters for the TypeConverters node
        /// </summary>
        private void HandleTypeConverters()
        {
            // only supports add for now
            AddRemoveClearHandler handler = new AddRemoveClearHandler(reader, "type property converter", null, null, "TypeConverters");
            foreach (ARCRec record in handler.GetTags())
            {
                switch (record.opName)
                {
                    case "add":
                        if (string.IsNullOrEmpty(record.values["type"]))
                            throw new Exception("Must specify 'type' for TypeConverters add");
                        if (string.IsNullOrEmpty(record.values["converter"]))
                            throw new Exception("Must specify 'converter' for TypeConverters add");

                        // load the specified types
                        Type objectType = Type.GetType(record.values["type"]);
                        Type converterType = Type.GetType(record.values["converter"]);

                        PropertyInfo property = null;
                        // check for the property element, if it exists, the converter is for a property on the type
                        if (!string.IsNullOrEmpty(record.values["property"]))
                        {
                            property = objectType.GetProperty(record.values["property"]);
                        }
                        IJsonTypeConverter converter = (IJsonTypeConverter) Activator.CreateInstance(converterType);
                        if (property != null)
                            context.RegisterTypeConverter(property, converter);
                        else
                            context.RegisterTypeConverter(objectType, converter);

                        break;
                    default:
                        throw new Exception("Unrecognized element " + record.opName + " within TypeConverters tag");
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the configuration of Type Converters factories for the TypeConverterFactories node
        /// </summary>
        private void HandleTypeConverterFactories()
        {
            // only supports add for now
            AddRemoveClearHandler handler = new AddRemoveClearHandler(reader, "type", null, null, "TypeConverterFactories");
            foreach (ARCRec record in handler.GetTags())
            {
                switch (record.opName)
                {
                    case "add":
                        if (string.IsNullOrEmpty(record.values["type"]))
                            throw new Exception("Must specify 'type' for TypeConverterFactories add");

                        // load the specified types
                        Type factoryType = Type.GetType(record.values["type"]);

                        ITypeConverterFactory converterFactory = (ITypeConverterFactory)Activator.CreateInstance(factoryType);
                        context.AddTypeConverterFactory(converterFactory);

                        break;
                    default:
                        throw new Exception("Unrecognized element " + record.opName + " within TypeConverterFactories tag");
                        break;
                }
            }
        }

        private void HandleReferenceWritingType()
        {
            string value = reader.ReadElementContentAsString();
            context.ReferenceWritingType = (SerializationContext.ReferenceOption) Enum.Parse(context.ReferenceWritingType.GetType(), value);
        }

        #region Add, Remove, Clear Handler
        private struct ARCRec
        {
            public string opName;   // add, remove, or clear
            public NameValueCollection values;

            public ARCRec(string opName, NameValueCollection values)
            {
                this.opName = opName;
                this.values = values;
            }
        }

        private class AddRemoveClearHandler
        {
            private XmlReader reader;
            private Dictionary<string, NameValueCollection> tagMap = new Dictionary<string, NameValueCollection>();
            private string outerTag;

            public AddRemoveClearHandler(XmlReader reader, string addAttributes, string removeAttributes, string clearAttributes, string outerTag)
            {
                if (addAttributes != null)
                {
                    this.tagMap.Add("add", new NameValueCollection());
                    foreach (string s in addAttributes.Split(' '))
                        this.tagMap["add"].Add(s, s);
                }

                if (removeAttributes != null)
                {
                    this.tagMap.Add("remove", new NameValueCollection());
                    foreach (string s in removeAttributes.Split(' '))
                        this.tagMap["remove"].Add(s, s);
                }

                if (clearAttributes != null)
                {
                    this.tagMap.Add("clear", new NameValueCollection());
                    foreach (string s in clearAttributes.Split(' '))
                        this.tagMap["clear"].Add(s, s);
                }

                this.outerTag = outerTag;
                this.reader = reader;
            }

            public IEnumerable<ARCRec> GetTags()
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "outerTag")
                        yield break;
                    if (reader.IsStartElement())
                    {
                        if (!tagMap.ContainsKey(reader.Name))
                            throw new Exception("Unrecognized element " + reader.Name + " within " + outerTag + " tag");

                        string tag = reader.Name;
                        NameValueCollection values = new NameValueCollection();
                        NameValueCollection validAttributes = tagMap[reader.Name];

                        while (reader.MoveToNextAttribute())
                        {
                            if (validAttributes.Get(reader.Name) == null)
                                throw new Exception("Unrecognized attribute " + reader.Name + " to " + tag + " tag within " + outerTag + " tag");

                            values[reader.Name] = reader.ReadContentAsString();
                        }
                        yield return new ARCRec(tag, values);
                    }
                }
            }
        }
        #endregion

    }

    public class XmlConfigSection : ConfigurationSection
    {
        private string xml;

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            xml = reader.ReadOuterXml();
        }

        
        public string RawXml
        {
            get { return xml; }
        }
    }
}
