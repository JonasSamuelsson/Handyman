using System;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Docs
{
    public class Enums
    {
        [Flags]
        enum Options
        {
            None = 0,
            First = 1,
            Second = 2,
            Both = First | Second
        }

        [Fact]
        public void Validate()
        {
            var type = typeof(Options?);

            var dataContract = new Enum
            {
                Flags = true,
                Nullable = true,
                Values =
                {
                    { 0, "None" },
                    { 1, "First" },
                    { 2, "Second" },
                    { 3, "Both" }
                }
            };

            new DataContractValidator().Validate(type, dataContract);
        }
    }
}