using System;

namespace Handyman.DataContractValidator
{
    public class Value<T> : Value
    {
        public Value()
            : base(typeof(T))
        {
        }
    }

    public abstract class Value
    {
        protected Value(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}