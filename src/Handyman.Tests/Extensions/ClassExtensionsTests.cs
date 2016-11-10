﻿﻿using Handyman.Extensions;
 using Shouldly;

namespace Handyman.Tests.Extensions
{
    public class ClassExtensionsTests
    {
        public void ShouldCheckIsNull()
        {
            default(object).IsNull().ShouldBe(true);
            new object().IsNull().ShouldBe(false);
        }

        public void ShouldCheckIsNotNull()
        {
            default(object).IsNotNull().ShouldBe(false);
            new object().IsNotNull().ShouldBe(true);
        }
    }
}