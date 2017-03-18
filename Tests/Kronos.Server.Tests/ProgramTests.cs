using System;
using System.Threading.Tasks;
using NLog.Config;
using Xunit;

namespace Kronos.Server.Tests
{
    public class ProgramTests
    {
        [Fact]
        public async Task CanStartAndStop()
        {
            // Arrange
            var args = new SettingsArgs();

            // Act
            Exception ex = await Record.ExceptionAsync(async () =>
            {
                Task t = Task.Run(() => Program.Start(args, new LoggingConfiguration()));

                while (!Program.IsWorking)
                {
                    await Task.Delay(100);
                }

                Program.Stop();
            });

            // Assert
            Assert.Null(ex);
        }
    }
}
