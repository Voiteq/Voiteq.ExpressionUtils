using System.Collections.Generic;

namespace Voiteq.ExpressionUtils.Tests
{
    public class LookupCacheTests
    {
        public class TestUserClass
        {
            public int Key { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public int Age { get; set; }
        }

        public class TestCompanyClass
        {
            public int Key { get; set; }
            public string Name { get; set; }
            public int CityId { get; set; }
        }

        private static IList<TestUserClass> CreateTestData()
        {
            return new List<TestUserClass>
            {
                new TestUserClass {Key = 12, Name = "Sara"},
                new TestUserClass {Key = 23, Name = "Bill"},
                new TestUserClass {Key = 5, Name = "Joe"}
            };
        }

    }
}