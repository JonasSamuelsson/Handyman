using System.Globalization;
using System.Reflection;
using Xunit.Sdk;

namespace Handyman.Tests
{
    public class CultureAttribute : BeforeAfterTestAttribute
    {
        private readonly string _name;

        public CultureAttribute(string name)
        {
            _name = name;
        }

        public CultureInfo GetCulture()
        {
            return CultureInfo.GetCultureInfo(_name);
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            base.Before(methodUnderTest);
            CustomCultureBehavior.TrySetCultureFromAttribute(methodUnderTest);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            CustomCultureBehavior.ResetCulture();
            base.After(methodUnderTest);
        }
    }
}