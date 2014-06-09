using System.Linq;
using System;
using Sixeyed.Caching.Serialization.Serializers;

namespace Sixeyed.Caching.Serialization
{
    /// <summary>
    /// Wrapper for accessing <see cref="ISerializer"/> implementations
    /// </summary>
    public class Serializer
    {
        private static Serializer _default = new Serializer();

        public static Serializer Default { get { return _default; } }

        public ISerializer Get(SerializationFormat format)
        {
            switch (format)
            {
                case SerializationFormat.None:
                    return this.Null;
                case SerializationFormat.Json:
                    return this.Json;
                case SerializationFormat.Xml:
                    return this.Xml;
                case SerializationFormat.Binary:
                    return this.Binary;
                case SerializationFormat.Null:
                    throw new Exception("You must specify a valid SerializationFormat");
                default:
                    throw new Exception(string.Format("There is no serializer with format '{0}'", format));
            }
        }

        private static ISerializer _null = new NullSerializer();

        public ISerializer Null
        {
            get { return _null; }
            set { _null = value; }
        }

        private static IJsonSerializer _json = null;

        public IJsonSerializer Json
        {
            get 
            {
                if (_json == null)
                    throw new Exception("Json Serializer is not configured");
                return _json;
            }
            set { _json = value; }
        }

        private static ISerializer _xml = new XmlSerializer();

        public ISerializer Xml
        {
            get { return _xml; }
            set { _xml = value; }
        }

        private static ISerializer _binary = new BinarySerializer();

        public ISerializer Binary
        {
            get { return _binary; }
            set { _binary = value; }
        }
    }
}
