using System;
using System.Linq;
using System.Reflection;
using Fixie;
using Handyman.Extensions;

namespace Handyman.Tests
{
    public class CustomCultureBehavior : CaseBehavior
    {
        public void Execute(Case context, Action next)
        {
            try
            {
                TrySetCultureFromAttribute(context.Method);
                next();
            }
            finally
            {
                ResetCulture();
            }
        }

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