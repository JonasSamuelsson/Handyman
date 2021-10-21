using System.Collections.Generic;

namespace Handyman.DataContractValidator
{
    public class Enum
    {
        public bool Flags { get; set; }
        public bool Nullable { get; set; }
        public Dictionary<long, string> Values { get; set; } = new Dictionary<long, string>();

    }
}