# LokiLogger

A comprehensive testing and benchmarking tool for Grafana Loki integration with .NET applications using Serilog.

## Overview

LokiLogger is designed to evaluate and demonstrate the capabilities of Grafana Loki as a logging backend for .NET applications. It provides a test suite for assessing log ingestion performance, query capabilities, and integration with Serilog.

## Features

- **Comprehensive Test Suite**:
  - Environment validation for Loki and Grafana
  - Log level testing (Debug, Info, Warning, Error, Fatal)
  - Structured logging evaluation
  - Date-based filtering tests
  - Performance benchmarking with configurable log rates

- **Performance Metrics**:
  - Log ingestion rates (logs/second)
  - CPU utilization during ingestion
  - Memory consumption
  - Query response times

- **Flexible Query Interface**:
  - Basic and complex label queries
  - Text-based filtering
  - Time range queries (absolute and relative)
  - Result pagination

## Prerequisites

- .NET 6.0 or later
- Grafana (running on port 3000)
- Grafana Loki (running on port 3100)
- Serilog and related packages:
  - Serilog.Sinks.GrafanaLoki
  - Serilog.Enrichers.ThreadId
  - Serilog.Enrichers.WithCaller
  - Serilog.Enrichers.AssemblyName
  - Serilog.Enrichers.ClientInfo
  - Serilog.Enrichers.CorrelationId

## Configuration

The application uses constants defined in `AppGlobal.cs`:

```csharp
namespace LokiLogger
{
    public class AppGlobal
    {
        public const string GrafanaUrl = "http://192.168.27.169:3000";
        public const string LokiUrl = "http://192.168.27.169:3100";
        public const string QueryRange_LokiEndpoint = "/loki/api/v1/query_range";
        public const string OutputTemplate = "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] [{ThreadId}] {Message}{NewLine}{Exception}";

        // Customizable test parameters
        public static readonly int TestDurationSeconds = 1;
        public static readonly int[] LogRatesPerSecond = { 10_000 };

        public static string UserName = "Darshan Parmar";
    }
}
```

You can modify these values to suit your environment and testing requirements.

## Usage

### Running the Test Plan

1. Start the application
2. Enter your name when prompted
3. Select option `1` for "Grafana Loki Test Plan Execution"
4. Review the test results displayed in the console

### Running Specific Queries

1. Start the application
2. Enter your name when prompted
3. Select option `2` for "Grafana Loki Only Query Execution"
4. The application will run predefined queries against Loki

## Test Cases

The application includes the following test cases:

| ID | Description | Success Criteria |
|----|-------------|------------------|
| ENV-001 | Verify Loki installation | Loki responds to readiness check |
| ENV-002 | Configure Serilog with Loki | Configuration completes without errors |
| ENV-003 | Verify Grafana connection | Grafana is accessible |
| INT-001/002/003 | Test Serilog Integration | Logs of different levels are sent and received |
| INT-004 | Test structured logging | Structured logs are correctly transmitted |
| DATE-001 | Test date-based log filtering | Logs are filterable by date labels |
| ING-xxx | Performance tests | Achieves target ingestion rate |
| SRCH-001/002/003 | Log search tests | Queries return expected results |

## Query Examples

The application demonstrates several query patterns:

```
{app="loki-test"}
{app="loki-test", date="2025-04-01"}
{app="loki-test", year="2025", month="04"}
{app="loki-test", date="2025-04-01"} |~ "error"
{app="loki-test", User="DKP"}
```

## Performance Benchmarking

The application can benchmark Loki's ingestion performance with configurable rates. Modify the `LogRatesPerSecond` array in `AppGlobal.cs` to test different rates:

```csharp
public static readonly int[] LogRatesPerSecond = { 1_000, 5_000, 10_000 };
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
