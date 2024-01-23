namespace CMS.Api.Listeners
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using Microsoft.Extensions.Configuration;

    public sealed class DailyTraceListener : TraceListener
    {
        public DailyTraceListener(IConfiguration configuration) => this.LogFolder = configuration.GetValue<string>("Directories:ErrorLogs");

        private bool UseUtcTime { get; set; }

        private string LogFolder { get; set; }

        private bool Disposed { get; set; }

        private bool HasHeader { get; set; }

        private string CurrentLogFilePath { get; set; }

        private DateTime? CurrentLogDate { get; set; }

        private FileStream LogFileStream { get; set; }

        private StreamWriter LogFileWriter { get; set; }

        private SemaphoreSlim LogLocker { get; set; } = new SemaphoreSlim(1, 1);

        public DailyTraceListener UseUtc()
        {
            this.UseUtcTime = false;
            return this;
        }

        public DailyTraceListener UseHeader()
        {
            this.HasHeader = true;
            return this;
        }

        public void Invoke(string from, string route, string message, string description)
        {
            var time = FormatTime(this.GetCurrentTime());
            this.WriteLine($"{time},{EscapeCsv(from)},{EscapeCsv(route)},{EscapeCsv(message)},{EscapeCsv(description)}");
        }

        public override void Write(string message)
        {
            try
            {
                this.LogLocker.Wait();

                this.CheckDisposed();
                this.CheckFile();

                this.LogFileWriter.Write(message);
                this.LogFileWriter.Flush();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
            finally
            {
                this.LogLocker.Release();
            }
        }

        public override void WriteLine(string message)
        {
            this.Write(message + Environment.NewLine);
        }

        protected override void Dispose(bool disposing)
        {
            this.Disposed = true;

            try
            {
                this.LogFileWriter?.Dispose();
                this.LogFileStream?.Dispose();
                this.LogLocker.Dispose();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            base.Dispose(disposing);
        }

        private static string EscapeCsv(string input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] != ',' && input[i] != '\n')
                {
                    continue;
                }

                input = input.Replace("\"", "\"\"");
                return $"\"{input}\"";
            }

            return input;
        }

        private static string FormatTime(DateTime time) => time.ToString("o");

        private void WriteHeader()
        {
            this.LogFileWriter.WriteLine("Time,From,Router,Message,Description");
        }

        private DateTime GetCurrentTime() => this.UseUtcTime ? DateTime.UtcNow : DateTime.Now;

        private void InitializeLogFile()
        {
            this.CheckDisposed();

            try
            {
                this.LogFileWriter?.Dispose();

                if (this.LogFileStream != null)
                {
                    var logFileWriter = this.LogFileWriter;
                    logFileWriter?.Dispose();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            var currentTime = this.GetCurrentTime();

            var fileName = $"{currentTime:yyyy-MM-dd}.log";
            this.CurrentLogFilePath = Path.Combine(this.LogFolder, fileName);

            // Ensure the folder is there
            Directory.CreateDirectory(this.LogFolder);

            // Create/Open log file
            this.LogFileStream = new FileStream(this.CurrentLogFilePath, FileMode.Append);
            this.LogFileWriter = new StreamWriter(this.LogFileStream);

            // Write Header if needed
            if (this.LogFileStream.Length == 0 && this.HasHeader)
            {
                this.WriteHeader();
            }
        }

        private void CheckFile()
        {
            this.CheckDisposed();

            var currentTime = this.GetCurrentTime();
            if (this.CurrentLogDate != null && currentTime.Date == this.CurrentLogDate)
            {
                return;
            }

            this.InitializeLogFile();
            this.CurrentLogDate = currentTime.Date;
        }

        private void CheckDisposed()
        {
            if (this.Disposed)
            {
                throw new InvalidOperationException("The Trace Listener is Disposed.");
            }
        }
    }
}
