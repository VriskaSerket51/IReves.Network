using System;
using IReves.Network.Performance.Encryption.Aes;

namespace IReves.Network.Performance
{
    internal class Program
    {
        public static void Main()
        {
            try
            {
                new NetAESEncryptionPerformanceTests().Run();
            } catch
            {
                throw;
            }
        }
    }
}
