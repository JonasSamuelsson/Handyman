using System.Linq;
using System.Reflection;
using Handyman.Extensions;

namespace Handyman.Tests
{
    public static class CustomCultureBehavior
    {
        public static void TrySetCultureFromAttribute(MethodInfo method)
        {
            var cultureAttribute = method.Get<CultureAttribute>().SingleOrDefault();
            if (cultureAttribute != null)
            {
                var cultureInfo = cultureAttribute.GetCulture();
                Configuration.FormatProvider = () => cultureInfo;
            }
        }

        public static void ResetCulture()
        {
            Configuration.Reset();
        }
    }
}