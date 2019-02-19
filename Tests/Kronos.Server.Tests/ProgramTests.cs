using System;
using System.Threading.Tasks;
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

            var server = Task.Run(() => Program.Start(args));

            // Act
            Exception ex = await Record.ExceptionAsync(async () =>
            {
                while (!Program.IsWorking)
                {
                    await Task.Delay(100);
                }

                Program.Stop();

                await server;
            });


            // Assert
            Assert.Null(ex);
        }
    }
}
