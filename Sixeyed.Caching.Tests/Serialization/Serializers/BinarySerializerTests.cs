using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sixeyed.Caching.Extensions;
using System.Threading;
using Sixeyed.Caching;
using Sixeyed.Caching.Tests.Stubs;
using Sixeyed.Caching.Serialization;

namespace Sixeyed.Caching.Tests.Serialization
{
    [TestClass]
    public class BinarySerializerTests
    {
        [TestMethod]
        public void SerializeAndDeserialize()
        {
            var obj = StubRequestWithEnum.GetRequest();
            var serialized = Serializer.Default.Binary.Serialize(obj);
            var deserialized = Serializer.Default.Binary.Deserialize<StubRequestWithEnum>(serialized);
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(obj.Id, deserialized.Id);
            Assert.AreEqual(obj.Name, deserialized.Name);
            Assert.AreEqual(obj.CreatedOn, deserialized.CreatedOn);
            Assert.AreEqual(obj.Status, deserialized.Status);
        }
    }
}
