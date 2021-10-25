namespace Handyman.DataContractValidator
{
    public class CanBeNull
    {
        public static readonly CanBeNull String = new CanBeNull(typeof(string));

        public CanBeNull(object item)
        {
            Item = item;
        }

        public object Item { get; }
    }
}