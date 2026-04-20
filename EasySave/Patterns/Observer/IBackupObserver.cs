using EasyLog;

namespace EasySave.Patterns.Observer
{
    public interface IBackupObserver
    {
        void Update(StateEntry state);
    }
}