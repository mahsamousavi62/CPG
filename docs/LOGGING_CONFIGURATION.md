# CPG Logging Configuration Guide

This document provides comprehensive configuration guidelines for the CPG standardized logging system.

## Table of Contents
- [Overview](#overview)
- [Serilog Configuration](#serilog-configuration)
- [Custom Enrichers](#custom-enrichers)
- [Elasticsearch Index Mapping](#elasticsearch-index-mapping)
- [Configuration Examples](#configuration-examples)

---

## Overview

The CPG logging system uses Serilog with multiple sinks (Elasticsearch, File, Console) and custom enrichers to capture structured audit logs with 25 parameters.

### Key Components

1. **Sinks**: Where logs are written (Elasticsearch, Files, Console)
2. **Enrichers**: Add contextual data to every log entry
3. **Filters**: Control which logs go to which sinks
4. **Formatters**: Control log output format

---

## Serilog Configuration

### Current Configuration (`appsettings.json`)

```json
{
  "Serilog": {
    "Using": [
      "Serilog.Sinks.Async",
      "Serilog.Sinks.File",
      "Serilog.Settings.Configuration",
      "Serilog.Expressions",
      "Serilog.Sinks.Elasticsearch",
      "Serilog.Enrichers.ClientInfo"
    ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "System.Net.Http.HttpClient": "Information",
        "signalR.LogLevel": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Logger",
        "Args": {
          "configureLogger": {
            "Filter": [
              {
                "Name": "ByExcluding",
                "Args": {
                  "expression": "StartsWith(@m, '[TraceInfo]')"
                }
              }
            ],
            "WriteTo": [
              {
                "Name": "Async",
                "Args": {
                  "configure": [
                    {
                      "Name": "File",
                      "Args": {
                        "path": "..\\Logs\\log.log",
                        "rollingInterval": "Day",
                        "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3} => Message: {Message:lj}{NewLine}Properties: {Properties}{NewLine}{Exception} [ENDOFMSG]{NewLine}{NewLine}",
                        "fileSizeLimitBytes": 52428800,
                        "shared": true,
                        "rollOnFileSizeLimit": true,
                        "retainedFileTimeLimit": "102.00:00:00",
                        "restrictedToMinimumLevel": "Information"
                      }
                    }
                  ],
                  "bufferSize": 10240,
                  "blockWhenFull": true
                }
              }
            ]
          }
        }
      },
      {
        "Name": "Elasticsearch",
        "Args": {
          "nodeUris": "http://10.104.146.11:9200",
          "indexFormat": "payment-cpg-develop-{0:yyyy.MM}",
          "batchPostingLimit": 50,
          "batchAction": "Create",
          "period": 2,
          "inlineFields": true,
          "connectionGlobalHeaders": "Authorization=Basic Y2hhcmlzcGF5LXByb2Qtc2VydmljZToyNkY0UWMzUjZXY1ZVdnU=",
          "emitEventFailure": "ThrowException",
          "autoRegisterTemplate": true,
          "autoRegisterTemplateVersion": "ESv8"
        }
      },
      {
        "Name": "Console",
        "Args": {
          "formatter": "Serilog.Formatting.Compact.RenderedCompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithExceptionDetails",
      "WithCorrelationId",
      "WithCorrelationIdHeader",
      "WithClientAgent",
      {
        "Name": "WithClientIp",
        "Args": {
          "xForwardHeaderName": "X-Forwarded-For"
        }
      }
    ]
  }
}
```

### Configuration Sections Explained

#### 1. Using
Declares which Serilog packages are in use:
- **Serilog.Sinks.Async**: Async logging for performance
- **Serilog.Sinks.File**: File-based logging
- **Serilog.Sinks.Elasticsearch**: Elasticsearch integration
- **Serilog.Enrichers.ClientInfo**: Client IP and User-Agent enrichment

#### 2. MinimumLevel
Controls which log levels are captured:
- **Default**: `Information` - logs Info, Warning, Error, Fatal
- **Override**: Can set different levels for specific namespaces

#### 3. WriteTo (Sinks)

**File Sink:**
- Path: `..\\Logs\\log.log`
- Rolling: Daily (creates new file each day)
- Size limit: 50MB per file
- Buffer: 10,240 entries
- Retention: 102 days

**Elasticsearch Sink:**
- Node: `http://10.104.146.11:9200`
- Index: `payment-cpg-develop-{year.month}`
- Batch size: 50 logs per batch
- Period: 2 seconds between batches
- Auto-register template: Enabled

**Console Sink:**
- Format: Rendered Compact JSON
- Used for local development and Docker logs

#### 4. Enrich (Built-in Enrichers)

- **FromLogContext**: Adds properties from LogContext.PushProperty()
- **WithMachineName**: Adds machine name
- **WithExceptionDetails**: Adds detailed exception info
- **WithCorrelationId**: Adds correlation_id (from Serilog.Enrichers.CorrelationId package)
- **WithCorrelationIdHeader**: Extracts correlation_id from X-Correlation-ID header
- **WithClientAgent**: Adds User-Agent
- **WithClientIp**: Extracts client IP from X-Forwarded-For header

---

## Custom Enrichers

### Added in Program.cs

The following custom enrichers are registered in `Program.cs`:

```csharp
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.WithCorrelationIdEnricher(services)
        .Enrich.WithRequestIdEnricher(services)
        .Enrich.WithUserContextEnricher(services);
});
```

#### 1. CorrelationIdEnricher

**Purpose**: Adds `correlation_id` to all log entries for request tracing

**Behavior**:
1. Checks for `X-Correlation-ID` header
2. Falls back to `HttpContext.TraceIdentifier`
3. Generates new GUID if neither exists

**Output Property**: `correlation_id`

**Implementation**: `CPG.Infrastructure.Logging.Enrichers.CorrelationIdEnricher`

#### 2. RequestIdEnricher

**Purpose**: Adds unique `request_id` for each operation

**Behavior**:
1. Checks for `X-Request-ID` header
2. Generates new GUID if not present
3. Caches in HttpContext.Items for request lifetime

**Output Property**: `request_id`

**Implementation**: `CPG.Infrastructure.Logging.Enrichers.RequestIdEnricher`

#### 3. UserContextEnricher

**Purpose**: Adds user/client context from JWT claims

**Behavior**:
Extracts from JWT token claims:
- `user_id`
- `company_id`
- `application_id`
- `client_id`
- `mobile_phone`

**Output Properties**: `user_id`, `company_id`, `application_id`, `client_id`, `mobile_phone`

**Implementation**: `CPG.Infrastructure.Logging.Enrichers.UserContextEnricher`

---

## Elasticsearch Index Mapping

### Recommended Index Template

To properly index the 25 audit log parameters, create this Elasticsearch template:

```json
PUT _index_template/payment-cpg-template
{
  "index_patterns": ["payment-cpg-*"],
  "priority": 1,
  "template": {
    "settings": {
      "number_of_shards": 3,
      "number_of_replicas": 1,
      "refresh_interval": "5s"
    },
    "mappings": {
      "properties": {
        "@timestamp": { "type": "date" },
        "level": { "type": "keyword" },
        "message": { "type": "text" },

        // Core Identification
        "correlation_id": { "type": "keyword" },
        "request_id": { "type": "keyword" },

        // User Context (from enrichers)
        "user_id": { "type": "long" },
        "company_id": { "type": "long" },
        "application_id": { "type": "long" },
        "client_id": { "type": "keyword" },
        "mobile_phone": { "type": "keyword" },

        // ClientCallLog nested object
        "ClientCallLog": {
          "type": "object",
          "properties": {
            "ServiceName": { "type": "keyword" },
            "HttpMethod": { "type": "keyword" },
            "RequestPath": { "type": "keyword" },
            "StatusCode": { "type": "integer" },
            "IsSuccess": { "type": "boolean" },
            "DurationMs": { "type": "long" },
            "IpAddress": { "type": "ip" },
            "UserAgent": { "type": "text" },
            "StartDateTime": { "type": "date" },
            "EndDateTime": { "type": "date" },
            "RequestBody": { "type": "text", "index": false },
            "ResponseBody": { "type": "text", "index": false },
            "ErrorCode": { "type": "keyword" }
          }
        },

        // ProviderCallLog nested object
        "ProviderCallLog": {
          "type": "object",
          "properties": {
            "ProviderName": { "type": "keyword" },
            "ProviderType": { "type": "keyword" },
            "ServiceType": { "type": "keyword" },
            "ServiceUrl": { "type": "keyword" },
            "IsSuccess": { "type": "boolean" },
            "IsTimeout": { "type": "boolean" },
            "RetryAttempt": { "type": "integer" },
            "DurationMs": { "type": "long" },
            "StartDateTime": { "type": "date" },
            "EndDateTime": { "type": "date" },
            "RequestBody": { "type": "text", "index": false },
            "ResponseBody": { "type": "text", "index": false },
            "ErrorCode": { "type": "keyword" },
            "ErrorMessage": { "type": "text" }
          }
        },

        // UserActionLog nested object
        "UserActionLog": {
          "type": "object",
          "properties": {
            "ActionType": { "type": "keyword" },
            "EntityType": { "type": "keyword" },
            "EntityId": { "type": "keyword" },
            "Changes": { "type": "text" },
            "IpAddress": { "type": "ip" },
            "Timestamp": { "type": "date" }
          }
        },

        // CallLog (legacy support)
        "CallLog": {
          "type": "object",
          "properties": {
            "ServiceType": { "type": "keyword" },
            "AuditType": { "type": "keyword" },
            "ProviderType": { "type": "keyword" },
            "ServiceCallStatus": { "type": "boolean" },
            "ServiceCallUrl": { "type": "keyword" },
            "ServiceCallDate": { "type": "date" },
            "ErrorCode": { "type": "keyword" },
            "CorrolationId": { "type": "keyword" },
            "RequestBody": { "type": "text", "index": false },
            "ResponseBody": { "type": "text", "index": false }
          }
        }
      }
    }
  }
}
```

---

## Configuration Examples

### Example 1: Querying Logs by Correlation ID

```json
GET payment-cpg-develop-*/_search
{
  "query": {
    "term": {
      "correlation_id": "abc123def456"
    }
  },
  "sort": [
    { "@timestamp": "asc" }
  ]
}
```

### Example 2: Finding Failed Provider Calls

```json
GET payment-cpg-develop-*/_search
{
  "query": {
    "bool": {
      "must": [
        { "exists": { "field": "ProviderCallLog" } },
        { "term": { "ProviderCallLog.IsSuccess": false } }
      ]
    }
  },
  "size": 100
}
```

### Example 3: Finding Timeout Scenarios

```json
GET payment-cpg-develop-*/_search
{
  "query": {
    "bool": {
      "must": [
        { "exists": { "field": "ProviderCallLog" } },
        { "term": { "ProviderCallLog.IsTimeout": true } }
      ]
    }
  },
  "aggs": {
    "by_provider": {
      "terms": {
        "field": "ProviderCallLog.ProviderName"
      }
    }
  }
}
```

### Example 4: User Actions for Specific Company

```json
GET payment-cpg-develop-*/_search
{
  "query": {
    "bool": {
      "must": [
        { "term": { "company_id": 12345 } },
        { "exists": { "field": "UserActionLog" } }
      ]
    }
  },
  "sort": [
    { "@timestamp": "desc" }
  ]
}
```

---

## Performance Tuning

### Recommended Settings

#### For High-Volume Production

```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Elasticsearch",
        "Args": {
          "batchPostingLimit": 100,
          "period": 5,
          "queueSizeLimit": 100000,
          "emitEventFailure": "WriteToSelfLog"
        }
      },
      {
        "Name": "Async",
        "Args": {
          "bufferSize": 50000,
          "blockWhenFull": false
        }
      }
    ]
  }
}
```

#### For Development/Debugging

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {RequestId} {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

---

## Environment-Specific Configuration

### Development

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    },
    "WriteTo": [
      {
        "Name": "Elasticsearch",
        "Args": {
          "indexFormat": "payment-cpg-dev-{0:yyyy.MM}"
        }
      }
    ]
  }
}
```

### Staging

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      {
        "Name": "Elasticsearch",
        "Args": {
          "indexFormat": "payment-cpg-stage-{0:yyyy.MM}"
        }
      }
    ]
  }
}
```

### Production

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Elasticsearch",
        "Args": {
          "indexFormat": "payment-cpg-prod-{0:yyyy.MM}",
          "batchPostingLimit": 100,
          "period": 5
        }
      }
    ]
  }
}
```

---

## Troubleshooting

### Issue: Logs not appearing in Elasticsearch

**Check:**
1. Elasticsearch connection: `curl http://10.104.146.11:9200`
2. Index exists: `GET payment-cpg-develop-*`
3. Serilog errors in console/file logs
4. Authorization header is correct

### Issue: Missing correlation_id or request_id

**Cause**: Enrichers not properly registered in Program.cs

**Fix**: Ensure enrichers are registered:
```csharp
.Enrich.WithCorrelationIdEnricher(services)
.Enrich.WithRequestIdEnricher(services)
```

### Issue: User context fields are null

**Cause**: JWT token not present or claims missing

**Check**:
1. Request has Authorization header
2. Token contains expected claims
3. UserContextEnricher is registered

---

## Additional Resources

- [Serilog Documentation](https://serilog.net/)
- [Serilog.Sinks.Elasticsearch](https://github.com/serilog-contrib/serilog-sinks-elasticsearch)
- [Elasticsearch Query DSL](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl.html)

---

**Last Updated**: 2025-10-11
**Version**: 1.0
**Author**: CPG Development Team
