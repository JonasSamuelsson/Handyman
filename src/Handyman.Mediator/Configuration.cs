using System;

namespace Handyman.Mediator
{
    public class Configuration
    {
        /// <summary>
        /// Set if request pipeline should be used, default is false.
        /// </summary>
        public bool RequestPipelineEnabled { get; set; } = false;

        /// <summary>
        /// Service provider
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }
    }
}