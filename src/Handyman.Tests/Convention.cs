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
    }
}