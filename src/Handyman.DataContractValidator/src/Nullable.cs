namespace Handyman.DataContractValidator
{
    public class Nullable
    {
        public static readonly Nullable String = new Nullable(typeof(string));

        public Nullable(object item)
        {
            Item = item;
        }

        public object Item { get; }
    }
}