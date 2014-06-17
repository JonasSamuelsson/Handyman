using System.Linq;

namespace Handyman.Tests
{
    public class Convention : Fixie.Conventions.Convention
    {
        public Convention()
        {
            Classes.NameEndsWith("Tests");
            Methods.Where(x => x.IsPublic);
            CaseExecution.SetUp((execution, instance) =>
            {
                Configuration.Reset();
                var cultureAttribute = execution.Case.Method.Get<CultureAttribute>().SingleOrDefault();
                if (cultureAttribute == null) return;
                var cultureInfo = cultureAttribute.GetCulture();
                Configuration.FormatProvider = () => cultureInfo;
            });
        }
    }
}