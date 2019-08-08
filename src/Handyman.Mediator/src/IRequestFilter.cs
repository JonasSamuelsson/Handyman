﻿using System;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IRequestFilter<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Execute(RequestFilterContext<TRequest> context, Func<Task<TResponse>> next);
    }
}