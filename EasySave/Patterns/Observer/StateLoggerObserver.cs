using System.Collections.Generic;
using EasyLog;

namespace EasySave.Patterns.Observer
{
    public class StateLoggerObserver : IBackupObserver
    {
        public void Update(StateEntry state)
        {
            Logger.Instance.UpdateState(new List<StateEntry> { state });
        }
    }
}