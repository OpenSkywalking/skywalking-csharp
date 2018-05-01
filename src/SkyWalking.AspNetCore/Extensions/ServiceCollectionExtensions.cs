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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkyWalking.Diagnostics.HttpClient;
using SkyWalking.Diagnostics.SqlClient;
using SkyWalking.Extensions.DependencyInjection;

namespace SkyWalking.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static SkyWalkingBuilder AddSkyWalking(this IServiceCollection services,
            Action<SkyWalkingOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return services.Configure(options).AddSkyWalkingCore();
        }

        public static SkyWalkingBuilder AddSkyWalking(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.Configure<SkyWalkingOptions>(configuration).AddSkyWalkingCore();
        }

        private static SkyWalkingBuilder AddSkyWalkingCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new SkyWalkingBuilder(services);

            builder.AddHosting().AddDiagnostics().AddHttpClient().AddSqlClient();

            return builder;
        }
    }
}