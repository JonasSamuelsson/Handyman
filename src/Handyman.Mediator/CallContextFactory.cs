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
            var adapterFactory = CreateEventHandlerAdapterFactory(adapterType);
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
            var adapterFactory = CreateEventHandlerAdapterFactory(adapterType);
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

        public static CallContext GetEventCallContext(Type eventType)
        {
            var adapterType = GetEventHandlerAdapterType(eventType);
            var adapterFactory = CreateEventHandlerAdapterFactory(adapterType);
            var handlerInterface = GetEventHandlerInterface(eventType);
            return new CallContext
            {
                AdapterFactory = adapterFactory,
                HandlerInterface = handlerInterface
            };
        }

        private static Type GetEventHandlerAdapterType(Type eventType)
        {
            return typeof(EventHandlerAdapter<>).MakeGenericType(eventType);
        }

        private static Type GetEventHandlerInterface(Type eventType)
        {
            return eventType.GetTypeInfo().ImplementedInterfaces
               .Where(x => x == typeof(IEvent))
               .Select(_ => typeof(IEventHandler<>).MakeGenericType(eventType))
               .Single();
        }

        private static Func<object, object> CreateEventHandlerAdapterFactory(Type adapterType)
        {
            var lambdaParameter = Expression.Parameter(typeof(object), "handler");
            var ctor = adapterType.GetTypeInfo().DeclaredConstructors.Single();
            var ctorParameterType = ctor.GetParameters().Single().ParameterType;
            var ctorParameter = Expression.Convert(lambdaParameter, ctorParameterType);
            return Expression.Lambda<Func<object, object>>(Expression.New(ctor, ctorParameter), lambdaParameter).Compile();
        }

        public static CallContext GetAsyncEventCallContext(Type eventType)
        {
            var adapterType = GetAsyncEventHandlerAdapterType(eventType);
            var adapterFactory = CreateAsyncEventHandlerAdapterFactory(adapterType);
            var handlerInterface = GetAsyncEventHandlerInterface(eventType);
            return new CallContext
            {
                AdapterFactory = adapterFactory,
                HandlerInterface = handlerInterface
            };
        }

        private static Type GetAsyncEventHandlerAdapterType(Type eventType)
        {
            return typeof(AsyncEventHandlerAdapter<>).MakeGenericType(eventType);
        }

        private static Func<object, object> CreateAsyncEventHandlerAdapterFactory(Type adapterType)
        {
            var lambdaParameter = Expression.Parameter(typeof(object), "handler");
            var ctor = adapterType.GetTypeInfo().DeclaredConstructors.Single();
            var ctorParameterType = ctor.GetParameters().Single().ParameterType;
            var ctorParameter = Expression.Convert(lambdaParameter, ctorParameterType);
            return Expression.Lambda<Func<object, object>>(Expression.New(ctor, ctorParameter), lambdaParameter).Compile();
        }

        private static Type GetAsyncEventHandlerInterface(Type eventType)
        {
            return eventType.GetTypeInfo().ImplementedInterfaces
                .Where(x => x == typeof(IAsyncEvent))
                .Select(_ => typeof(IAsyncEventHandler<>).MakeGenericType(eventType))
                .Single();
        }
    }
}