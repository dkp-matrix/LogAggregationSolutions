# Loki Metrics Analysis

This document provides an analysis of the Loki logging system metrics under a load of approximately 1,000 logs/second. The metrics are organized by component to provide a clear overview of system performance.

## Test Scenario

- **Log Rate:** 1,000 logs/second
- **Total Logs Count:** 11,001 \* 1,000 logs
- **Disk Usage:** 536 MB (56,221,6960 bytes)

## Table of Contents

- [API and Connection Metrics](#api-and-connection-metrics)
- [Distributor Metrics](#distributor-metrics)
- [Ingester Metrics](#ingester-metrics)
- [Index and Store Metrics](#index-and-store-metrics)
- [LogQL and Query Metrics](#logql-and-query-metrics)
- [Line Processing Metrics](#line-processing-metrics)
- [Go Runtime Metrics](#go-runtime-metrics)
- [Process Information](#process-information)
- [Cache Performance](#cache-performance)
- [Ring Metrics](#ring-metrics)
- [System Overview](#system-overview)

## API and Connection Metrics

![Loki API or Connections Metrics](metrics/Loki%20API%20or%20Connections%20Metrics.png)

## Distributor Metrics

![Loki Distributor Metrics](metrics/Loki%20Distributor%20Metrics.png)

## Ingester Metrics

![Loki Ingester Metrics](metrics/Loki%20Ingester%20Metrics.png)
![Loki Ingester Metrics 1](metrics/Loki%20Ingester%20Metrics%201.png)
![Loki Ingester Metrics 2](metrics/Loki%20Ingester%20Metrics%202.png)
![Loki Ingester Metrics 3](metrics/Loki%20Ingester%20Metrics%203.png)
![Loki Ingester Metrics 4](metrics/Loki%20Ingester%20Metrics%204.png)

## Index and Store Metrics

![Loki Index Metrics](metrics/Loki%20Index%20Metrics.png)
![Loki Store Metrics](metrics/Loki%20Store%20Metrics.png)
![Loki Store Metrics 1](metrics/Loki%20Store%20Metrics%201.png)

## LogQL and Query Metrics

![Loki LogQL Metrics](metrics/Loki%20LogQL%20Metrics.png)
![Loki LogQL Metrics 1](metrics/Loki%20LogQL%20Metrics%201.png)
![Loki Querier](metrics/Loki%20Querier.png)
![Loki Querier 1](metrics/Loki%20Querier%201.png)

## Line Processing Metrics

![Loki Line Metrics](metrics/Loki%20Line%20Metrics.png)

## Go Runtime Metrics

![Go GC Metrics](metrics/Go%20GC%20Metrics.png)
![Go Memory Metrics](metrics/Go%20Memory%20Metrics.png)
![Go General Info](metrics/Go%20General%20Info.png)

## Process Information

![Process Info](metrics/Process%20Info.png)

**Key Observations:**

- CPU usage peaked at 40.9 ms (avg 27.2 ms)
- Memory usage reached 466 MiB resident memory
- File descriptors at 1122 out of maximum 16777216

**Performance Indicators:**

- CPU utilization is moderate and appropriate for the workload
- Memory footprint is reasonable for the data volume
- File descriptor usage well within system limits

## Cache Performance

![Loki Cache Metrics](metrics/Loki%20Cache%20Metrics.png)

## Ring Metrics

![Loki Ring Metrics](metrics/Loki%20Ring%20Metrics.png)

## System Overview

![Loki General Info](metrics/Loki%20General%20Info.png)
