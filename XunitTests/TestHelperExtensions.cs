using System.Collections;
using System.Collections.Generic;
using FluentAssertions;

namespace XunitTests
{
    public static class TestHelperExtensions
    {
        public static bool AssertIsEquivalent<T>(this IEnumerable<T> collection, IEnumerable<T> collection2)
        {
            collection.Should().BeEquivalentTo(collection2);
            return true;
        }
    }
}