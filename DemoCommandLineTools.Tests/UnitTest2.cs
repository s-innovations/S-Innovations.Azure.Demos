using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SInnovations.ConfigurationManager;
using SInnovations.ConfigurationManager.Providers;

namespace DemoCommandLineTools.Tests
{
    [TestClass]
    public class UnitTest2
    {
        public static string DecryptEnvelop(string base64EncryptedString)
        {
            var encryptedBytes = Convert.FromBase64String(base64EncryptedString);
            var envelope = new EnvelopedCms();
            envelope.Decode(encryptedBytes);
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            envelope.Decrypt(store.Certificates);
            return Encoding.UTF8.GetString(envelope.ContentInfo.Content);
        }

        [TestMethod]
        public void TestMethod1()
        {

            var configurationManager = new ConfigurationManager(new AppSettingsProvider());
            configurationManager.UseAzureKeyVault(
                new AzureKeyVaultSettingsProviderOptions
                {
                    ConfigurationManager = configurationManager,
                    SecretConverter = DecryptEnvelop
                });
            
            var test = configurationManager.GetAzureKeyVaultSecret("storage");
            Console.WriteLine(test.Value);

        }

      
    }
}
