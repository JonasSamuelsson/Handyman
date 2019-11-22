using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IExceptionHandler
    {
        Task Handle(IEnumerable<Exception> exceptions);
    }
}