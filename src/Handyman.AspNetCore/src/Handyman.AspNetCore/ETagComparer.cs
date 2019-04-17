namespace Handyman.AspNetCore
{
    public static class ETagComparer
    {
        public static bool Equals(string eTag1, string eTag2)
        {
            if (eTag1 == "*" || eTag2 == "*")
                return true;

            if (string.IsNullOrWhiteSpace(eTag1) || string.IsNullOrWhiteSpace(eTag2))
                return false;

            return eTag1 == eTag2;
        }

        public static bool EqualsSqlServerRowVersion(string eTag, byte[] rowVersion)
        {
            if (eTag == "*")
                return true;

            if (string.IsNullOrWhiteSpace(eTag))
                return false;

            return eTag == ETagConverter.FromSqlServerRowVersion(rowVersion);
        }
    }
}