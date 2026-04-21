using EasyLog;

namespace EasySave.Patterns.Observer
{
    // Defines the contract for observers listening to real-time backup state changes.
    // Allows decoupled components (like the Logger) to react when a job's progress updates.
    public interface IBackupObserver
    {
        void Update(StateEntry state);
    }
}