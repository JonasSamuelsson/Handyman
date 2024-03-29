﻿using Handyman.DataContractValidator.Model;
using System;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class AnyTypeInfoResolver : ITypeInfoResolver
    {
        public ITypeInfo ResolveTypeInfo(object o, TypeInfoResolverContext context, Func<object, ITypeInfo> next)
        {
            return (o as Type ?? o.GetType()) == typeof(Any)
                ? new AnyTypeInfo()
                : next(o);
        }
    }
}