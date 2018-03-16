﻿using SkyWalking.Context.Ids;
using SkyWalking.Dictionary;

namespace SkyWalking.Context
{
    public class ContextCarrier : IContextCarrier
    {
        private ID _traceSegmentId;
        /// <summary>
        /// id of parent span
        /// </summary>
        private int _spanId = -1;

        /// <summary>
        /// id of parent application instance
        /// </summary>
        private int _parentApplicationInstanceId = DictionaryUtil.NullValue;

        /// <summary>
        /// id of first application instance in this distributed trace
        /// </summary>
        private int _entryApplicationInstanceId = DictionaryUtil.NullValue;

        /// <summary>
        /// peer(ipv4/ipv6/hostname + port) of the server , from client side .
        /// </summary>
        private string _peerHost;

        /// <summary>
        /// Operation/Service name of the first one in this distributed trace .
        /// </summary>
        private string _entryOperationName;

        /// <summary>
        /// Operation/Service name of the parent one in this distributed trace .
        /// </summary>
        private string _parentOPerationName;

        private DistributedTraceId _primaryDistributedTraceId;
        
        
        
        public DistributedTraceId DistributedTraceId { get; set; }
        
        public int EntryApplicationInstanceId { get; set; }

        public string EntryOperationName
        {
            get { return _entryOperationName; }
            set { _entryOperationName = "#" + value; }
        }
        
        public int ParentApplicationInstanceId { get; set; }
        
        public string ParentOperationName { get; set; }
        
        public string PeerHost { get; set; }
        
        public int SpanId { get; set; }
        
        public ID TraceSegmentId { get; set; }
        
        public DistributedTraceId PrimaryDistributedTraceId { get; }

        public bool IsValid
        {
            get
            {
                return _traceSegmentId != null
                       && _traceSegmentId.IsValid
                       && _spanId > -1
                       && _parentApplicationInstanceId != DictionaryUtil.NullValue
                       && _entryApplicationInstanceId != DictionaryUtil.NullValue
                       && string.IsNullOrEmpty(_parentOPerationName)
                       && string.IsNullOrEmpty(_entryOperationName)
                       && _primaryDistributedTraceId != null;
            }
        }

        public IContextCarrier Deserialize(string text)
        {
            string[] parts = text?.Split("|".ToCharArray(), 8);
            if (parts?.Length == 8)
            {
                _traceSegmentId = new ID(parts[0]);
                _spanId = int.Parse(parts[1]);
                _parentApplicationInstanceId = int.Parse(parts[2]);
                _entryApplicationInstanceId = int.Parse(parts[3]);
                _peerHost = parts[4];
                _entryOperationName = parts[5];
                _parentOPerationName = parts[6];
                _primaryDistributedTraceId = new PropagatedTraceId(parts[7]);
            }

            return this;
        }

        public string Serialize()
        {
            if (!IsValid)
            {
                return string.Empty;
            }

            return string.Join("|",
                TraceSegmentId.Encode,
                SpanId.ToString(),
                ParentApplicationInstanceId.ToString(),
                EntryApplicationInstanceId.ToString(),
                PeerHost,
                EntryOperationName,
                ParentOperationName,
                PrimaryDistributedTraceId.Encode);
        }

        public CarrierItem Items
        {
            get
            {
                SW3CarrierItem carrierItem = new SW3CarrierItem(this, null);
                CarrierItemHead head = new CarrierItemHead(carrierItem);
                return head;
            }
        }
    }
}