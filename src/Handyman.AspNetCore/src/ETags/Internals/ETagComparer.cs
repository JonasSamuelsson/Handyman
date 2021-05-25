namespace Handyman.AspNetCore.ETags.Internals
{
    internal class ETagComparer : IETagComparer
    {
        private readonly IETagConverter _converter;

        public ETagComparer(IETagConverter converter)
        {
            _converter = converter;
        }

        public void EnsureEquals(string eTag, byte[] bytes)
        {
            if (Equals(eTag, bytes))
                return;

            throw new PreconditionFailedException();
        }

        public void EnsureEquals(string eTag1, string eTag2)
        {
            if (Equals(eTag1, eTag2))
                return;

            throw new PreconditionFailedException();
        }

        public bool Equals(string eTag, byte[] bytes)
        {
            if (eTag == "*")
                return true;

            if (string.IsNullOrWhiteSpace(eTag))
                return false;

            return eTag == _converter.FromByteArray(bytes);
        }

        public bool Equals(string eTag1, string eTag2)
        {
            if (eTag1 == "*" || eTag2 == "*")
                return true;

            if (string.IsNullOrWhiteSpace(eTag1) || string.IsNullOrWhiteSpace(eTag2))
                return false;

            return eTag1 == eTag2;
        }
    }
}