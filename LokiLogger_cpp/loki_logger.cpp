#include <iostream>
#include <vector>
#include <thread>
#include <mutex>
#include <chrono>
#include <condition_variable>
#include <nlohmann/json.hpp>
#include <curl/curl.h>

using json = nlohmann::json;

// Loki Push API
const std::string LOKI_URL = "http://localhost:3100/loki/api/v1/push";

// Mutex and condition variable for thread safety
std::mutex log_mutex;
std::condition_variable log_cv;
std::vector<std::pair<std::string, std::string>> log_buffer; // Store timestamp-message pairs
bool stop_logging = false;

// **Batch Config**
const int BATCH_SIZE = 100;           // Send logs when 100 logs are collected
const int FLUSH_INTERVAL_MS = 5000;   // Flush logs every 5 seconds

void send_logs_to_loki() {
    while (true) {
        std::vector<std::pair<std::string, std::string>> logs_to_send;

        {
            std::unique_lock<std::mutex> lock(log_mutex);
            log_cv.wait_for(lock, std::chrono::milliseconds(FLUSH_INTERVAL_MS), [] { 
                return log_buffer.size() >= BATCH_SIZE || stop_logging; 
            });

            if (stop_logging && log_buffer.empty()) break;

            if (!log_buffer.empty()) {
                logs_to_send.swap(log_buffer); // Move logs out of buffer (avoids locking issues)
            }
        }

        if (!logs_to_send.empty()) {
            // Construct Loki-compatible JSON payload
            json log_entry;
            log_entry["streams"] = json::array();
            
            json stream;
            stream["stream"] = { {"job", "cpp-logger"} };
            stream["values"] = json::array();

            for (const auto& log : logs_to_send) {
                stream["values"].push_back({ log.first, log.second });
            }

            log_entry["streams"].push_back(stream);

            // Convert JSON to string
            std::string log_data = log_entry.dump();

            // Send logs via CURL
            CURL *curl = curl_easy_init();
            if (curl) {
                struct curl_slist *headers = nullptr;
                headers = curl_slist_append(headers, "Content-Type: application/json");

                curl_easy_setopt(curl, CURLOPT_URL, LOKI_URL.c_str());
                curl_easy_setopt(curl, CURLOPT_POST, 1L);
                curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);
                curl_easy_setopt(curl, CURLOPT_POSTFIELDS, log_data.c_str());

                CURLcode res = curl_easy_perform(curl);
                if (res != CURLE_OK) {
                    std::cerr << "CURL Error: " << curl_easy_strerror(res) << std::endl;
                }

                curl_slist_free_all(headers);
                curl_easy_cleanup(curl);
            }
        }
    }
}

void log_message(const std::string& message) {
    std::string timestamp = std::to_string(
        std::chrono::duration_cast<std::chrono::nanoseconds>(
            std::chrono::system_clock::now().time_since_epoch()
        ).count()
    );

    {
        std::lock_guard<std::mutex> lock(log_mutex);
        log_buffer.emplace_back(timestamp, message);
    }

    if (log_buffer.size() >= BATCH_SIZE) {
        log_cv.notify_one(); // Signal batch is ready
    }
}

int main() {
    std::thread log_thread(send_logs_to_loki); // Start log processor

    for (int i = 0; i < 500; i++) { // Simulating logs
        log_message("Test log entry " + std::to_string(i));
        std::this_thread::sleep_for(std::chrono::milliseconds(10));
    }

    // Wait before stopping
    std::this_thread::sleep_for(std::chrono::seconds(10));

    // Stop logging
    {
        std::lock_guard<std::mutex> lock(log_mutex);
        stop_logging = true;
    }
    log_cv.notify_one();
    log_thread.join();

    return 0;
}
