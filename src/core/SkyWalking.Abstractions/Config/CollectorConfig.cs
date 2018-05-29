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

namespace SkyWalking.Config
{
    public class CollectorConfig
    {
        /// <summary>
        /// service registry check interval
        /// </summary>
        public static long ServiceRegisterCheckInterval { get; set; } = 3;
        
        /// <summary>
        /// Collector agent_gRPC/grpc service addresses.
        /// By using this, no discovery mechanism provided. The agent only uses these addresses to uplink data.
        /// Recommend to use this only when collector cluster IPs are unreachable from agent side. Such as:
        /// 1. Agent and collector cluster are in different VPC in Cloud.
        /// 2. Agent uplinks data to collector cluster through Internet.
        /// Single collector：DirectServers="127.0.0.1:11800"
        /// Collector cluster：DirectServers="10.2.45.126:11800,10.2.45.127:11800"
        /// </summary>
        public static string DirectServers { get; set; }
    }
}