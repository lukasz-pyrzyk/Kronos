using System;
using System.IO;
using System.Text;
using Kronos.Client.Core;

namespace Kronos.Client
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            IKronosClient client = KronosClientFactory.CreateClient();
            string key = "siema";
            byte[] package = Encoding.UTF8.GetBytes($"siema {sizeof(long)} awfapowfjapw fapwof jpawofjawpo fjapwo fjapwofhapowfapowfj paowhf aowpfoahwowpof jap");
            DateTime expiryDate = new DateTime();

            client.InsertToServer(key, package, expiryDate);
        }
    }
}
