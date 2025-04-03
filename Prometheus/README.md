# Prometheus - Setup & Configuration

This document provides instructions for setting up and running Prometheus on Windows, along with its configuration for scraping Loki metrics.

## Setup & Running Prometheus

### Prerequisites
- Download [Prometheus](https://prometheus.io/download/) for Windows.
- Ensure you have a `prometheus.yml` configuration file.

### Running Prometheus
To start Prometheus, navigate to the Prometheus directory and run:
```sh
prometheus.exe --config.file=prometheus.yml
```
Prometheus will start and listen on `localhost:9090` for HTTP queries and metrics exploration.

## Prometheus Configuration (`prometheus.yml`)

```yaml
# Global configuration
global:
  scrape_interval: 15s # Set the scrape interval to every 15 seconds. Default is every 1 minute.
  evaluation_interval: 15s # Evaluate rules every 15 seconds. The default is every 1 minute.
  # scrape_timeout is set to the global default (10s).

# Scrape configuration
scrape_configs:
  # Prometheus itself
  - job_name: "prometheus"
    static_configs:
      - targets: ["localhost:9090"]

  # Loki monitoring
  - job_name: "loki"
    static_configs:
      - targets: ["localhost:3100"]
        labels:
          job: "loki"
          instance: "localhost:3100"
```

## Verifying Prometheus Metrics

Open a browser and go to: `http://localhost:9090`

## Conclusion
With this setup, Prometheus collects Loki metrics every 15 seconds, enabling efficient monitoring and analysis of logging performance.

