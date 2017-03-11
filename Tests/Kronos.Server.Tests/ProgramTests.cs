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
            bool wasWorking = false;

            // Act
            Exception ex = await Record.ExceptionAsync(async () =>
            {
                Task t = Task.Run(() => Program.Start(args, new LoggingConfiguration()));

                await Task.Delay(500);
                wasWorking = Program.IsWorking;

                Program.Stop();
                await t;
            });

            // Assert
            Assert.True(wasWorking);
            Assert.False(Program.IsWorking);
            Assert.Null(ex);
        }
    }
}
