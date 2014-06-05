namespace Handyman.Tests
{
    public class Convention : Fixie.Conventions.Convention
    {
        public Convention()
        {
            Classes.NameEndsWith("Tests");
            Methods.Where(x => x.IsPublic);
            CaseExecution.SetUp((execution, instance) => Configuration.Reset());
        }
    }
}