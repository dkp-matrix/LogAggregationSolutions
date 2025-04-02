# Loki Logger - C++ to Grafana Loki

This repository contains C++ implementations to send logs to a Grafana Loki instance using the HTTP API. The code provides different approaches for logging, including batch logging, structured logging, and rate-controlled logging.

## Directory Structure
```
📁 loki
    ├── loki_logger.cpp   # Batch logging with optimized requests
    ├── loki_logger1.cpp  # Structured logging with labels
    ├── loki_logger2.cpp  # High-frequency logging for testing
    ├── README.md         # Documentation
```

## Dependencies
Before compiling and running the code, ensure you have the following dependencies installed:

- [GCC](https://gcc.gnu.org/) (for compiling C++)
- [cURL](https://curl.se/) (for HTTP requests)
- [JSON for Modern C++](https://github.com/nlohmann/json) (for JSON handling)

## Compilation Commands
Use the following commands to compile each logger:

### **1. Batch Logging (loki_logger.cpp)**
```sh
g++ -o loki_logger loki_logger.cpp -lcurl -I json/include
```

### **2. Structured Logging with Labels (loki_logger1.cpp)**
```sh
g++ -std=c++11 loki_logger1.cpp -o loki_logger1 -lcurl
```

### **3. Rate-Controlled Logging (loki_logger2.cpp)**
```sh
g++ -std=c++11 loki_logger2.cpp -o loki_logger2 -lcurl
```

## Usage

### **1. Batch Logging (`loki_logger.cpp`)**
This implementation efficiently batches logs and sends them periodically to Loki.
```sh
./loki_logger
```

### **2. Structured Logging (`loki_logger1.cpp`)**
This version supports custom labels and structured logging.
```sh
./loki_logger1
```

### **3. Rate-Controlled Logging (`loki_logger2.cpp`)**
This implementation pushes logs at a controlled rate (e.g., 5 logs/sec for 10 seconds).
```sh
./loki_logger2
```

## Configuration
Each logger sends logs to **Grafana Loki** at:
```
http://localhost:3100/loki/api/v1/push
```
If your Loki instance is running on a different URL, update the `LOKI_URL` variable in each source file accordingly.

## Features
- **Batch Logging:** Efficiently pushes logs in batches to reduce network requests.
- **Structured Logging:** Supports labeled logs for better filtering.
- **Rate-Controlled Logging:** Controls log frequency to avoid overloading Loki.
- **Thread-Safe Logging:** Uses mutex locks for concurrent logging.
- **Error Handling:** Prints CURL errors when logs fail to send.

## Example Output
When running `loki_logger1.cpp`, you should see output similar to:
```
LokiLogger initialized with URL: http://localhost:3100
[info] Application started
Sending to Loki: {"streams":[{"stream":{"application":"my_cpp_app","environment":"development","level":"info"},"values":[["...","Application started"]]}]}
Loki response code: 204
```

## Troubleshooting
### **1. Loki Not Running**
Ensure your Loki instance is running.

### **2. cURL Not Installed**
For Ubuntu:
```sh
sudo apt install libcurl4-openssl-dev
```
For macOS:
```sh
brew install curl
```

### **3. JSON Library Missing**
Download [JSON for Modern C++](https://github.com/nlohmann/json) and include it in `json/include/`.
