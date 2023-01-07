﻿using Handyman.Tools.Docs.ValidateLinks;
using System;
using System.Threading.Tasks;

namespace Handyman.Tools.Docs.Tests.ValidateLinks;

public class TestHttpClient : IHttpClient
{
    public Func<string, Response> Handler { get; set; }

    public Task<Response> Get(string url)
    {
        return Task.FromResult(Handler.Invoke(url));
    }
}