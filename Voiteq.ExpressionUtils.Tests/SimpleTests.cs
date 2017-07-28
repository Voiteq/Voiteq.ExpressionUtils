using Shouldly;
using Xunit;

namespace Voiteq.ExpressionUtils.Tests
{
    public class SimpleTests
    {
        [Fact]
        public void CanInferEqualityCacheKey()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => x.Name == "Bob").ShouldBe("(Name==\"Bob\")");
        }

        [Fact]
        public void CanInferEqualityCacheKeyWithReversedBinaryExpressionOrder()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => "Bob" == x.Name).ShouldBe("(Name==\"Bob\")");
        }

        [Fact]
        public void CanInferInequalityCacheKey()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => x.Name != "Bob").ShouldBe("(Name!=\"Bob\")");
        }

        [Fact]
        public void CanInferInequalityCacheKeyWithReversedBinaryExpressionOrder()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => "Bob" != x.Name).ShouldBe("(Name!=\"Bob\")");
        }

        [Fact]
        public void CaninferCacheKeyWithAndOperator()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => x.City == "Toronto" && x.Name == "Bob").ShouldBe("((City==\"Toronto\")&&(Name==\"Bob\"))");
        }

        [Fact]
        public void CaninferCacheKeyWithOrOperator()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => x.City == "Toronto" || x.Name == "Bob").ShouldBe("((City==\"Toronto\")||(Name==\"Bob\"))");
        }

        [Fact]
        public void CanInferSameCacheKeyWithOperandsInDifferentOrder()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => x.Name == "Bob" && x.City == "Toronto").ShouldBe("((City==\"Toronto\")&&(Name==\"Bob\"))");
        }

        [Fact]
        public void CaninferCacheKeyWithAndOperatorAndParentheses()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => (x.Name == "Bob") && (x.City == "Toronto")).ShouldBe("((City==\"Toronto\")&&(Name==\"Bob\"))");
        }

        [Fact]
        public void CaninferCacheKeyWithMethodCall()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => x.Name.PadLeft(23) == "Bob").ShouldBe("((Name.PadLeft(23))==\"Bob\")");
        }

        [Fact]
        public void CaninferCacheKeyWithMultipleMethodCallsInWrongOrder()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => x.Name.StartsWith("a") && x.Name.PadLeft(23) == "Bob").ShouldBe("(((Name.PadLeft(23))==\"Bob\")&&(Name.StartsWith(\"a\")))");
        }

        [Fact]
        public void WillNormaliseSortOrderForCommutativeBinaryFunction()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => x.Age + 3 * x.Age < 100).ShouldBe("(((Age*3)+Age)<100)");
        }

        [Fact]
        public void WillNotNormaliseSortOrderForNonCommutativeBinaryFunction()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(x => x.Age - 3 * x.Age < 100).ShouldBe("((Age-(Age*3))<100)");
        }

        [Fact]
        public void WillReplaceBoundMethodCallsWithActualValues()
        {
            var e = new TestClasses.TestCompanyClass { CityId = 23 };
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(dim => dim.City == e.CityId.ToString()).ShouldBe("(City==\"23\")");
        }

        [Fact]
        public void WillReplaceBoundPropertyAccessesWithActualValues()
        {
            var e = new TestClasses.TestCompanyClass { CityId = 23 };
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(dim => dim.Age == e.CityId).ShouldBe("(Age==23)");
        }

        [Fact]
        public void WillNotAttemptToReplaceUnboundMethodCallsWithActualValues()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(dim => dim.City.ToString().Length < 2).ShouldBe("((City.ToString()).Length<2)");
        }

        [Fact]
        public void CanInferKeyWithNullableComparison()
        {
            Stringifier.inferCacheKey<TestClasses.TestUserClass>(dim => dim.Status == 2).ShouldBe("(Convert(2, Int32?)==Status)");
        }

    }
}