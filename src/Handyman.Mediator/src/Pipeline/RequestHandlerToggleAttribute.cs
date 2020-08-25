using System;
using System.Linq;

namespace Handyman.Mediator.Pipeline
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestHandlerToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Lazy<RequestHandlerToggleMetadata> _metadata;
        private readonly Type[] _toggleEnabledHandlerTypes;

        public RequestHandlerToggleAttribute(Type toggleEnabledHandlerType)
            : this(new[] { toggleEnabledHandlerType ?? throw new ArgumentNullException(nameof(toggleEnabledHandlerType)) })
        {
        }

        public RequestHandlerToggleAttribute(Type[] toggleEnabledHandlerTypes)
        {
            if (toggleEnabledHandlerTypes == null)
                throw new ArgumentNullException(nameof(toggleEnabledHandlerTypes));

            if (!toggleEnabledHandlerTypes.Any())
                throw new ArgumentException();

            _metadata = new Lazy<RequestHandlerToggleMetadata>(CreateMetadata);
            _toggleEnabledHandlerTypes = toggleEnabledHandlerTypes;
        }

        public string Name { get; set; }
        public string[] Tags { get; set; }
        public Type[] ToggleDisabledHandlerTypes { get; set; }

        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            builder.AddHandlerSelector(new RequestHandlerToggleHandlerSelector(_metadata.Value));
        }

        private RequestHandlerToggleMetadata CreateMetadata()
        {
            return new RequestHandlerToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledHandlerTypes = ToggleDisabledHandlerTypes,
                ToggleEnabledHandlerTypes = _toggleEnabledHandlerTypes
            };
        }
    }
}