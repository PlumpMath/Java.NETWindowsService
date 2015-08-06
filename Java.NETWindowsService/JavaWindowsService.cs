using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using Java.NETWindowsService.ProcessHost;
using NLog;

namespace Java.NETWindowsService
{
    public partial class JavaWindowsService : ServiceBase
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static int _javaProcessId;

        public JavaWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var processStartArguments = GetArgsFromConfig();
            _logger.Info("Starting with args: {0}", processStartArguments);
            try
            {
                var javaProcess = new JavaProcess(processStartArguments);
                javaProcess.OnMessage += OnMessage;
                javaProcess.OnExit += OnExited;
                _javaProcessId = javaProcess.Start();
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                throw;
            }
        }

        private string GetArgsFromConfig()
        {
            var startArgumentsBuilder = new StringBuilder();
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                startArgumentsBuilder.Append(" " + ConfigurationManager.AppSettings[key]);
            }
            return startArgumentsBuilder.ToString();
        }

        private void OnMessage(string message)
        {
            _logger.Info("JVM: {0}", message);
        }

        private void OnExited(int exitCode)
        {
            _logger.Error("Java process stopped, exit code:" + exitCode);
        }

        protected override void OnStop()
        {
            _logger.Info("Stop");
            try
            {
                StopProcess();
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
            }
        }

        private void StopProcess()
        {
            var processById = Process.GetProcessById(_javaProcessId);
            processById.Kill();
            processById.WaitForExit();
        }
    }
}
