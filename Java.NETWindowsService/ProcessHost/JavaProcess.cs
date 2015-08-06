using System;
using System.Diagnostics;
using System.IO;
using Java.NETWindowsService.Exceptions;

namespace Java.NETWindowsService.ProcessHost
{
    public class JavaProcess
    {
        private readonly ProcessStartInfo _startInfo;

        public event Action<string> OnMessage;
        public event Action<int> OnExit;

        public JavaProcess(string processStartArguments)
        {
            var javaPath = GetJavaPath(Environment.GetEnvironmentVariable("JAVA_HOME"));

            _startInfo = new ProcessStartInfo(javaPath, processStartArguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };
        }

        private string GetJavaPath(string javaHomePathEnvironmentVariable)
        {
            var javaPath = string.Format(@"{0}\bin\java.exe", javaHomePathEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(javaHomePathEnvironmentVariable))
            {
                throw new JavaException(
                    "Error: JAVA_HOME is set to an invalid directory. JAVA_HOME = '%JAVA_HOME%' Please set the JAVA_HOME variable in your environment to match the location of your Java installation.");
            }
            if (!File.Exists(javaPath))
            {
                throw new JavaException(
                    "Error: Please confirm you have a valid Java installation. And please set the JAVA_HOME = '%JAVA_HOME%' variable in your environment to match the location of your Java installation. ");
            }

            return javaPath;
        }

        public int Start()
        {
            var process = new Process { StartInfo = _startInfo };

            process.OutputDataReceived += RaiseMessageEvent;
            process.ErrorDataReceived += RaiseMessageEvent;
            process.Exited += (sender, e) =>
            {
                if (OnExit != null) OnExit(process.ExitCode);
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return process.Id;
        }

        private void RaiseMessageEvent(object sender, DataReceivedEventArgs e)
        {
            if (OnMessage != null) OnMessage(e.Data);
        }
    }
}
