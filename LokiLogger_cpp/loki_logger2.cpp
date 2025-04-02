#include <iostream>
#include <curl/curl.h>
#include <chrono>
#include <thread>
#include <nlohmann/json.hpp>

using json = nlohmann::json;

// Loki server URL (change if needed)
const std::string LOKI_URL = "http://localhost:3100/loki/api/v1/push";

// Log settings
const int TEST_DURATION_SECONDS = 10;  // Total time to run
const int LOG_RATE_PER_SECOND = 5;     // Logs per second

// Function to send logs to Loki
void send_log_to_loki(const std::string &message) {
    CURL *curl = curl_easy_init();
    if (!curl) {
        std::cerr << "Failed to initialize CURL" << std::endl;
        return;
    }

    // Get current timestamp in nanoseconds
    std::string timestamp = std::to_string(
        std::chrono::duration_cast<std::chrono::nanoseconds>(
            std::chrono::system_clock::now().time_since_epoch()
        ).count()
    );

    // Correct JSON format for Loki
    json log_entry = {
        {"streams", {
            {
                {"stream", {{"job", "cpp-logger"}}}, 
                {"values", json::array({ {timestamp, message} })} // FIXED: Proper array structure
            }
        }}
    };

    std::string log_data = log_entry.dump();
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


int main() {
    std::cout << "Starting Loki Log Pusher..." << std::endl;
    auto start_time = std::chrono::steady_clock::now();

    int total_logs = TEST_DURATION_SECONDS * LOG_RATE_PER_SECOND;
    int interval_ms = 1000 / LOG_RATE_PER_SECOND; 

    for (int i = 0; i < total_logs; ++i) {
        send_log_to_loki("Test log entry " + std::to_string(i + 1));
        std::this_thread::sleep_for(std::chrono::milliseconds(interval_ms));
    }

    std::cout << "Logging finished." << std::endl;
    return 0;
}
