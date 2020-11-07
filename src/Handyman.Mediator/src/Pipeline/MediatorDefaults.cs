namespace Handyman.Mediator.Pipeline
{
    public static class MediatorDefaults
    {
        public static readonly IEventHandlerExecutionStrategy EventHandlerExecutionStrategy = new WhenAllEventHandlerExecutionStrategy();
        public static readonly IRequestHandlerExecutionStrategy RequestHandlerExecutionStrategy = new DefaultRequestHandlerExecutionStrategy();

        public static class Order
        {
            public const int First = int.MinValue;
            public const int Default = 0;
            public const int Last = int.MaxValue;
        }
    }
}