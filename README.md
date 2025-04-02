# Log Aggregation Solutions

A comprehensive suite of tools and libraries for modern log aggregation, analysis, and management in .NET and C++ applications.

## Overview

LogAggregationSolutions is a collection of projects designed to demonstrate, test, and implement efficient log management strategies for .NET and C++ applications. The solution focuses on centralized logging, performance benchmarking, and integration with popular log aggregation systems.

## Projects

### LokiLogger

A testing and benchmarking tool for Grafana Loki integration with .NET applications using Serilog. This project evaluates Loki's capabilities for log ingestion, querying, and visualization via Grafana.

[View LokiLogger Documentation](./LokiLogger/README.md)

#### Key Features:

- Performance benchmarking for log ingestion
- Serilog integration with Grafana Loki
- Comprehensive test suite for log filtering and querying
- Date-based log organization and retrieval

### LokiLogger_cpp

A C++ logging library for sending logs to Grafana Loki using its HTTP API. This project demonstrates different logging approaches such as batch logging, structured logging, and rate-controlled logging.

[View LokiLogger_cpp Documentation](./LokiLogger_cpp/README.md)

#### Key Features:

- Efficient log batching to reduce network overhead
- Structured logging with custom labels
- Rate-controlled log ingestion for testing scalability
- Thread-safe logging with mutex locks

### Loki

A self-hosted Grafana Loki instance for log aggregation and analysis. This component provides the configuration and setup guide for running Loki locally.

[View Loki Documentation](./Loki/README.md)

#### Key Features:

- Centralized log storage with query capabilities
- Horizontal scalability and multi-tenancy
- Integration with Grafana for visualization

## Getting Started

### Prerequisites

- .NET 6.0 or later (for .NET components)
- C++ compiler with cURL support (for LokiLogger_cpp)
- Grafana (for visualization)
- Grafana Loki (for log storage and querying)

### Building the Solution

```bash
# Clone the repository
git clone https://github.com/yourusername/LogAggregationSolutions.git

# Navigate to the solution directory
cd LogAggregationSolutions

# Build the .NET solution
dotnet build
```

### Running Projects

```bash
# Run LokiLogger
cd LokiLogger
dotnet run

# Run LokiLogger_cpp
cd LokiLogger_cpp
g++ -o loki_logger loki_logger.cpp -lcurl
./loki_logger

# Run Loki (Windows)
cd Loki
loki-windows-amd64.exe --config.file=loki-config.yml
```

## Architecture

The solution follows a modular architecture where each project addresses specific aspects of log aggregation:

- **LokiLogger**: Focuses on Grafana Loki integration and performance benchmarking (C#)
- **LokiLogger_cpp**: Provides C++ integration with Loki for efficient log streaming
- **Loki**: Local setup for running Grafana Loki

## Common Use Cases

- Performance testing logging backends
- Evaluating log query efficiency
- Implementing structured logging
- Setting up centralized logging for distributed applications

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Roadmap

- Add support for additional log aggregation systems
- Create a unified dashboard for comparing performance metrics
- Develop libraries for automatic log enrichment
- Implement machine learning components for log analysis and anomaly detection
