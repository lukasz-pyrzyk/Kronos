using System;
using System.Threading.Tasks;
using Kronos.Client;
using Kronos.Core.Exceptions;
using Kronos.Server;
using Xunit;

namespace Kronos.AcceptanceTest
{
    [Collection("AcceptanceTest")]
    public class Authorization : Base
    {
        [Fact]
        public override async Task RunAsync()
        {
            await RunInternalAsync();
        }

        protected override async Task ProcessAsync(IKronosClient client)
        {
            // Act
            Exception ex = await Record.ExceptionAsync(() => client.CountAsync());

            Assert.IsType<KronosException>(ex);
        }

        protected override CliArguments GetSettings()
        {
            var currentSettings = base.GetSettings();
            currentSettings.Password = "Super secret admin password";
            return currentSettings;
        }
    }
}
