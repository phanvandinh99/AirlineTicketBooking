using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AirlineWeb.Common
{
    public static class Logger
    {
        private static readonly object _lockObject = new object();
        private static readonly StringBuilder _logBuffer = new StringBuilder();
        private static readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static DateTime _lastCleanupDate = DateTime.MinValue;
        private static readonly Task _completedTask = Task.CompletedTask;

        // Cache cho method info để tránh tính toán lại
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _methodInfoCache =
            new System.Collections.Concurrent.ConcurrentDictionary<string, string>();

        #region Synchronous Methods (for backward compatibility)
        /// <summary>
        /// Log thông tin thông thường (synchronous)
        /// </summary>
        public static void Info(string message) => LogToFile("INFO", GetCallingMethodInfo(), message);

        /// <summary>
        /// Log thành công (synchronous)
        /// </summary>
        public static void Success(string message) => LogToFile("SUCCESS", GetCallingMethodInfo(), message);

        /// <summary>
        /// Log cảnh báo (synchronous)
        /// </summary>
        public static void Warning(string message) => LogToFile("WARNING", GetCallingMethodInfo(), message);

        /// <summary>
        /// Log lỗi (synchronous)
        /// </summary>
        public static void Error(string message) => LogToFile("ERROR", GetCallingMethodInfo(), message);

        /// <summary>
        /// Log lỗi với Exception (synchronous)
        /// </summary>
        public static void Error(Exception ex)
        {
            var methodInfo = GetCallingMethodInfo();
            LogToFile("ERROR", methodInfo, ex.Message);
            LogToFile("ERROR", methodInfo, $"StackTrace: {ex.StackTrace}");
        }

        /// <summary>
        /// Log lỗi với Exception và message tùy chỉnh (synchronous)
        /// </summary>
        public static void Error(string message, Exception ex)
        {
            var methodInfo = GetCallingMethodInfo();
            LogToFile("ERROR", methodInfo, $"{message} - {ex.Message}");
            LogToFile("ERROR", methodInfo, $"StackTrace: {ex.StackTrace}");
        }
        #endregion

        #region Asynchronous Methods (for better performance)
        /// <summary>
        /// Log thông tin thông thường (asynchronous)
        /// </summary>
        public static Task InfoAsync(string message) => LogToFileAsync("INFO", GetCallingMethodInfo(), message);

        /// <summary>
        /// Log thành công (asynchronous)
        /// </summary>
        public static Task SuccessAsync(string message) => LogToFileAsync("SUCCESS", GetCallingMethodInfo(), message);

        /// <summary>
        /// Log cảnh báo (asynchronous)
        /// </summary>
        public static Task WarningAsync(string message) => LogToFileAsync("WARNING", GetCallingMethodInfo(), message);

        /// <summary>
        /// Log lỗi (asynchronous)
        /// </summary>
        public static Task ErrorAsync(string message) => LogToFileAsync("ERROR", GetCallingMethodInfo(), message);

        /// <summary>
        /// Log lỗi với Exception (asynchronous)
        /// </summary>
        public static async Task ErrorAsync(Exception ex)
        {
            var methodInfo = GetCallingMethodInfo();
            await LogToFileAsync("ERROR", methodInfo, ex.Message);
            await LogToFileAsync("ERROR", methodInfo, $"StackTrace: {ex.StackTrace}");
        }

        /// <summary>
        /// Log lỗi với Exception và message tùy chỉnh (asynchronous)
        /// </summary>
        public static async Task ErrorAsync(string message, Exception ex)
        {
            var methodInfo = GetCallingMethodInfo();
            await LogToFileAsync("ERROR", methodInfo, $"{message} - {ex.Message}");
            await LogToFileAsync("ERROR", methodInfo, $"StackTrace: {ex.StackTrace}");
        }
        #endregion

        /// <summary>
        /// Lấy thông tin method và controller đang gọi (tối ưu với cache)
        /// </summary>
        private static string GetCallingMethodInfo()
        {
            try
            {
                var stackTrace = new StackTrace();

                // Tìm method gọi Logger (bỏ qua các method trong Logger class)
                for (int i = 1; i < stackTrace.FrameCount; i++)
                {
                    var frame = stackTrace.GetFrame(i);
                    var method = frame?.GetMethod();

                    if (method?.DeclaringType?.FullName?.Contains("AirlineWeb.Common.Logger") != true)
                    {
                        var declaringType = method.DeclaringType;
                        if (declaringType == null) continue;

                        var cacheKey = $"{declaringType.FullName}.{method.Name}";

                        return _methodInfoCache.GetOrAdd(cacheKey, key =>
                        {
                            var controllerName = declaringType.Name;
                            var methodName = method.Name;

                            // Loại bỏ "Controller" suffix nếu có
                            if (controllerName.EndsWith("Controller"))
                            {
                                controllerName = controllerName.Substring(0, controllerName.Length - 10);
                            }

                            return $"{controllerName}/{methodName}";
                        });
                    }
                }

                return "Unknown/Unknown";
            }
            catch
            {
                return "Unknown/Unknown";
            }
        }

        private static void LogToFile(string level, string methodInfo, string message)
        {
            try
            {
                // Tạo log entry với string interpolation (nhanh hơn string.Format)
                var now = DateTime.Now;
                var logEntry = $"[{now:yyyy-MM-dd HH:mm:ss}] [{level}] [{methodInfo}] {message}{Environment.NewLine}";

                // Buffer log entry
                lock (_lockObject)
                {
                    _logBuffer.Append(logEntry);

                    // Flush buffer khi đủ lớn hoặc sau mỗi 10 entries
                    if (_logBuffer.Length > 4096 || _logBuffer.ToString().Split('\n').Length > 10)
                    {
                        FlushLogBuffer(now);
                    }
                }
            }
            catch
            {
                // Fallback: sử dụng Debug.WriteLine nếu không ghi được file
                Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{methodInfo}] {message}");
            }
        }

        private static async Task LogToFileAsync(string level, string methodInfo, string message)
        {
            try
            {
                // Tạo log entry với string interpolation
                var now = DateTime.Now;
                var logEntry = $"[{now:yyyy-MM-dd HH:mm:ss}] [{level}] [{methodInfo}] {message}{Environment.NewLine}";

                // Buffer log entry (async-safe)
                await Task.Run(() =>
                {
                    lock (_lockObject)
                    {
                        _logBuffer.Append(logEntry);

                        // Flush buffer khi đủ lớn hoặc sau mỗi 10 entries
                        if (_logBuffer.Length > 4096 || _logBuffer.ToString().Split('\n').Length > 10)
                        {
                            FlushLogBufferAsync(now).ConfigureAwait(false);
                        }
                    }
                });
            }
            catch
            {
                // Fallback: sử dụng Debug.WriteLine nếu không ghi được file
                Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{methodInfo}] {message}");
            }
        }

        private static void FlushLogBuffer(DateTime now)
        {
            try
            {
                // Tạo thư mục nếu chưa có (chỉ check 1 lần)
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }

                // Cleanup cũ (chỉ chạy 1 lần mỗi ngày)
                if (now.Date > _lastCleanupDate.Date)
                {
                    CleanupOldLogs();
                    _lastCleanupDate = now;
                }

                // Ghi file
                var today = now.ToString("yyyy-MM-dd");
                var logFile = Path.Combine(_logDirectory, $"log_{today}.txt");

                File.AppendAllText(logFile, _logBuffer.ToString());
                _logBuffer.Clear();
            }
            catch
            {
                // Nếu lỗi, clear buffer để tránh memory leak
                _logBuffer.Clear();
            }
        }

        private static async Task FlushLogBufferAsync(DateTime now)
        {
            try
            {
                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }

                // Cleanup cũ (chỉ chạy 1 lần mỗi ngày)
                if (now.Date > _lastCleanupDate.Date)
                {
                    await Task.Run(() => CleanupOldLogs());
                    _lastCleanupDate = now;
                }

                // Ghi file async
                var today = now.ToString("yyyy-MM-dd");
                var logFile = Path.Combine(_logDirectory, $"log_{today}.txt");

                await File.AppendAllTextAsync(logFile, _logBuffer.ToString());
                _logBuffer.Clear();
            }
            catch
            {
                // Nếu lỗi, clear buffer để tránh memory leak
                _logBuffer.Clear();
            }
        }

        private static void CleanupOldLogs()
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-15);
                var logFiles = Directory.GetFiles(_logDirectory, "log_*.txt");

                foreach (var file in logFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Bỏ qua lỗi cleanup để không ảnh hưởng đến logging chính
            }
        }

        /// <summary>
        /// Force flush buffer (synchronous)
        /// </summary>
        public static void Flush()
        {
            lock (_lockObject)
            {
                if (_logBuffer.Length > 0)
                {
                    FlushLogBuffer(DateTime.Now);
                }
            }
        }

        /// <summary>
        /// Force flush buffer (asynchronous)
        /// </summary>
        public static async Task FlushAsync()
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    if (_logBuffer.Length > 0)
                    {
                        FlushLogBufferAsync(DateTime.Now).ConfigureAwait(false);
                    }
                }
            });
        }
    }
}