using System.Linq;
using Sixeyed.Caching.Containers;
using System;

namespace Sixeyed.Caching.Serialization
{
    /// <summary>
    /// Wrapper for accessing <see cref="ISerializer"/> implementations
    /// </summary>
    public static class Serializer
    {
        public static ISerializer GetCurrent(SerializationFormat format)
        {            
            var serializers = Container.GetAll<ISerializer>();
            var serializer = serializers.FirstOrDefault(s => s.Format == format);
            if(serializer == null)
            {
                throw new Exception(string.Format("No {0} serializer has been registered", format));
            }
            return serializer;
        }

        public static ISerializer Json     
        {
            get { return GetCurrent(SerializationFormat.Json); }
        }

        public static ISerializer Xml
        {
            get { return GetCurrent(SerializationFormat.Xml); }
        }

        public static ISerializer Binary
        {
            get { return GetCurrent(SerializationFormat.Binary); }
        }
    }
}
