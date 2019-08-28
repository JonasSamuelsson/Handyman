using System.Collections.Generic;
using Handyman.Mediator;

namespace ExperimentRequestHandler
{
    [Experiment(typeof(AddHandler))]
    public class Add : IRequest<int>
    {
        public IEnumerable<int> Numbers { get; set; }
    }
}