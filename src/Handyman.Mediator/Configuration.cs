using System;

namespace Handyman.Mediator
{
    public class Configuration
    {
        /// <summary>
        /// Set if request pipeline should be used, default is true.
        /// </summary>
        public bool RequestPipelineEnabled { get; set; } = true;

        /// <summary>
        /// Service provider
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }
    }
}