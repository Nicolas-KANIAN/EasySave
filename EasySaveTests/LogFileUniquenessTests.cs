using EasyLog;

namespace EasySaveApp.Tests.EasyLogTests
{
    public class LogFileUniquenessTests
    {
        // Verifies the Singleton pattern implementation of the Logger. 
        // This is crucial to prevent file access conflicts when multiple backup jobs try to write logs at the exact same time.
        [Fact]
        public void LoggerInstance_ShouldBeASingleton_AndReturnSameObject()
        {
            var logger1 = Logger.Instance;
            var logger2 = Logger.Instance;

            Assert.Same(logger1, logger2);
        }
    }
}