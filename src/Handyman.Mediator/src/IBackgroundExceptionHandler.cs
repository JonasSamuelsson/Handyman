using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IBackgroundExceptionHandler
    {
        Task Handle(IEnumerable<Exception> exceptions, CancellationToken cancellationToken);
    }
}