using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sixeyed.Caching.Containers;
using Sixeyed.Caching.Serialization;
using Sixeyed.Caching.Serialization.Serializers.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sixeyed.Caching.Tests
{
    [TestClass]
    public static class Startup
    {
        [AssemblyInitialize()]
        public static void JsonSerializer(TestContext context)
        {
            var thisAssembly = typeof(JsonSerializer).Assembly;
            Container.RegisterAll<ISerializer>(thisAssembly, Lifetime.Singleton);
        }
    }
}
