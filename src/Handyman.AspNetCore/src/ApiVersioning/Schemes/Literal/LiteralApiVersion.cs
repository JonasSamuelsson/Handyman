namespace Handyman.AspNetCore.ApiVersioning.Schemes.Literal
{
    internal class LiteralApiVersion : IApiVersion
    {
        public LiteralApiVersion(string text)
        {
            Text = text;
        }

        public string Text { get; }

        public bool IsMatch(IApiVersion other)
        {
            return other is LiteralApiVersion apiVersion && apiVersion.Text == Text;
        }
    }
}