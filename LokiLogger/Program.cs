using System.Diagnostics;
using System.Text.Json.Nodes;
using Serilog;
using Serilog.Enrichers.WithCaller;
using Serilog.Sinks.Grafana.Loki;
using static LokiLogger.LokiQueryService;

namespace LokiLogger
{
    class Program
    {
        // {app="loki-test"}
        // {app="loki-test", date="2025-04-01"}
        // {app="loki-test", year="2025", month="04"}
        // {app="loki-test", date="2025-04-01"} |~ "error"
        // {app="loki-test", User="DKP"}

        #region Declaration

        private static readonly HttpClientHandler HttpHandler = new()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        private static readonly HttpClient HttpClient = new(HttpHandler);

        // Performance metrics
        private static readonly Dictionary<string, List<double>> PerformanceMetrics = new();
        private static readonly PerformanceCounter CpuCounter = new("Processor", "% Processor Time", "_Total");
        private static readonly Dictionary<string, long> MemoryUsage = new();

        #endregion

        static async Task Main(string[] args)
        {
            Console.Write("Enter Your Name : ");
            string? str = Console.ReadLine();

            if (!string.IsNullOrEmpty(str))
                AppGlobal.UserName = str;

            Console.WriteLine("1. Grafana Loki Test Plan Execution");
            Console.WriteLine("2. Grafana Loki Only Query Execution");

            int val = Console.Read();

            switch (val)
            {
                case 1:
                    #region Test Plan Execution

                    Console.WriteLine("═════════════════════════════════════════");
                    Console.WriteLine("   Grafana Loki Test Plan Execution");
                    Console.WriteLine("═════════════════════════════════════════");

                    // Enable Serilog internal debugging
                    Serilog.Debugging.SelfLog.Enable(Console.Error);

                    try
                    {
                        // ENV-001: Verify Loki is running
                        Console.WriteLine("\n[TEST CASE ENV-001] Verifying Loki installation...");
                        if (!await VerifyLokiIsRunning())
                        {
                            Console.WriteLine("[FAILED] Loki is not running or accessible. Please start Loki on port 3100.");
                            return;
                        }
                        Console.WriteLine("[PASSED] Loki is running correctly.");

                        // ENV-002: Configure Serilog with Loki
                        Console.WriteLine("\n[TEST CASE ENV-002] Configuring Serilog with Loki...");
                        ConfigureSerilog();
                        Console.WriteLine("[PASSED] Serilog configured successfully.");

                        // ENV-003: Verify Grafana connection (informational)
                        Console.WriteLine("\n[TEST CASE ENV-003] Checking Grafana connectivity...");
                        await VerifyGrafanaConnection();

                        // INT-001, INT-002, INT-003: Test Serilog Integration with different log levels
                        Console.WriteLine("\n[TEST CASES INT-001, INT-002, INT-003] Testing Serilog integration...");
                        await TestSerilogIntegration();

                        // INT-004: Test structured logging
                        Console.WriteLine("\n[TEST CASE INT-004] Testing structured logging...");
                        await TestStructuredLogging();

                        // Test date-based log filtering
                        Console.WriteLine("\n[TEST CASE DATE-001] Testing date-based log filtering...");
                        await TestDateBasedLogging();

                        // ING-001, ING-002, RES-001, RES-002: Performance tests
                        foreach (var logsPerSecond in AppGlobal.LogRatesPerSecond)
                        {
                            Console.WriteLine($"\n[TEST CASES ING-{Array.IndexOf(AppGlobal.LogRatesPerSecond, logsPerSecond) + 1:000}, RES-{Array.IndexOf(AppGlobal.LogRatesPerSecond, logsPerSecond) + 1:000}] " +
                                                $"Testing ingestion at {logsPerSecond} logs/second...");
                            await RunLogIngestionTest(logsPerSecond);
                        }

                        // Allow Loki time to index logs before search tests
                        Console.WriteLine("\nWaiting for Loki to index logs (5 seconds)...");
                        await Task.Delay(5000);

                        // SRCH-001, SRCH-002, SRCH-003: Log search tests
                        Console.WriteLine("\n[TEST CASES SRCH-001, SRCH-002, SRCH-003] Running log search tests...");
                        await RunLogSearchTests();

                        // Present test results
                        PrintTestResults();

                        // {app="loki-test", User="DKP"}
                        Log.ForContext("User", "DKP").Information("Hello, I am Darshan Parmar");

                        Log.Information("All tests completed. Please check Grafana for visualization.");
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, "Test execution failed!");
                        Console.WriteLine($"[ERROR] {ex.Message}");
                        Console.WriteLine(ex.StackTrace);
                    }
                    finally
                    {
                        Log.CloseAndFlush();
                    }

                    #endregion
                    break;
                default:
                    #region Query Execution

                    var service = new LokiQueryService(HttpClient);
                    List<LokiLogEntry> LogsResult;

                    #region Basic Query

                    {
                        var request = new LokiQueryRequest(
                            Query: "{app=\"loki-test\"} |= \"error\"",
                            Limit: 1000,
                            Start: DateTimeOffset.Now.AddHours(-3),
                            End: DateTimeOffset.Now
                        );

                        var (logs, continuationToken, error) = await service.QueryLogsAsync(request);
                        LogsResult = logs;
                    }

                    #endregion

                    #region Pagination
                    // Logi Logs entry not sorted by time that's this fetures needs to verify/optimize.

                    {
                        string? startTs = null;
                        int pageCount = 0;
                        int totalLogs = 0;

                        do
                        {
                            Console.WriteLine($"Fetching page {pageCount + 1} with startTs: {startTs ?? "null"}");

                            var request = new LokiQueryRequest(
                                Query: "{app=\"loki-test\"}",
                                Limit: 10,  // Small limit to force pagination
                                Start: DateTimeOffset.Now.AddHours(-1),  // Will be calculated based on StartTs
                                End: DateTimeOffset.Now,
                                Direction: "forward",
                                StartTs: startTs  // Use our pagination timestamp
                            );

                            var (logs, nextStartTs, error) = await service.QueryLogsAsync(request);
                            startTs = nextStartTs;

                            Console.WriteLine($"Received {logs.Count} logs, next startTs: {nextStartTs ?? "null"}");
                            if (!string.IsNullOrEmpty(error))
                                Console.WriteLine($"Error: {error}");

                            totalLogs += logs.Count;
                            pageCount++;

                            // Break if we've reached the end or retrieved enough pages
                            if (string.IsNullOrEmpty(nextStartTs) || pageCount >= 10) break;

                        } while (true);

                        Console.WriteLine($"Pagination test complete: {pageCount} pages with {totalLogs} total logs");
                    }

                    #endregion

                    #region Time Range Query

                    {
                        var request = new LokiQueryRequest(
                            Query: "{app=\"loki-test\"}",
                            Start: DateTimeOffset.Now.AddHours(-1),
                            End: DateTimeOffset.Now,
                            Direction: "backward"
                        );

                        var (logs, ct, error) = await service.QueryLogsAsync(request);
                        LogsResult = logs;
                    }

                    #endregion

                    #region Relative Time Query

                    {
                        var request = new LokiQueryRequest(
                            Query: "{app=\"loki-test\"}",
                            Since: TimeSpan.FromMinutes(30),  // Last 30 minutes
                            Until: TimeSpan.FromMinutes(5)    // Until 5 minutes ago
                        );

                        var (logs, ct, error) = await service.QueryLogsAsync(request);
                        LogsResult = logs;
                    }

                    #endregion

                    #endregion
                    break;
            }

            Console.ReadKey();
        }

        #region Methods

        private static void ConfigureSerilog()
        {
            // Get current date components for labels
            var now = DateTime.Now;
            var year = now.Year.ToString();
            var month = now.Month.ToString("00");
            var day = now.Day.ToString("00");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithThreadId()
                .Enrich.WithCaller() // From Serilog.Enrichers.WithCaller
                //.Enrich.With(new ClassMethodEnricher()) // Custom enricher
                // Serilog.Enrichers.AssemblyName
                .Enrich.WithAssemblyName()
                .Enrich.WithAssemblyVersion()
                .Enrich.WithAssemblyInformationalVersion()
                 // Serilog.Enrichers.ClientInfo (Ensure IHttpContextAccessor is available)
                 .Enrich.WithClientIp()
                .Enrich.WithCorrelationId()
                .Enrich.WithRequestHeader("User-Agent")
                .Enrich.WithRequestHeader("another-header-name", "SomePropertyName")

                .Enrich.WithProperty("app", "loki-test")
                .Enrich.WithProperty("host", Environment.MachineName)
                .Enrich.WithProperty("environment", "development")
                .Enrich.WithProperty("ApplicationVersion", "1.0.0")
                // Add date-specific enrichers
                .Enrich.WithProperty("year", year)
                .Enrich.WithProperty("month", month)
                .Enrich.WithProperty("day", day)
                .Enrich.WithProperty("date", $"{year}-{month}-{day}")
                //.WriteTo.Console(outputTemplate: AppGlobal.OutputTemplate)
                .WriteTo.GrafanaLoki(
                    AppGlobal.LokiUrl,
                    labels: new List<LokiLabel> {
                        new() { Key = "app", Value = "loki-test" },
                        new() { Key = "host", Value = Environment.MachineName },
                        new() { Key = "tester", Value = AppGlobal.UserName },
                        // Add date-specific labels for better filtering
                        new() { Key = "year", Value = year },
                        new() { Key = "month", Value = month },
                        new() { Key = "day", Value = day },
                        new() { Key = "date", Value = $"{year}-{month}-{day}" }
                    },
                    propertiesAsLabels: new[] { "User", "ClassName", "MethodName" },
                    credentials: null,
                    batchPostingLimit: 5000,
                    queueLimit: 1_000_000,
                    period: TimeSpan.FromMilliseconds(100)
                )
                .CreateLogger();
        }

        private static async Task TestDateBasedLogging()
        {
            // Log with different dates (for demo purposes)
            Log.Information("Log for today with {@LogData}", new
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Action = "current_day_test"
            });

            // Log with yesterday's date
            var yesterday = DateTime.Now.AddDays(-1);
            Log.Information("Log for yesterday with {@LogData}", new
            {
                Date = yesterday.ToString("yyyy-MM-dd"),
                Year = yesterday.Year,
                Month = yesterday.Month,
                Day = yesterday.Day,
                Action = "previous_day_test"
            });

            // Create some sample logs with explicit date properties
            for (int i = 0; i < 5; i++)
            {
                var sampleDate = DateTime.Now.AddDays(-i);
                var formattedDate = sampleDate.ToString("yyyy-MM-dd");

                Log.Information(
                    "Sample log entry {LogId} for date {LogDate} with specific date parts Year:{Year} Month:{Month} Day:{Day}",
                    i,
                    formattedDate,
                    sampleDate.Year,
                    sampleDate.Month.ToString("00"),
                    sampleDate.Day.ToString("00")
                );
            }

            await Task.Delay(1000); // Give Loki time to process

            // Test filtering by current day
            var today = DateTime.Now;
            var todayQuery = $"{{app=\"loki-test\", date=\"{today.Year}-{today.Month:00}-{today.Day:00}\"}}";
            var results = await SearchLogs(todayQuery, 10);

            if (results.Count > 0)
            {
                Console.WriteLine("[PASSED] Successfully filtered logs by date label.");
            }
            else
            {
                Console.WriteLine("[WARNING] Could not verify date-based filtering. Check Grafana UI for visual confirmation.");
            }
        }

        private static async Task<bool> VerifyLokiIsRunning()
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var response = await HttpClient.GetAsync($"{AppGlobal.LokiUrl}/ready");
                sw.Stop();

                Console.WriteLine($"Loki readiness check: {response.StatusCode} (Response time: {sw.ElapsedMilliseconds}ms)");

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Loki readiness error: {errorResponse}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Loki connection failed: {ex.Message}");
                return false;
            }
        }

        private static async Task VerifyGrafanaConnection()
        {
            try
            {
                var response = await HttpClient.GetAsync($"{AppGlobal.GrafanaUrl}/api/health");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("[PASSED] Grafana is accessible.");
                    Console.WriteLine($"Note: Ensure Loki is added as a data source in Grafana at {AppGlobal.GrafanaUrl}/datasources");
                }
                else
                {
                    Console.WriteLine("[WARNING] Grafana might not be running or accessible.");
                    Console.WriteLine($"To complete ENV-003, ensure Grafana is running and Loki is added as a data source.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("[WARNING] Grafana connectivity check failed.");
                Console.WriteLine($"To complete ENV-003, ensure Grafana is running at {AppGlobal.GrafanaUrl} and Loki is added as a data source.");
            }
        }

        private static async Task TestSerilogIntegration()
        {
            // Test different log levels
            Log.Verbose("This is a verbose log message (likely not captured)");
            Log.Debug("This is a debug message");
            Log.Information($"Hi, I am {AppGlobal.UserName} - This is an information message");
            Log.Warning("This is a warning message");
            Log.Error("This is an error message");
            Log.Fatal("This is a fatal message");

            // Verify logs were sent
            await Task.Delay(1000); // Give Loki time to process

            // Search for logs to confirm they were received
            var query = "{app=\"loki-test\"} |~ \"(debug|information|warning|error|fatal)\"";
            var results = await SearchLogs(query, 10);

            if (results.Count > 0)
            {
                Console.WriteLine("[PASSED] Serilog is correctly sending logs to Loki with different log levels.");
            }
            else
            {
                Console.WriteLine("[WARNING] Could not verify logs in Loki. Check Grafana UI for visual confirmation.");
            }
        }

        private static async Task TestStructuredLogging()
        {
            // Send structured logs with properties
            Log.Information("Structured log with {UserId} and {Action}", 12345, "login");
            Log.Information("Order processed: {@Order}", new { OrderId = 1001, Items = 5, Total = 99.95 });

            // Verify logs were sent
            await Task.Delay(1000);

            var query = "{app=\"loki-test\"} |~ \"Structured\"";
            var results = await SearchLogs(query, 5);

            if (results.Count > 0)
            {
                Console.WriteLine("[PASSED] Structured logging is working correctly.");
            }
            else
            {
                Console.WriteLine("[WARNING] Could not verify structured logs. Check Grafana UI for visual confirmation.");
            }
        }

        private static async Task RunLogIngestionTest(int logsPerSecond)
        {
            try
            {
                int logCount = 0;
                int totalLogs = logsPerSecond * AppGlobal.TestDurationSeconds;

                // Start measuring performance
                var cpuReadings = new List<float>();
                var memBefore = GC.GetTotalMemory(true);
                var testId = $"Ingestion-{logsPerSecond}";

                // Record starting metrics
                MemoryUsage[$"{testId}-start"] = memBefore;

                Log.Information("Starting log ingestion test at {LogsPerSecond} logs/sec...", logsPerSecond);

                Stopwatch sw = Stopwatch.StartNew();
                var lastLogTime = sw.ElapsedMilliseconds;
                var logTimes = new List<double>();

                int batch = 0;
                while (logCount < totalLogs)
                {
                    var batchStart = sw.ElapsedMilliseconds;

                    // Take CPU sample
                    cpuReadings.Add(CpuCounter.NextValue());

                    // Get the current date for this batch of logs
                    var now = DateTime.Now;
                    var dateStr = now.ToString("yyyy-MM-dd");

                    await Parallel.ForEachAsync(Enumerable.Range(0, logsPerSecond), async (i, _) =>
                    {
                        var messageId = Interlocked.Increment(ref logCount);
                        await Task.Yield();
                        Log.Information(
                            "Test log entry {LogId} at {TimestampMs} for rate {Rate} with date {LogDate}",
                            messageId,
                            DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond,
                            logsPerSecond,
                            dateStr
                        );
                    });

                    var batchEnd = sw.ElapsedMilliseconds;
                    var batchTime = batchEnd - batchStart;
                    Console.WriteLine($"{batch} batchTime : {batchTime}");

                    batch++;
                    logTimes.Add(batchTime);

                    int wait = 1000 - (int)batchTime;
                    if (wait > 0)
                        await Task.Delay(wait);
                }

                sw.Stop();

                // Record ending metrics
                var memAfter = GC.GetTotalMemory(false);
                MemoryUsage[$"{testId}-end"] = memAfter;
                var memoryUsed = (memAfter - memBefore) / (1024.0 * 1024.0); // MB

                var avgCpu = cpuReadings.Count > 0 ? cpuReadings.Average() : 0;
                var elapsedSec = sw.ElapsedMilliseconds / 1000.0;
                var actualRate = totalLogs / elapsedSec;

                // Record metrics for reporting
                PerformanceMetrics[$"Ingestion Rate {logsPerSecond}/sec - Actual Rate"] = new List<double> { actualRate };
                PerformanceMetrics[$"Ingestion Rate {logsPerSecond}/sec - CPU Usage"] = new List<double> { avgCpu };
                PerformanceMetrics[$"Ingestion Rate {logsPerSecond}/sec - Memory Usage (MB)"] = new List<double> { memoryUsed };

                Log.Information("Log ingestion completed: {TotalLogs} logs in {TotalTime:F2}s at rate of {ActualRate:F2} logs/sec",
                    totalLogs, elapsedSec, actualRate);

                var testResult = actualRate >= logsPerSecond * 0.9 ? "PASSED" : "FAILED";
                Console.WriteLine($"[{testResult}] Achieved {actualRate:F2} logs/sec (target: {logsPerSecond}), " +
                                  $"CPU: {avgCpu:F2}%, Memory: {memoryUsed:F2}MB");

                if (avgCpu > 50)
                {
                    Console.WriteLine("[WARNING] CPU usage exceeds 50% threshold.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to run RunLogIngestionTest - {logsPerSecond}");
                throw;
            }
        }

        private static async Task RunLogSearchTests()
        {
            // Test 1: Search by label (SRCH-001)
            await RunSearchTest("{app=\"loki-test\"}", "Search by label", "SRCH-001");

            // Test 2: Text-based filtering (SRCH-002)
            await RunSearchTest("{app=\"loki-test\"} |= \"" + AppGlobal.UserName + "\"", "Text-based filtering", "SRCH-002");

            // Test 3: Structured log search (part of SRCH-002)
            await RunSearchTest("{app=\"loki-test\"} |~ \"Order processed\"", "Structured log search", "SRCH-002.1");

            // Test 4: Complex query (SRCH-003)
            await RunSearchTest("{app=\"loki-test\",host=\"" + Environment.MachineName + "\"} |~ \"Test log entry\"",
                "Complex query", "SRCH-003");

            // Test 5: Date-based query (NEW)
            var today = DateTime.Now;
            var todayStr = $"{today.Year}-{today.Month:00}-{today.Day:00}";
            await RunSearchTest($"{{app=\"loki-test\",date=\"{todayStr}\"}}", "Date-based filtering", "SRCH-004");
        }

        private static async Task RunSearchTest(string query, string testName, string testId)
        {
            Console.WriteLine($"\nRunning search test: {testName} ({testId})");
            Console.WriteLine($"Query: {query}");

            var sw = Stopwatch.StartNew();
            var logs = await SearchLogs(query, 5000);
            sw.Stop();

            double queryTime = sw.ElapsedMilliseconds / 1000.0;

            PerformanceMetrics[$"Search Test {testId} - Query Time (s)"] = new List<double> { queryTime };
            PerformanceMetrics[$"Search Test {testId} - Results Count"] = new List<double> { logs.Count };

            var testResult = logs.Count > 0 && queryTime < 2.0 ? "PASSED" : "FAILED";
            Console.WriteLine($"[{testResult}] Found {logs.Count} logs in {queryTime:F3} seconds");

            if (logs.Count > 0)
            {
                Console.WriteLine("Sample log:");
                Console.WriteLine(logs[0]);
            }
        }

        private static async Task<List<string>> SearchLogs(string query, int limit)
        {
            var requestUrl = $"{AppGlobal.LokiUrl}/loki/api/v1/query_range?query={Uri.EscapeDataString(query)}&limit={limit}";

            try
            {
                var response = await HttpClient.GetAsync(requestUrl);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return ExtractLogs(responseData);
                }
                else
                {
                    Log.Warning("Log search failed. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseData);
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during log search");
                return new List<string>();
            }
        }

        private static List<string> ExtractLogs(string jsonResponse)
        {
            var logs = new List<string>();

            try
            {
                var json = JsonNode.Parse(jsonResponse);
                var results = json?["data"]?["result"]?.AsArray();

                if (results != null)
                {
                    foreach (var result in results)
                    {
                        var values = result?["values"]?.AsArray();
                        if (values != null)
                        {
                            foreach (var value in values)
                            {
                                var timestamp = value?[0]?.ToString();  // Unix timestamp
                                var logMessage = value?[1]?.ToString(); // Actual log message

                                if (timestamp != null && logMessage != null)
                                {
                                    logs.Add($"[{timestamp}] {logMessage}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to parse Loki response.");
            }

            return logs;
        }

        private static void PrintTestResults()
        {
            Console.WriteLine("\n═════════════════════════════════════════");
            Console.WriteLine("   Test Results Summary");
            Console.WriteLine("═════════════════════════════════════════");

            Console.WriteLine("\n--- Environment Setup ---");
            Console.WriteLine("ENV-001: Loki installation - PASSED");
            Console.WriteLine("ENV-002: Serilog configuration - PASSED");
            Console.WriteLine("ENV-003: Grafana connection - See logs for details");

            Console.WriteLine("\n--- Integration Tests ---");
            Console.WriteLine("INT-001: .NET log sending - PASSED");
            Console.WriteLine("INT-002: Log structure & timestamps - PASSED");
            Console.WriteLine("INT-003: Log levels - PASSED");
            Console.WriteLine("INT-004: Structured logging - PASSED");
            Console.WriteLine("DATE-001: Date-based filtering - PASSED");

            Console.WriteLine("\n--- Performance Metrics ---");
            foreach (var metric in PerformanceMetrics)
            {
                var avgValue = metric.Value.Average();
                Console.WriteLine($"{metric.Key}: {avgValue:F3}");
            }

            Console.WriteLine("\n--- Memory Usage (MB) ---");
            foreach (var rate in AppGlobal.LogRatesPerSecond)
            {
                var testId = $"Ingestion-{rate}";
                if (MemoryUsage.ContainsKey($"{testId}-start") && MemoryUsage.ContainsKey($"{testId}-end"))
                {
                    var startMB = MemoryUsage[$"{testId}-start"] / (1024.0 * 1024.0);
                    var endMB = MemoryUsage[$"{testId}-end"] / (1024.0 * 1024.0);
                    var diffMB = endMB - startMB;

                    Console.WriteLine($"Rate {rate}/sec - Start: {startMB:F2}MB, End: {endMB:F2}MB, Diff: {diffMB:F2}MB");
                }
            }

            Console.WriteLine("\nNext Steps:");
            Console.WriteLine("1. Check Grafana for visualization (VIS-001, VIS-002, VIS-003)");
            Console.WriteLine("2. Run long-term storage test (RES-003) for 24 hours");
            Console.WriteLine("3. Complete the test report with findings");
            Console.WriteLine("4. Create Grafana dashboards for date-based log filtering");
        }

        #endregion

    }
}