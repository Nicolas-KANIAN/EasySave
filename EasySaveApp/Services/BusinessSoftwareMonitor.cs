using System.Diagnostics;

namespace EasySave.Services
{
    public class BusinessSoftwareMonitor
    {
        private string _softwareName = string.Empty;
        private bool _isCurrentlyRunning = false;
        private CancellationTokenSource? _cts;

        public event EventHandler? SoftwareStarted;
        public event EventHandler? SoftwareStopped;

        public bool IsRunning => _isCurrentlyRunning;

        public void SetSoftwareName(string name)
        {
            _softwareName = name?.Replace(".exe", "", StringComparison.OrdinalIgnoreCase) ?? string.Empty;
        }

        public void Start()
        {
            if (_cts != null) return;
            _cts = new CancellationTokenSource();

            Task.Run(() => MonitorLoop(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts = null;
        }

        private async Task MonitorLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!string.IsNullOrEmpty(_softwareName))
                {
                    bool exists = Process.GetProcessesByName(_softwareName).Length > 0;

                    if (exists && !_isCurrentlyRunning)
                    {
                        _isCurrentlyRunning = true;
                        SoftwareStarted?.Invoke(this, EventArgs.Empty);
                    }
                    else if (!exists && _isCurrentlyRunning)
                    {
                        _isCurrentlyRunning = false;
                        SoftwareStopped?.Invoke(this, EventArgs.Empty);
                    }
                }

                await Task.Delay(2000, token);
            }
        }
    }
}