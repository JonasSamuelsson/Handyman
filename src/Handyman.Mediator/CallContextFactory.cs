using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Handyman.Mediator
{
    internal static class CallContextFactory
    {
        public static CallContext GetRequestCallContext(Type requestType)
        {
            var adapterType = GetRequestHandlerAdapterType(requestType);
            var adapterFactory = CreateHandlerAdapterFactory(adapterType);
            var handlerInterface = GetRequestHandlerInterface(requestType);
            return new CallContext
            {
                AdapterFactory = adapterFactory,
                HandlerInterface = handlerInterface
            };
        }

        private static Type GetRequestHandlerAdapterType(Type requestType)
        {
            return typeof(RequestHandlerAdapter<>).MakeGenericType(requestType);
        }

        private static Type GetRequestHandlerInterface(Type requestType)
        {
            return requestType.GetTypeInfo().ImplementedInterfaces
               .Where(x => x == typeof(IRequest))
               .Select(x => typeof(IRequestHandler<>).MakeGenericType(requestType))
               .Single();
        }

        public static CallContext GetRequestResponseCallContext<TResponse>(Type requestType)
        {
            var adapterType = GetRequestResponseHandlerAdapterType<TResponse>(requestType);
            var adapterFactory = CreateHandlerAdapterFactory(adapterType);
            var handlerInterface = GetRequestResponseHandlerInterface(requestType);
            return new CallContext
            {
                AdapterFactory = adapterFactory,
                HandlerInterface = handlerInterface
            };
        }

        private static Type GetRequestResponseHandlerAdapterType<TResponse>(Type requestType)
        {
            var responseType = typeof(TResponse);
            return typeof(RequestHandlerAdapter<,>).MakeGenericType(requestType, responseType);
        }

        private static Type GetRequestResponseHandlerInterface(Type requestType)
        {
            return requestType.GetTypeInfo().ImplementedInterfaces
               .Where(x => x.IsConstructedGenericType)
               .Where(x => x.GetGenericTypeDefinition() == typeof(IRequest<>))
               .Select(x => typeof(IRequestHandler<,>).MakeGenericType(requestType, x.GenericTypeArguments[0]))
               .Single();
        }

        public static CallContext GetMessageCallContext(Type messageType)
        {
            var adapterType = GetMessageHandlerAdapterType(messageType);
            var adapterFactory = CreateHandlerAdapterFactory(adapterType);
            var handlerInterface = GetMessageHandlerInterface(messageType);
            return new CallContext
            {
                AdapterFactory = adapterFactory,
                HandlerInterface = handlerInterface
            };
        }

        private static Type GetMessageHandlerAdapterType(Type messageType)
        {
            return typeof(MessageHandlerAdapter<>).MakeGenericType(messageType);
        }

        private static Type GetMessageHandlerInterface(Type messageType)
        {
            return messageType.GetTypeInfo().ImplementedInterfaces
               .Where(x => x == typeof(IMessage))
               .Select(_ => typeof(IMessageHandler<>).MakeGenericType(messageType))
               .Single();
        }

        private static Func<object, object> CreateHandlerAdapterFactory(Type adapterType)
        {
            var lambdaParameter = Expression.Parameter(typeof(object), "handler");
            var ctor = adapterType.GetTypeInfo().DeclaredConstructors.Single();
            var ctorParameterType = ctor.GetParameters().Single().ParameterType;
            var ctorParameter = Expression.Convert(lambdaParameter, ctorParameterType);
            return Expression.Lambda<Func<object, object>>(Expression.New(ctor, ctorParameter), lambdaParameter).Compile();
        }
    }
}