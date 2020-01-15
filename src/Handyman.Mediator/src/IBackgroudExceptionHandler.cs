using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IBackgroudExceptionHandler
    {
        Task Handle(IEnumerable<Exception> exceptions);
    }
}