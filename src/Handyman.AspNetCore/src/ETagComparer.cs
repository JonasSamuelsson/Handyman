namespace Handyman.AspNetCore
{
    public class ETagComparer
    {
        public static readonly ETagComparer Instance = new ETagComparer();

        private ETagComparer() { }

        public bool Equals(string eTag1, string eTag2)
        {
            if (eTag1 == "*" || eTag2 == "*")
                return true;

            if (string.IsNullOrWhiteSpace(eTag1) || string.IsNullOrWhiteSpace(eTag2))
                return false;

            return eTag1 == eTag2;
        }

        public bool EqualsSqlServerRowVersion(string eTag, byte[] rowVersion)
        {
            if (eTag == "*")
                return true;

            if (string.IsNullOrWhiteSpace(eTag))
                return false;

            return eTag == ETagConverter.FromSqlServerRowVersion(rowVersion);
        }
    }
}