using System;
using System.Linq;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestFilterToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Lazy<RequestFilterToggleMetadata> _metadata;
        private readonly Type[] _toggleEnabledFilterTypes;

        public RequestFilterToggleAttribute(Type[] toggleEnabledFilterTypes)
        {
            if (toggleEnabledFilterTypes == null)
                throw new ArgumentNullException(nameof(toggleEnabledFilterTypes));

            if (!toggleEnabledFilterTypes.Any())
                throw new ArgumentException();

            _metadata = new Lazy<RequestFilterToggleMetadata>(CreateMetadata);
            _toggleEnabledFilterTypes = toggleEnabledFilterTypes;
        }

        public string Name { get; set; }
        public string[] Tags { get; set; }
        public Type[] ToggleDisabledFilterTypes { get; set; }

        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            builder.AddFilterSelector(new RequestFilterToggleFilterSelector(_metadata.Value));
        }

        private RequestFilterToggleMetadata CreateMetadata()
        {
            return new RequestFilterToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledFilterTypes = ToggleDisabledFilterTypes,
                ToggleEnabledFilterTypes = _toggleEnabledFilterTypes
            };
        }
    }
}