using System.Collections.Generic;

namespace Handyman.Tools.Ado
{
    public class ResponseContent<T>
    {
        public IEnumerable<T> Value { get; set; }
    }
}