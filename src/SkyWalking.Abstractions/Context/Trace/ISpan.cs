﻿using System;
using System.Collections.Generic;
using System.Text;
using SkyWalking.NetworkProtocol.Trace;

namespace SkyWalking.Context.Trace
{
    /// <summary>
    /// The <code>AbstractSpan</code> represents the span's skeleton, which contains all open methods.
    /// </summary>
    public interface ISpan
    {
        /// <summary>
        /// Set the component id, which defines in ComponentsDefine
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        ISpan SetComponent(IComponent component);

        /// <summary>
        /// Only use this method in explicit instrumentation, like opentracing-skywalking-bridge. It it higher recommend
        /// don't use this for performance consideration.
        /// </summary>
        /// <param name="componentName"></param>
        /// <returns></returns>
        ISpan SetComponent(string componentName);

        /// <summary>
        /// Set a key:value tag on the Span.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ISpan Tag(string key, string value);

        ISpan SetLayer(SpanLayer layer);

        /// <summary>
        /// Record an exception event of the current walltime timestamp.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        ISpan Log(Exception exception);

        ISpan ErrorOccurred();

        bool IsEntry { get; }

        bool IsExit { get; }

        ISpan Log(long timestamp, IDictionary<string, object> @event);

        ISpan SetOperationName(string operationName);

        ISpan Start();
        
        int SpanId { get; }
        
        string OperationName { get; }
        
        int OperationId { get; }

        ISpan SetOperationId(int operationId);

        ISpan Start(long timestamp);
        
        void Ref(ITraceSegmentRef traceSegmentRef);
    }
}
