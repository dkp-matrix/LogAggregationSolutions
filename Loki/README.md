# Loki - Log Aggregation

Loki is a horizontally scalable, multi-tenant log aggregation system inspired by Prometheus. This setup guide will help you configure and run Loki on Windows.

## Prerequisites

Before running Loki, ensure that you have the following:

- [loki-windows-amd64.exe](https://grafana.com/docs/loki/latest/installation/local/) (Downloaded from Grafana's official website) for Windows OS (64-bit)
- A valid `loki-config.yaml` configuration file

## Setup and Running Loki

### 1. Download Loki

1. Download the latest Loki binary for Windows from the [Grafana Loki releases page](https://github.com/grafana/loki/releases).
2. Extract the downloaded archive to your preferred location.
3. Navigate to the extracted folder.

### 2. Create the Configuration File

Create a `loki-config.yaml` file in the same directory as `loki-windows-amd64.exe`. A basic configuration file may look like this:

[View loki-config.yaml](./loki-config.yaml)

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

## Loki Log Basic Structure & Format

Loki ingests logs in JSON format, structured as follows:

```json
{
  "streams": [
    {
      "stream": {
        "job": "my-app",
        "host": "localhost"
      },
      "values": [["1672531200000000000", "This is a sample log entry"]]
    }
  ]
}
```

### Breakdown:

- **`stream`**: Contains labels that categorize logs (e.g., `job`, `host`).
- **`values`**: A list of timestamped log entries.
  - First value: UNIX timestamp in nanoseconds.
  - Second value: Actual log message.

## Custom Log Structure & Adding Fields

Loki allows custom fields via labels in the `stream` section.

### Example: Adding Custom Fields

Modify the structure to include custom fields like `user_id` and `log_level`:

```json
{
  "streams": [
    {
      "stream": {
        "job": "my-app",
        "host": "localhost",
        "user_id": "12345",
        "log_level": "info"
      },
      "values": [["1672531200000000000", "User logged in successfully"]]
    }
  ]
}
```

### Steps to Implement Custom Fields

1. Update your logging framework (e.g., Serilog, Fluentd, Promtail) to include custom labels.
2. Modify the Loki ingestion request to pass additional metadata.
3. Ensure your Loki queries filter by new labels (e.g., `{job="my-app", user_id="12345"}`).

## Querying Logs with Custom Fields

To filter logs based on custom fields, use Loki's LogQL:

```logql
{job="my-app", user_id="12345"} |= "User logged in"
```

This retrieves all logs where `user_id` is `12345` and the message contains "User logged in".

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
