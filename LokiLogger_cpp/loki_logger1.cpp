#include <curl/curl.h>
#include <iostream>
#include <string>
#include <chrono>
#include <unordered_map>
#include <sstream>
#include <mutex>

class LokiLogger {
private:
    CURL* curl;
    std::string loki_url;
    std::unordered_map<std::string, std::string> labels;
    bool verbose;
    std::mutex log_mutex;  // Ensure thread safety

    // Get current timestamp in nanoseconds
    static std::string get_current_timestamp() {
        auto now = std::chrono::system_clock::now();
        auto now_ns = std::chrono::duration_cast<std::chrono::nanoseconds>(now.time_since_epoch()).count();
        return std::to_string(now_ns);
    }

    // Write callback for CURL response (not really needed but useful for debugging)
    static size_t write_callback(void* contents, size_t size, size_t nmemb, void* userp) {
        ((std::string*)userp)->append((char*)contents, size * nmemb);
        return size * nmemb;
    }

    // Format labels into a valid JSON object
    std::string format_labels(const std::string& level) {
        std::stringstream ss;
        ss << "{";
        bool first = true;
        for (const auto& label : labels) {
            if (!first) ss << ",";
            ss << "\"" << label.first << "\":\"" << label.second << "\"";
            first = false;
        }
        ss << ",\"level\":\"" << level << "\"";  // Ensure level is not overwritten globally
        ss << "}";
        return ss.str();
    }

public:
    LokiLogger(const std::string& url, bool verbose_output = true) 
        : loki_url(url), verbose(verbose_output) {
        curl_global_init(CURL_GLOBAL_ALL);
        curl = curl_easy_init();
        
        // Default labels
        labels["source"] = "cpp_application";

        if (verbose) std::cout << "LokiLogger initialized with URL: " << loki_url << std::endl;
    }

    ~LokiLogger() {
        if (verbose) std::cout << "LokiLogger shutting down..." << std::endl;
        if (curl) curl_easy_cleanup(curl);
        curl_global_cleanup();
    }

    void add_label(const std::string& key, const std::string& value) {
        labels[key] = value;
        if (verbose) std::cout << "Added label: " << key << " = " << value << std::endl;
    }

    bool send_log(const std::string& message, const std::string& level = "info") {
        std::lock_guard<std::mutex> lock(log_mutex);  // Ensure thread safety

        if (!curl) {
            if (verbose) std::cerr << "Error: CURL not initialized" << std::endl;
            return false;
        }

        // Always print the log message to the console
        std::cout << "[" << level << "] " << message << std::endl;

        // Prepare JSON payload
        std::string timestamp = get_current_timestamp();
        std::string formatted_labels = format_labels(level);

        std::stringstream json_payload;
        json_payload << "{\"streams\":[{\"stream\":" << formatted_labels
                     << ",\"values\":[[\"" << timestamp << "\",\"" << message << "\"]]}]}";

        std::string payload = json_payload.str();
        std::string response;

        if (verbose) std::cout << "Sending to Loki: " << payload << std::endl;

        // Set up the CURL request
        curl_easy_setopt(curl, CURLOPT_URL, (loki_url + "/loki/api/v1/push").c_str());
        curl_easy_setopt(curl, CURLOPT_POST, 1L);
        curl_easy_setopt(curl, CURLOPT_POSTFIELDS, payload.c_str());
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, write_callback);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &response);

        // Set headers
        struct curl_slist* headers = NULL;
        headers = curl_slist_append(headers, "Content-Type: application/json");
        curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);

        // Perform the request
        CURLcode res = curl_easy_perform(curl);
        long response_code;
        curl_easy_getinfo(curl, CURLINFO_RESPONSE_CODE, &response_code);
        curl_slist_free_all(headers);

        if (res != CURLE_OK) {
            std::cerr << "curl_easy_perform() failed: " << curl_easy_strerror(res) << std::endl;
            return false;
        }

        if (verbose) {
            std::cout << "Loki response code: " << response_code << std::endl;
            std::cout << "Loki response: " << response << std::endl;
        }

        return response_code == 204;
    }

    bool log_debug(const std::string& message) { return send_log(message, "debug"); }
    bool log_info(const std::string& message) { return send_log(message, "info"); }
    bool log_warn(const std::string& message) { return send_log(message, "warn"); }
    bool log_error(const std::string& message) { return send_log(message, "error"); }
};

// Example usage
int main() {
    std::cout << "Loki Logger Example Starting..." << std::endl;

    // Initialize the logger with your Loki instance URL
    LokiLogger logger("http://localhost:3100");

    // Add custom labels
    logger.add_label("application", "my_cpp_app");
    logger.add_label("environment", "development");

    // Send logs with different levels
    logger.log_info("Application started");

    try {
        // Simulate an error
        std::cout << "Simulating an error..." << std::endl;
        throw std::runtime_error("Something went wrong");
    } catch (const std::exception& e) {
        logger.log_error(std::string("Exception caught: ") + e.what());
    }

    logger.log_debug("Detailed debug information");
    logger.log_warn("Warning: Resource usage high");
    logger.log_info("Application shutting down");

    std::cout << "Loki Logger Example Completed." << std::endl;
    std::cout << "Press Enter to exit..." << std::endl;
    std::cin.get();
    
    return 0;
}
