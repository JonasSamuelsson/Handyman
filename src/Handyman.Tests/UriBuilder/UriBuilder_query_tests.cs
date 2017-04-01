using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Handyman.Tests.UriBuilder
{
   public class UriBuilder_query_tests
   {
      [Fact]
      public void should_only_use_the_last_call_to_query_if_multiple_calls_are_made()
      {
         new Handyman.UriBuilder()
            .Query("key1=value1")
            .Query("key2=value2")
            .ToString()
            .ShouldBe("?key2=value2");
      }

      [Fact]
      public void param_with_null_value_should_not_be_inluded_in_query()
      {
         new Handyman.UriBuilder()
            .QueryParams("key", default(int?))
            .ToString()
            .ShouldBe("");

         new Handyman.UriBuilder()
            .QueryParams("key", (IEnumerable<int>)null)
            .ToString()
            .ShouldBe("");
      }

      [Fact]
      public void enums_should_be_represented_by_their_numeric_value()
      {
         new Handyman.UriBuilder()
            .QueryParams("number", Number.Zero)
            .ToString()
            .ShouldBe("?number=0");

         new Handyman.UriBuilder()
            .QueryParams("numbers", Numbers.One | Numbers.Two)
            .ToString()
            .ShouldBe("?numbers=3");
      }

      [Fact]
      public void key_only()
      {
         new Handyman.UriBuilder()
            .QueryParams("key")
            .ToString()
            .ShouldBe("?key=");
      }

      [Fact]
      public void key_value()
      {
         new Handyman.UriBuilder()
            .QueryParams("key", "value")
            .ToString()
            .ShouldBe("?key=value");
      }

      [Fact]
      public void key_values()
      {
         new Handyman.UriBuilder()
            .QueryParams("key", new[] { "value1", "value2" })
            .ToString()
            .ShouldBe("?key=value1&key=value2");
      }

      [Fact]
      public void key_collection()
      {
         var stringValues = new[] { "value1", "value2" }.AsEnumerable();
         new Handyman.UriBuilder()
            .QueryParams("key", stringValues)
            .ToString()
            .ShouldBe("?key=value1&key=value2");

         var intValues = new List<int> { 1, 2, 3 };
         new Handyman.UriBuilder()
            .QueryParams("ints", intValues)
            .ToString()
            .ShouldBe("?ints=1&ints=2&ints=3");

         var enumValues = new[] { Number.Zero, Number.One };
         new Handyman.UriBuilder()
            .QueryParams("number", enumValues)
            .ToString()
            .ShouldBe("?number=0&number=1");
      }

      enum Number
      {
         Zero = 0,
         One = 1
      }

      [Flags]
      enum Numbers
      {
         One = 1,
         Two = 2
      }
   }
}