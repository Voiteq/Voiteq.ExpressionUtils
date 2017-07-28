namespace Voiteq.ExpressionUtils.Tests
{
    public class TestClasses
    {
        public class TestUserClass
        {
            public int Key { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public int Age { get; set; }
            public int? Status { get; set; }
        }

        public class TestCompanyClass
        {
            public int Key { get; set; }
            public string Name { get; set; }
            public int CityId { get; set; }
        }

        public class TestContainerClass
        {
            public int Key { get; set; }
            public TestUserClass TestUserClass { get; set; }
        }
    }
}