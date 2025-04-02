# Loki - Log Aggregation

Loki is a horizontally scalable, multi-tenant log aggregation system inspired by Prometheus. This setup guide will help you configure and run Loki on Windows.

## Prerequisites

Before running Loki, ensure that you have the following:

- Windows OS (64-bit)
- [loki-windows-amd64.exe](https://grafana.com/docs/loki/latest/installation/local/) (Downloaded from Grafana's official website)
- A valid `loki-config.yml` configuration file

## Setup and Running Loki

### 1. Download Loki

1. Download the latest Loki binary for Windows from the [Grafana Loki releases page](https://github.com/grafana/loki/releases).
2. Extract the downloaded archive to your preferred location.
3. Navigate to the extracted folder.

### 2. Create the Configuration File

Create a `loki-config.yml` file in the same directory as `loki-windows-amd64.exe`. A basic configuration file may look like this:

```yaml
auth_enabled: false
server:
  http_listen_port: 3100
ingester:
  lifecycler:
    address: 127.0.0.1
    ring:
      kvstore:
        store: inmemory
      replication_factor: 1
    final_sleep: 0s
  chunk_idle_period: 5m
  chunk_retain_period: 30s
schema_config:
  configs:
    - from: 2024-01-01
      store: boltdb-shipper
      object_store: filesystem
      schema: v11
      index:
        prefix: index_
        period: 24h
storage_config:
  boltdb_shipper:
    active_index_directory: /tmp/loki/index
    cache_location: /tmp/loki/index_cache
    shared_store: filesystem
  filesystem:
    directory: /tmp/loki/chunks
limits_config:
  enforce_metric_name: false
  reject_old_samples: true
  reject_old_samples_max_age: 168h
chunk_store_config:
  max_look_back_period: 0s
table_manager:
  retention_deletes_enabled: false
  retention_period: 0s
```

### 3. Run Loki

To start Loki, open a Command Prompt or PowerShell in the directory where `loki-windows-amd64.exe` is located and run the following command:

```sh
loki-windows-amd64.exe --config.file=loki-config.yml
```

If everything is set up correctly, you should see logs indicating that Loki is running on port `3100`.

### 4. Verify Loki

To confirm that Loki is running correctly, open a browser and go to:

```
http://localhost:3100/ready
```

If Loki is running properly, you should receive a response: `ready`.

## Using Loki with Log Aggregation

Once Loki is running, you can send logs to it using various clients such as:

- **Promtail**: Official log shipper for Loki.
- **Serilog**: .NET logging with `Serilog.Sinks.GrafanaLoki`.
- **C++ Clients**: Using `cURL` or HTTP requests.

## Troubleshooting

### 1. Loki Fails to Start

- Ensure that `loki-config.yml` is correctly formatted.
- Check if another application is using port `3100`.

### 2. Logs Are Not Being Ingested

- Verify the client application is correctly configured to send logs.
- Check Loki logs for errors.

## Conclusion

Loki is now set up and running on your Windows machine. You can now integrate it with log collectors and visualization tools like Grafana.

For more details, check the [Grafana Loki documentation](https://grafana.com/docs/loki/latest/).
