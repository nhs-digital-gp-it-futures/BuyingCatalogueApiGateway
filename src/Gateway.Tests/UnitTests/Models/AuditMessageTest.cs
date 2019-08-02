using FluentAssertions;
using Gateway.Models.Messages;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Gateway.Tests.UnitTests.Models
{    
    public class AuditMessageTest
    {
        [Fact]
        public void AuditMessage_SerializesCorrectly_Request()
        {
            var correlationId = "c463ae7f-fc2c-4386-a81c-4b3d7c383c70";
            var body = "{\"message\" : \"This is a message\" }";
            var endpoint = "/endpoint";
            var headers = new HeaderDictionary
            {
                { "bob", "dave" },
                { "X-Correlation-Id", correlationId }
            };
            var query = "?name=kate";
            var methodName = "GET";
            
            // Slightly hacky, but it works
            var auditMessage = JObject.Parse(new AuditMessage(endpoint, methodName, headers, query, body).GenerateAuditMessage());

            Dictionary<string, JToken> auditMessageTokens = new Dictionary<string, JToken>();

            foreach (var token in auditMessage)
            {
                auditMessageTokens.Add(token.Key, token.Value);
            }

            auditMessageTokens["Body"].ToString().Should().Be(body);
            auditMessageTokens["Method"].ToString().Should().Be(methodName);
            auditMessageTokens["Endpoint"].ToString().Should().Be(endpoint);
            auditMessageTokens["Query"].ToString().Should().Be(query);
        }

        [Fact]
        public void AuditMessage_SerializesCorrectly_Response()
        {
            var correlationId = "c463ae7f-fc2c-4386-a81c-4b3d7c383c70";
            var body = "{\"message\" : \"This is a message\" }";
            var headers = new HeaderDictionary
            {
                { "X-Correlation-Id", correlationId }
            };

            var auditMessage = JObject.Parse(new AuditMessage(headers, 404, body).GenerateAuditMessage());

            Dictionary<string, JToken> auditMessageTokens = new Dictionary<string, JToken>();

            foreach(var token in auditMessage)
            {
                auditMessageTokens.Add(token.Key, token.Value);
            }

            auditMessageTokens["Body"].ToString().Should().Be(body);
            int.Parse(auditMessageTokens["StatusCode"].ToString()).Should().Be(404);            
        }
    }
}
