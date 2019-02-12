/*
 * Licensed to the OpenSkywalking under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The OpenSkywalking licenses this file to You under the Apache License, Version 2.0
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
using System.Collections.Generic;
using System.IO;
using System.Web;
using SkyWalking.Components;
using SkyWalking.Config;
using SkyWalking.Context;
using SkyWalking.Context.Tag;
using SkyWalking.Context.Trace;

namespace SkyWalking.AspNet
{
    internal class SkyWalkingApplicationRequestCallback
    {
        private readonly IContextCarrierFactory _contextCarrierFactory;
        private readonly InstrumentConfig _config;

        public SkyWalkingApplicationRequestCallback(IConfigAccessor configAccessor, IContextCarrierFactory carrierFactory)
        {
            _config = configAccessor.Get<InstrumentConfig>();
            _contextCarrierFactory = carrierFactory;
        }

        public void ApplicationOnBeginRequest(object sender, EventArgs e)
        {
            var httpApplication = sender as HttpApplication;
            var httpContext = httpApplication.Context;

            if(httpContext.Request.HttpMethod == "OPTIONS")
            {
                //asp.net Exclude OPTIONS request
                return;
            }

            var carrier = _contextCarrierFactory.Create();
            foreach (var item in carrier.Items)
                item.HeadValue = httpContext.Request.Headers[item.HeadKey];
            var httpRequestSpan = ContextManager.CreateEntrySpan($"{_config.ApplicationCode} {httpContext.Request.Path}", carrier);
            httpRequestSpan.AsHttp();
            httpRequestSpan.SetComponent(ComponentsDefine.AspNet);
            Tags.Url.Set(httpRequestSpan, httpContext.Request.Path);
            Tags.HTTP.Method.Set(httpRequestSpan, httpContext.Request.HttpMethod);

            var dictLog = new Dictionary<string, object>
                {
                    {"event", "AspNet BeginRequest"},
                    {"message", $"Request starting {httpContext.Request.Url.Scheme} {httpContext.Request.HttpMethod} {httpContext.Request.Url.OriginalString}"}
                };

            // record request body data
            SetBodyData(httpContext.Request, dictLog);
            httpRequestSpan.Log(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), dictLog);

            httpContext.Items.Add("span_Context", ContextManager.ActiveContext);
        }

        public void ApplicationOnEndRequest(object sender, EventArgs e)
        {
            var httpApplication = sender as HttpApplication;
            var httpContext = httpApplication.Context;
            ITracerContext context=null;

            if (httpContext.Request.HttpMethod == "OPTIONS")
            {
                //asp.net Exclude OPTIONS request
                return;
            }

            var httpRequestSpan = ContextManager.ActiveSpan;
            if (httpRequestSpan == null)
            {
                // ContextManager.ActiveSpan is null, from httpContext.Items
                if(!httpContext.Items.Contains("span_Context"))
                    return;

                context = httpContext.Items["span_Context"] as ITracerContext;
                if (context == null)
                    return;

                httpRequestSpan = context.ActiveSpan;
                if (httpRequestSpan == null)
                    return;
            }

            var statusCode = httpContext.Response.StatusCode;
            if (statusCode >= 400)
            {
                httpRequestSpan.ErrorOccurred();
            }

            Tags.StatusCode.Set(httpRequestSpan, statusCode.ToString());

            var exception = httpContext.Error;
            if (exception != null)
            {
                httpRequestSpan.ErrorOccurred().Log(exception);
            }

            httpRequestSpan.Log(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                new Dictionary<string, object>
                {
                    {"event", "AspNet EndRequest"},
                    {"message", $"Request finished {httpContext.Response.StatusCode} {httpContext.Response.ContentType}"}
                });
            
            ContextManager.StopSpan(httpRequestSpan, context);
        }

        /// <summary>
        /// record request body data
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dict"></param>
        private void SetBodyData(HttpRequest request, Dictionary<string, object> dict)
        {
            if (request.HttpMethod == "GET")
            {
                return;
            }

            if (dict == null)
                dict = new Dictionary<string, object>();

            if (request.ContentType?.ToLower().Contains("multipart/form-data")??false)
            {
                dict.Add("ContentLength", request.ContentLength);
                return;
            }

            var stearm = request.GetBufferedInputStream();
            using (StreamReader sr = new StreamReader(stearm))
            {
                var bodyStr = sr.ReadToEnd();
                dict.Add("Body", bodyStr);
            }
        }
    }
}