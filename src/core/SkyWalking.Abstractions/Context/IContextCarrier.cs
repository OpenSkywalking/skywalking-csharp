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

using System.Collections.Generic;
using SkyWalking.Context.Ids;

namespace SkyWalking.Context
{
    public interface IContextCarrier
    {

        DistributedTraceId DistributedTraceId { get; }

        int EntryApplicationInstanceId { get; set; }

        string EntryOperationName { get; set; }
        
        int EntryOperationId { get; set; }

        int ParentApplicationInstanceId { get; set; }

        string ParentOperationName { get; set; }
        
        int ParentOperationId { get; set; }

        string PeerHost { get; set; }
        
        int PeerId { get; set; }

        int SpanId { get; set; }

        ID TraceSegmentId { get; set; }

        bool IsValid { get; }

        IContextCarrier Deserialize(string text);

        string Serialize();

        CarrierItem Items { get; }

        void SetDistributedTraceIds(IEnumerable<DistributedTraceId> distributedTraceIds);
    }
}