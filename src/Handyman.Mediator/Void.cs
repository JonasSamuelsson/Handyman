namespace Handyman.Mediator
{
    public struct Void
    {
        public static readonly Void Instance = new Void();

        public override bool Equals(object obj)
        {
            return obj is Void;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "()";
        }
    }
}