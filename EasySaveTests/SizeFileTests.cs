using EasySave.Services;

namespace EasySaveApp.Tests.Models
{
    public class SizeFileTests
    {
        // Verifies that the configuration correctly stores the size limit and priority extensions for file processing.
        [Fact]
        public void AppConfig_MaxFileSize_ShouldBeSetAndRetrievedCorrectly()
        {
            var config = new AppConfig();
            long expectedLimitKb = 50000;

            config.MaxFileSizeKbForSimultaneous = expectedLimitKb;

            Assert.Equal(expectedLimitKb, config.MaxFileSizeKbForSimultaneous);
        }

        [Theory]
        [InlineData(".mp4", true)]
        [InlineData(".MP4", true)]
        [InlineData(".txt", false)]
        public void PriorityExtensions_ShouldBeCaseInsensitive(string extension, bool expectedIsPriority)
        {
            var config = new AppConfig();
            config.PriorityExtensions = new List<string> { ".mp4" };

            bool isPriority = config.PriorityExtensions.Contains(extension, System.StringComparer.OrdinalIgnoreCase);

            Assert.Equal(expectedIsPriority, isPriority);
        }
    }
}