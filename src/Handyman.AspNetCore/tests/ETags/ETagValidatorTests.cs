﻿using Handyman.AspNetCore.ETags;
using Shouldly;
using Xunit;

namespace Handyman.AspNetCore.Tests
{
    public class ETagValidatorTests
    {
        [Theory]
        [InlineData("\"x\"")]
        [InlineData("W/\"x\"")]
        [InlineData("W/\"iuwyehf72tw45ii7yhw734ydh287rygw87ryug8wd7yhr8w37yr8w37jry8\"")]
        public void ShouldAcceptValidETags(string eTag)
        {
            new ETagValidator().IsValidETag(eTag).ShouldBeTrue();
        }

        [Theory]
        [InlineData("x")]
        [InlineData("x\"")]
        [InlineData("\"x")]
        [InlineData("x\"x\"")]
        [InlineData("\"x\"x")]
        [InlineData("W/\"x")]
        [InlineData("xW/\"x\"")]
        [InlineData("W/\"x\"x")]
        [InlineData("w/\"x\"")]
        public void ShouldDenyInvalidETags(string eTag)
        {
            new ETagValidator().IsValidETag(eTag).ShouldBeFalse();
        }
    }
}