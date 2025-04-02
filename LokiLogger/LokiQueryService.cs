using System.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LokiLogger
{
    #region Example of a Loki Query Request and Response

    // Example of a Loki Query Request and Response

    // GET request to Loki's query_range API
    // Endpoint: /loki/api/v1/query_range
    // Parameters:
    // - direction = forward
    // - end = 1743571336585000000
    // - limit = 20
    // - query = {app="loki-test"}
    // - start = 1743567736585000000
    // - step = 2000ms
    // HTTP Headers:
    // - Host: localhost:3100
    // - User-Agent: Grafana/11.5.1
    // - X-Dashboard-Uid: aeh6vvceutaf4f
    // - X-Datasource-Uid: deh6qkk1cujuoa
    // - X-Grafana-Id: <JWT Token>
    // - X-Grafana-Org-Id: 1
    // - X-Loki-Response-Encoding-Flags: categorize-labels
    // - X-Panel-Id: 1
    // - X-Panel-Plugin-Id: timeseries
    // - X-Query-Group-Id: 66ebabdf-ec63-46f4-937c-6659dcf18573
    // - Accept-Encoding: gzip

    // Sample JSON Response:
    // {
    //  "status": "success",
    //  "data": {
    //    "resultType": "streams",
    //    "encodingFlags": [
    //      "categorize-labels"
    //    ],
    //    "result": [
    //      {
    //        "stream": {
    //          "app": "loki-test",
    //          "date": "2025-04-02",
    //          "day": "02",
    //          "host": "400127-0",
    //          "month": "04",
    //          "service_name": "loki-test",
    //          "tester": "Darshan Parmar",
    //          "year": "2025"
    //        },
    //        "values": [
    //          [
    //            "1743568161538530100",
    //            "{\"Message\":\"This is a debug message\",\"MessageTemplate\":\"This is a debug message\",\"ThreadId\":10,\"Caller\":\"LokiLogger.Program.TestSerilogIntegration()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"debug\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "debug"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568161755410700",
    //            "{\"Message\":\"Hi, I am Darshan Parmar - This is an information message\",\"MessageTemplate\":\"Hi, I am Darshan Parmar - This is an information message\",\"ThreadId\":10,\"Caller\":\"LokiLogger.Program.TestSerilogIntegration()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568161761894700",
    //            "{\"Message\":\"This is a warning message\",\"MessageTemplate\":\"This is a warning message\",\"ThreadId\":10,\"Caller\":\"LokiLogger.Program.TestSerilogIntegration()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"warning\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "warn"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568161767963700",
    //            "{\"Message\":\"This is an error message\",\"MessageTemplate\":\"This is an error message\",\"ThreadId\":10,\"Caller\":\"LokiLogger.Program.TestSerilogIntegration()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"error\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "error"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568161774394700",
    //            "{\"Message\":\"This is a fatal message\",\"MessageTemplate\":\"This is a fatal message\",\"ThreadId\":10,\"Caller\":\"LokiLogger.Program.TestSerilogIntegration()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"critical\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "critical"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568162813437200",
    //            "{\"Message\":\"Structured log with 12345 and \\\"login\\\"\",\"MessageTemplate\":\"Structured log with {UserId} and {Action}\",\"UserId\":12345,\"Action\":\"login\",\"ThreadId\":6,\"Caller\":\"LokiLogger.Program.TestStructuredLogging()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568162822267200",
    //            "{\"Message\":\"Order processed: { OrderId: 1001, Items: 5, Total: 99.95 }\",\"MessageTemplate\":\"Order processed: {@Order}\",\"Order\":{\"OrderId\":1001,\"Items\":5,\"Total\":99.95},\"ThreadId\":6,\"Caller\":\"LokiLogger.Program.TestStructuredLogging()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568163843152300",
    //            "{\"Message\":\"Log for today with { Date: \\\"2025-04-02\\\", Action: \\\"current_day_test\\\" }\",\"MessageTemplate\":\"Log for today with {@LogData}\",\"LogData\":{\"Date\":\"2025-04-02\",\"Action\":\"current_day_test\"},\"ThreadId\":8,\"Caller\":\"LokiLogger.Program.TestDateBasedLogging()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568163850156100",
    //            "{\"Message\":\"Log for yesterday with { Date: \\\"2025-04-01\\\", Year: 2025, Month: 4, Day: 1, Action: \\\"previous_day_test\\\" }\",\"MessageTemplate\":\"Log for yesterday with {@LogData}\",\"LogData\":{\"Date\":\"2025-04-01\",\"Year\":2025,\"Month\":4,\"Day\":1,\"Action\":\"previous_day_test\"},\"ThreadId\":8,\"Caller\":\"LokiLogger.Program.TestDateBasedLogging()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568163855010600",
    //            "{\"Message\":\"Sample log entry 0 for date \\\"2025-04-02\\\" with specific date parts Year:2025 Month:\\\"04\\\" Day:\\\"02\\\"\",\"MessageTemplate\":\"Sample log entry {LogId} for date {LogDate} with specific date parts Year:{Year} Month:{Month} Day:{Day}\",\"LogId\":0,\"LogDate\":\"2025-04-02\",\"Year\":2025,\"Month\":\"04\",\"Day\":\"02\",\"ThreadId\":8,\"Caller\":\"LokiLogger.Program.TestDateBasedLogging()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568163859455600",
    //            "{\"Message\":\"Sample log entry 1 for date \\\"2025-04-01\\\" with specific date parts Year:2025 Month:\\\"04\\\" Day:\\\"01\\\"\",\"MessageTemplate\":\"Sample log entry {LogId} for date {LogDate} with specific date parts Year:{Year} Month:{Month} Day:{Day}\",\"LogId\":1,\"LogDate\":\"2025-04-01\",\"Year\":2025,\"Month\":\"04\",\"Day\":\"01\",\"ThreadId\":8,\"Caller\":\"LokiLogger.Program.TestDateBasedLogging()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568163863929700",
    //            "{\"Message\":\"Sample log entry 2 for date \\\"2025-03-31\\\" with specific date parts Year:2025 Month:\\\"03\\\" Day:\\\"31\\\"\",\"MessageTemplate\":\"Sample log entry {LogId} for date {LogDate} with specific date parts Year:{Year} Month:{Month} Day:{Day}\",\"LogId\":2,\"LogDate\":\"2025-03-31\",\"Year\":2025,\"Month\":\"03\",\"Day\":\"31\",\"ThreadId\":8,\"Caller\":\"LokiLogger.Program.TestDateBasedLogging()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568163868426800",
    //            "{\"Message\":\"Sample log entry 3 for date \\\"2025-03-30\\\" with specific date parts Year:2025 Month:\\\"03\\\" Day:\\\"30\\\"\",\"MessageTemplate\":\"Sample log entry {LogId} for date {LogDate} with specific date parts Year:{Year} Month:{Month} Day:{Day}\",\"LogId\":3,\"LogDate\":\"2025-03-30\",\"Year\":2025,\"Month\":\"03\",\"Day\":\"30\",\"ThreadId\":8,\"Caller\":\"LokiLogger.Program.TestDateBasedLogging()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568163872868600",
    //            "{\"Message\":\"Sample log entry 4 for date \\\"2025-03-29\\\" with specific date parts Year:2025 Month:\\\"03\\\" Day:\\\"29\\\"\",\"MessageTemplate\":\"Sample log entry {LogId} for date {LogDate} with specific date parts Year:{Year} Month:{Month} Day:{Day}\",\"LogId\":4,\"LogDate\":\"2025-03-29\",\"Year\":2025,\"Month\":\"03\",\"Day\":\"29\",\"ThreadId\":8,\"Caller\":\"LokiLogger.Program.TestDateBasedLogging()\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568164911100600",
    //            "{\"Message\":\"Starting log ingestion test at 10000 logs/sec...\",\"MessageTemplate\":\"Starting log ingestion test at {LogsPerSecond} logs/sec...\",\"LogsPerSecond\":10000,\"ThreadId\":6,\"Caller\":\"LokiLogger.Program.RunLogIngestionTest(int)\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568164989597700",
    //            "{\"Message\":\"Test log entry 3 at 63879164964989 for rate 10000 with date \\\"2025-04-02\\\"\",\"MessageTemplate\":\"Test log entry {LogId} at {TimestampMs} for rate {Rate} with date {LogDate}\",\"LogId\":3,\"TimestampMs\":63879164964989,\"Rate\":10000,\"LogDate\":\"2025-04-02\",\"ThreadId\":12,\"Caller\":\"LokiLogger.Program.RunLogIngestionTest(int)+(?) => { }\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568164989597700",
    //            "{\"Message\":\"Test log entry 6 at 63879164964989 for rate 10000 with date \\\"2025-04-02\\\"\",\"MessageTemplate\":\"Test log entry {LogId} at {TimestampMs} for rate {Rate} with date {LogDate}\",\"LogId\":6,\"TimestampMs\":63879164964989,\"Rate\":10000,\"LogDate\":\"2025-04-02\",\"ThreadId\":13,\"Caller\":\"LokiLogger.Program.RunLogIngestionTest(int)+(?) => { }\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568164989597800",
    //            "{\"Message\":\"Test log entry 5 at 63879164964989 for rate 10000 with date \\\"2025-04-02\\\"\",\"MessageTemplate\":\"Test log entry {LogId} at {TimestampMs} for rate {Rate} with date {LogDate}\",\"LogId\":5,\"TimestampMs\":63879164964989,\"Rate\":10000,\"LogDate\":\"2025-04-02\",\"ThreadId\":10,\"Caller\":\"LokiLogger.Program.RunLogIngestionTest(int)+(?) => { }\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568164989597800",
    //            "{\"Message\":\"Test log entry 4 at 63879164964989 for rate 10000 with date \\\"2025-04-02\\\"\",\"MessageTemplate\":\"Test log entry {LogId} at {TimestampMs} for rate {Rate} with date {LogDate}\",\"LogId\":4,\"TimestampMs\":63879164964989,\"Rate\":10000,\"LogDate\":\"2025-04-02\",\"ThreadId\":11,\"Caller\":\"LokiLogger.Program.RunLogIngestionTest(int)+(?) => { }\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ],
    //          [
    //            "1743568164989597800",
    //            "{\"Message\":\"Test log entry 1 at 63879164964989 for rate 10000 with date \\\"2025-04-02\\\"\",\"MessageTemplate\":\"Test log entry {LogId} at {TimestampMs} for rate {Rate} with date {LogDate}\",\"LogId\":1,\"TimestampMs\":63879164964989,\"Rate\":10000,\"LogDate\":\"2025-04-02\",\"ThreadId\":8,\"Caller\":\"LokiLogger.Program.RunLogIngestionTest(int)+(?) => { }\",\"AssemblyName\":\"LokiLogger\",\"AssemblyVersion\":\"1.0.0.0\",\"AssemblyInformationalVersion\":\"1.0.0\",\"app\":\"loki-test\",\"host\":\"400127-0\",\"environment\":\"development\",\"ApplicationVersion\":\"1.0.0\",\"year\":\"2025\",\"month\":\"04\",\"day\":\"02\",\"date\":\"2025-04-02\",\"level\":\"info\"}",
    //            {
    //              "structuredMetadata": {
    //                "detected_level": "info"
    //              }
    //            }
    //          ]
    //        ]
    //      }
    //    ],
    //    "stats": {
    //      "summary": {
    //        "bytesProcessedPerSecond": 3255586,
    //        "linesProcessedPerSecond": 5485,
    //        "totalBytesProcessed": 13651,
    //        "totalLinesProcessed": 23,
    //        "execTime": 0.004193,
    //        "queueTime": 0,
    //        "subqueries": 0,
    //        "totalEntriesReturned": 20,
    //        "splits": 1,
    //        "shards": 1,
    //        "totalPostFilterLines": 23,
    //        "totalStructuredMetadataBytesProcessed": 690
    //      },
    //      "querier": {
    //        "store": {
    //          "totalChunksRef": 0,
    //          "totalChunksDownloaded": 0,
    //          "chunksDownloadTime": 0,
    //          "queryReferencedStructuredMetadata": false,
    //          "chunk": {
    //            "headChunkBytes": 0,
    //            "headChunkLines": 0,
    //            "decompressedBytes": 0,
    //            "decompressedLines": 0,
    //            "compressedBytes": 0,
    //            "totalDuplicates": 0,
    //            "postFilterLines": 0,
    //            "headChunkStructuredMetadataBytes": 0,
    //            "decompressedStructuredMetadataBytes": 0
    //          },
    //          "chunkRefsFetchTime": 0,
    //          "congestionControlLatency": 0,
    //          "pipelineWrapperFilteredLines": 0
    //        }
    //      },
    //      "ingester": {
    //        "totalReached": 1,
    //        "totalChunksMatched": 0,
    //        "totalBatches": 1,
    //        "totalLinesSent": 20,
    //        "store": {
    //          "totalChunksRef": 2,
    //          "totalChunksDownloaded": 2,
    //          "chunksDownloadTime": 517000,
    //          "queryReferencedStructuredMetadata": false,
    //          "chunk": {
    //            "headChunkBytes": 0,
    //            "headChunkLines": 0,
    //            "decompressedBytes": 13651,
    //            "decompressedLines": 23,
    //            "compressedBytes": 513587,
    //            "totalDuplicates": 0,
    //            "postFilterLines": 23,
    //            "headChunkStructuredMetadataBytes": 0,
    //            "decompressedStructuredMetadataBytes": 690
    //          },
    //          "chunkRefsFetchTime": 0,
    //          "congestionControlLatency": 0,
    //          "pipelineWrapperFilteredLines": 0
    //        }
    //      },
    //      "cache": {
    //        "chunk": {
    //          "entriesFound": 0,
    //          "entriesRequested": 0,
    //          "entriesStored": 0,
    //          "bytesReceived": 0,
    //          "bytesSent": 0,
    //          "requests": 0,
    //          "downloadTime": 0,
    //          "queryLengthServed": 0
    //        },
    //        "index": {
    //          "entriesFound": 0,
    //          "entriesRequested": 0,
    //          "entriesStored": 0,
    //          "bytesReceived": 0,
    //          "bytesSent": 0,
    //          "requests": 0,
    //          "downloadTime": 0,
    //          "queryLengthServed": 0
    //        },
    //        "result": {
    //          "entriesFound": 0,
    //          "entriesRequested": 0,
    //          "entriesStored": 0,
    //          "bytesReceived": 0,
    //          "bytesSent": 0,
    //          "requests": 0,
    //          "downloadTime": 0,
    //          "queryLengthServed": 0
    //        },
    //        "statsResult": {
    //          "entriesFound": 1,
    //          "entriesRequested": 1,
    //          "entriesStored": 0,
    //          "bytesReceived": 414,
    //          "bytesSent": 0,
    //          "requests": 1,
    //          "downloadTime": 0,
    //          "queryLengthServed": 2294000000000
    //        },
    //        "volumeResult": {
    //          "entriesFound": 0,
    //          "entriesRequested": 0,
    //          "entriesStored": 0,
    //          "bytesReceived": 0,
    //          "bytesSent": 0,
    //          "requests": 0,
    //          "downloadTime": 0,
    //          "queryLengthServed": 0
    //        },
    //        "seriesResult": {
    //          "entriesFound": 0,
    //          "entriesRequested": 0,
    //          "entriesStored": 0,
    //          "bytesReceived": 0,
    //          "bytesSent": 0,
    //          "requests": 0,
    //          "downloadTime": 0,
    //          "queryLengthServed": 0
    //        },
    //        "labelResult": {
    //          "entriesFound": 0,
    //          "entriesRequested": 0,
    //          "entriesStored": 0,
    //          "bytesReceived": 0,
    //          "bytesSent": 0,
    //          "requests": 0,
    //          "downloadTime": 0,
    //          "queryLengthServed": 0
    //        },
    //        "instantMetricResult": {
    //          "entriesFound": 0,
    //          "entriesRequested": 0,
    //          "entriesStored": 0,
    //          "bytesReceived": 0,
    //          "bytesSent": 0,
    //          "requests": 0,
    //          "downloadTime": 0,
    //          "queryLengthServed": 0
    //        }
    //      },
    //      "index": {
    //        "totalChunks": 0,
    //        "postFilterChunks": 0,
    //        "shardsDuration": 0,
    //        "usedBloomFilters": false
    //      }
    //    }
    //  }
    //}
    // 

    #endregion

    public class LokiQueryService
    {
        private readonly HttpClient _httpClient;

        #region Constructor

        public LokiQueryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #endregion

        #region immutable data models

        public record LokiQueryRequest(
            string Query,
            int? Limit = 1000,
            DateTimeOffset? Start = null,
            DateTimeOffset? End = null,
            string? Direction = "forward",
            TimeSpan? Step = null,
            TimeSpan? Since = null,
            TimeSpan? Until = null,
            string? StartTs = null
        )
        {
            [JsonIgnore]
            public DateTimeOffset ActualStart => StartTs != null ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(StartTs) / 1_000_000)
                                                                : (Start ?? (Since.HasValue ? DateTimeOffset.Now - Since.Value : DateTimeOffset.Now.AddHours(-1)));

            [JsonIgnore]
            public DateTimeOffset ActualEnd => End ?? (Until.HasValue ? DateTimeOffset.Now - Until.Value : DateTimeOffset.Now);
        }

        public record LokiLogEntry(
            [property: JsonPropertyName("ts")] DateTimeOffset Timestamp,
            [property: JsonPropertyName("line")] string Line,
            [property: JsonPropertyName("labels")] Dictionary<string, string> Labels
        );

        public record LokiQueryResponse(
            [property: JsonPropertyName("status")] string Status,
            [property: JsonPropertyName("data")] LokiResponseData Data,
            [property: JsonPropertyName("error")] string? Error
        );

        public record LokiResponseData(
            [property: JsonPropertyName("resultType")] string ResultType,
            [property: JsonPropertyName("encodingFlags")] List<string>? EncodingFlags,
            [property: JsonPropertyName("result")] List<LokiStreamResult> Results,
            [property: JsonPropertyName("stats")] dynamic Stats
        );

        public record LokiStreamResult(
            [property: JsonPropertyName("stream")] Dictionary<string, string> Stream,
            [property: JsonPropertyName("values")] List<List<string>> Values
        );

        #endregion

        #region Methods

        public async Task<(List<LokiLogEntry> Logs, string? NextPageStartTs, string? Error)> QueryLogsAsync(LokiQueryRequest request)
        {
            try
            {
                var queryParams = new Dictionary<string, string>
                {
                    ["query"] = request.Query,
                    ["limit"] = request.Limit?.ToString() ?? "1000",
                    ["start"] = (request.ActualStart.ToUnixTimeMilliseconds() * 1_000_000).ToString(),
                    ["end"] = (request.ActualEnd.ToUnixTimeMilliseconds() * 1_000_000).ToString(),
                    ["direction"] = request.Direction ?? "forward"
                };

                if (request.Step.HasValue)
                    queryParams["step"] = ((long)request.Step.Value.TotalMilliseconds).ToString();

                var queryString = string.Join("&", queryParams
                    .Where(kvp => kvp.Value != null)
                    .Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));

                var response = await _httpClient.GetAsync($"{AppGlobal.LokiUrl}{AppGlobal.QueryRange_LokiEndpoint}?{queryString}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return (new List<LokiLogEntry>(), null,
                        $"Error {response.StatusCode}: {content}");
                }

                Console.WriteLine($"Raw Loki response: {content.Substring(0, Math.Min(content.Length, 500))}...");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new DateTimeOffsetConverter() }
                };

                var result = JsonSerializer.Deserialize<LokiQueryResponse>(content, options);

                List<LokiLogEntry> logs = result?.Data?.Results
                    ?.SelectMany(stream => stream.Values
                        .Select(value => new LokiLogEntry(
                            Timestamp: DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(value[0]) / 1_000_000),
                            Line: value[1],
                            Labels: stream.Stream
                        )))
                    .ToList() ?? new List<LokiLogEntry>();

                // For pagination, we use the timestamp of the last log as the next start point
                string? nextPageStartTs = null;

                if (logs.Count > 0 && logs.Count >= (request.Limit ?? 1000))
                {
                    // Get the timestamp of the last log entry based on the sort direction
                    var lastLog = request.Direction == "backward"
                        ? logs.First()
                        : logs.Last();

                    // For "forward" direction, use the timestamp of the last log + 1ns for the next query
                    // For "backward" direction, use the timestamp of the first log - 1ns
                    long timestampNs = request.Direction == "backward"
                        ? (lastLog.Timestamp.ToUnixTimeMilliseconds() * 1_000_000) - 1
                        : (lastLog.Timestamp.ToUnixTimeMilliseconds() * 1_000_000) + 1;

                    nextPageStartTs = timestampNs.ToString();
                }

                Console.WriteLine($"Total {logs.Count} Logs Found, Next Start: {nextPageStartTs ?? "None"}");

                return (logs, nextPageStartTs, result?.Error);
            }
            catch (Exception ex)
            {
                return (new List<LokiLogEntry>(), null,
                    $"Exception: {ex.Message}");
            }
        }

        private class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
        {
            public override DateTimeOffset Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) =>
                DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64() / 1_000_000);

            public override void Write(
                Utf8JsonWriter writer,
                DateTimeOffset value,
                JsonSerializerOptions options) =>
                writer.WriteNumberValue((value.ToUnixTimeMilliseconds() * 1_000_000));
        }

        #endregion

    }

}
