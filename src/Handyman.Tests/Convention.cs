using System;
using System.Linq;
using Fixie;

namespace Handyman.Tests
{
    public class Convention : Fixie.Convention
    {
        public Convention()
        {
            Classes.NameEndsWith("Tests");
            Methods.Where(x => x.IsPublic);
            CaseExecution.Wrap<CustomCultureBehavior>();
        }

        public class CustomCultureBehavior : CaseBehavior
        {
            public void Execute(Case context, Action next)
            {
                try
                {
                    Configuration.Reset();
                    var cultureAttribute = context.Method.Get<CultureAttribute>().SingleOrDefault();
                    if (cultureAttribute != null)
                    {
                        var cultureInfo = cultureAttribute.GetCulture();
                        Configuration.FormatProvider = () => cultureInfo;
                    }

                    next();
                }
                catch
                {
                    Configuration.Reset();
                    throw;
                }
            }
        }
    }
}