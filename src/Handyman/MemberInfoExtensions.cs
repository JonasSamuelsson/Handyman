using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman
{
    public static class MemberInfoExtensions
    {
        public static IEnumerable<TAttribute> Get<TAttribute>(this MemberInfo member, bool inherits = false) where TAttribute : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttribute), inherits).Cast<TAttribute>();
        }

        public static bool Has<TAttribute>(this MemberInfo member, bool inherits = false) where TAttribute : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttribute), inherits).Any();
        }
    }
}