﻿/*
 * Licensed to the OpenSkywalking under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkyWalking.AspNetCore.Diagnostics;
using SkyWalking.Config;
using SkyWalking.Context;
using SkyWalking.Diagnostics;
using SkyWalking.Diagnostics.EntityFrameworkCore;
using SkyWalking.Diagnostics.HttpClient;
using SkyWalking.Diagnostics.SqlClient;
using SkyWalking.Utilities.Configuration;
using SkyWalking.Utilities.DependencyInjection;
using SkyWalking.Utilities.Logging;
using SkyWalking.Logging;
using SkyWalking.Service;
using SkyWalking.Transport;
using SkyWalking.Transport.Grpc;
using SkyWalking.Transport.Grpc.V5;
using SkyWalking.Transport.Grpc.V6;

namespace SkyWalking.Agent.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSkyWalkingCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IContextCarrierFactory, ContextCarrierFactory>();
            services.AddSingleton<ISegmentDispatcher, AsyncQueueSegmentDispatcher>();
            services.AddSingleton<IExecutionService, SegmentReportService>();
            services.AddSingleton<IExecutionService, RegisterService>();
            services.AddSingleton<IExecutionService, PingService>();
            services.AddSingleton<IExecutionService, SamplingRefreshService>();
            services.AddSingleton<IExecutionService, ServiceDiscoveryV5Service>();
            services.AddSingleton<ISkyWalkingAgentStartup, SkyWalkingAgentStartup>();
            services.AddSingleton<ISampler>(DefaultSampler.Instance);
            services.AddSingleton<IRuntimeEnvironment>(RuntimeEnvironment.Instance);
            services.AddSingleton<TracingDiagnosticProcessorObserver>();
            services.AddSingleton<IConfigAccessor, ConfigAccessor>();
            services.AddSingleton<IHostedService, InstrumentationHostedService>();
            services.AddSingleton<IEnvironmentProvider, HostingEnvironmentProvider>();
            services.AddGrpcTransport().AddLogging();
            services.AddSkyWalkingExtensions().AddAspNetCoreHosting().AddHttpClient().AddSqlClient()
                .AddEntityFrameworkCore(c => c.AddPomeloMysql().AddNpgsql().AddSqlite());
            return services;
        }

        private static IServiceCollection AddGrpcTransport(this IServiceCollection services)
        {
            services.AddSingleton<ISkyWalkingClientV5, SkyWalkingClientV5>();
            services.AddSingleton<ISegmentReporter, SegmentReporter>();
            services.AddSingleton<ConnectionManager>();
            services.AddSingleton<IPingCaller, PingCaller>();
            services.AddSingleton<IServiceRegister, ServiceRegister>();
            services.AddSingleton<IExecutionService, ConnectService>();
            return services;
        }

        private static IServiceCollection AddLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory, DefaultLoggerFactory>();
            return services;
        }
    }
}