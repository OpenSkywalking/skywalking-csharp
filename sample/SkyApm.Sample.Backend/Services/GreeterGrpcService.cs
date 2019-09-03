﻿using Grpc.Core;
using Grpc.Core.Interceptors;
using GrpcGreeter;
using SkyApm.Diagnostics.Grpc.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkyApm.Sample.Backend.Services
{
    public class GreeterGrpcService
    {
        private readonly Greeter.GreeterClient _client;
        public GreeterGrpcService(ClientDiagnosticInterceptor interceptor)
        {
            _client = new Greeter.GreeterClient(GetChannel(interceptor));
        }

        private CallInvoker GetChannel(ClientDiagnosticInterceptor interceptor)
        {
            var channel = new Channel("localhost:12345", ChannelCredentials.Insecure);
            var invoker = channel.Intercept(interceptor);
            return invoker;
        }

        public string SayHello(string name)
        {
            var reply = _client.SayHello(new HelloRequest { Name = name });
            return reply.Message;
        }

        public async Task<string> SayHelloAsync(string name)
        {
            var reply = await _client.SayHelloAsync(new HelloRequest { Name = name });
            return reply.Message;
        }

        public string SayHelloWithException(string name)
        {
            var reply = _client.SayHelloWithException(new HelloRequest { Name = name });
            return reply.Message;
        }

        public async Task<string> SayHelloWithExceptionAsync(string name)
        {
            var reply = await _client.SayHelloWithExceptionAsync(new HelloRequest { Name = name });
            return reply.Message;
        }
    }
}
