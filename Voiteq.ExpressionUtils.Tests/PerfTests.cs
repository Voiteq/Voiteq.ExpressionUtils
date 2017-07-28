using System;
using Shouldly;
using Xunit;

namespace Voiteq.ExpressionUtils.Tests
{
    public class PerfTests
    {
        [Fact]
        public void UnboundCallShouldBeQuick()
        {
            ((Action)(() => Stringifier.inferCacheKey<TestClasses.TestContainerClass>(x => x.TestUserClass.Name == "Bob"))).RunRepeatedly(1000000).TimeTaken.ShouldBeLessThan(10000);
        }

        [Fact]
        public void BoundCallShouldBeQuick()
        {
            var a = 23;
            ((Action)(() => Stringifier.inferCacheKey<TestClasses.TestContainerClass>(x => x.TestUserClass.Name == a.ToString()))).RunRepeatedly(10000).TimeTaken.ShouldBeLessThan(10000);
        }
    }
}