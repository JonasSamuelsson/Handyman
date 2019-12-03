namespace Handyman.Mediator
{
    public struct Void
    {
        public static readonly Void Instance = new Void();

        public bool Equals(Void other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Void other && Equals(other);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(Void left, Void right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Void left, Void right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return "()";
        }
    }
}