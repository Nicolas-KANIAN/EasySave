using EasyLog;

namespace EasySave.Patterns.Observer
{
    // Concrete observer that listens for real-time backup state changes.
    // Forwards the updated state to the Logger to keep the state.json file synchronized.
    public class StateLoggerObserver : IBackupObserver
    {
        public void Update(StateEntry state)
        {
            Logger.Instance.UpdateState(new List<StateEntry> { state });
        }
    }
}