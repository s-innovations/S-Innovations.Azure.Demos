using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoCommandLineTools.Tests
{
    /// <summary>
    /// Make a devopp application on your ad tenant or use redirect/clientid here.
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Find your tenant name and set it here for the user you plan to use. You may use the common endpoint but do know that there can be some issues wiht liveids
        /// </summary>
        public string TenantName { get; } = "car2cloud.onmicrosoft.com";  //This is what I spend the most time debugging - common vs specific endpoints for liveids.

        /// <summary>
        /// Create a Native Application in your tenant that will serv as your dev opp application that will delegate acesse as the signed in user.
        /// 
        /// Copy the clientID here
        /// </summary>
        public string ClientId { get; } = "22a1932b-342b-4571-a037-2d84c15a0e6c";

        /// <summary>
        /// Also copy the redirect uri;
        /// </summary>
        public string RedirectUri { get; } = "https://test.car2cloud.dk";

        /// <summary>
        /// Step One : Run this to get your subscription Ids and read the output of the test.
        /// </summary>
        [TestMethod]
        public void ListSubscriptions()
        {
            Program.Main(new[] { "-c", ClientId, "-u", RedirectUri, "-t", TenantName,"--listsubscriptions" });
        }

        /// <summary>
        /// Run ListSubscriptions and from output read the subscriptionid of choose.
        /// </summary>
        public string SubscriptionId = "6d8083fe-97c5-4f4d-b5d4-cc4bf001bfcb";

        /// <summary>
        /// Step Two: Run this to see if you have any vaults
        /// </summary>
        [TestMethod]
        public void ListVaults()
        {
            Program.Main(new[] { "-c", ClientId, "-u", RedirectUri, "-s", SubscriptionId, "-t", TenantName , "--listvaults", });
        }

        string ResourceGroupName{ get; set; } = "another-demo-rg";
        string Location { get; } = "West Europe";

        /// <summary>
        /// Heres the code to create a keyvault, but there are also many othr options. Continue to next unittest
        /// 
        /// It will create it in a resource group and give the authenticated user and this applicaton acess to all secrets and keys
        /// </summary>
        [TestMethod]
        public void CreateVault()
        {
         
            var vaultName = "cnugdemovaulttemp";
            var appId = ClientId;   //We are giving the devop native application acesss but it do not make sense, the application do
                                    //not have private keys and can only delegate accesss on behaf of users using it.
            
            Program.Main(new[] { "-c", ClientId, "-u", RedirectUri, "-s", SubscriptionId, "-t", TenantName, "-r", ResourceGroupName,
                "--location", Location, "--AllowUpdate", "--CreateVault", vaultName,"--applicationId", appId});

        }

        /// <summary>
        /// Add an application to a keyvault based on vaultName and application id.
        /// </summary>
        [TestMethod]
        public void AddApplicationToVault()
        {
            var vaultName = "cnugdemovaulttemp";
            var appId = "fb4fbba5-6fac-404e-b7b6-d7bf4b56a7ea";// "9b93480d-8263-4e97-9e35-0fd857859d0b";
           
            Program.Main(new[] { "-c", ClientId, "-u", RedirectUri, "-s", SubscriptionId, "-t", TenantName, "-r", ResourceGroupName,
                "--AddApplication", appId, "--Vault", vaultName });

        }

        /// <summary>
        /// The master unittest that do cool stuff like deploying stuff.
        /// </summary>
        [TestMethod]
        public void Deploy()
        {

            var template = @"..\..\..\src\AzureKeyVaultDemo.Deploy\Templates\WebSite.json";
            var websiteName = ResourceGroupName + "-website";
            var websitePlan = "Free";
            var StorageAccountType = "Standard_LRS";
            var hostingPlanName = ResourceGroupName + "-plan";
            var deployName = ResourceGroupName + "-deployment";
            var appId = "fb4fbba5-6fac-404e-b7b6-d7bf4b56a7ea";

            Program.Main(new[] {"-c", ClientId, "-u", RedirectUri, "-s", SubscriptionId, "-t", TenantName, "-r", ResourceGroupName,
                "--location", Location, "--deploy", template,"--deployName", deployName,
                "--siteName", websiteName,"--websitePlan",websitePlan,"--storageAccountType",StorageAccountType,
                "--hostingPlanName",hostingPlanName, "--applicationId",appId});
            
        }

        /// <summary>
        /// Read the output of the deployment unittest above to get keyvault and use for next steps.
        /// </summary>
        string VaultName { get; set; } = "q37pmne6oa5lu";
        
        /// <summary>
        /// We can create a certificate and store it as a secret in the keyvault
        /// </summary>
        [TestMethod]
        public void CreateCert()
        {
          
            var certname = "azure-keyvault-demo";
          
            Program.Main(new[] { "-c", ClientId, "-u", RedirectUri, "-s", SubscriptionId, "-t", TenantName, "-r", ResourceGroupName ,
                "--vault", VaultName, "--MakeCert", "--CertificateName", certname});

        }
        /// <summary>
        /// We can export it to file system if we really need it, but hey lets try to not store these things on your laptops using a empty password :)
        /// </summary>
        [TestMethod]
        public void ExportCert()
        {

      
            var certname = "azure-keyvault-demo";
            var location = "c:\\dev\\" + certname + ".pfx";

            Program.Main(new[] { "-c", ClientId, "-u", RedirectUri, "-s", SubscriptionId, "-t", TenantName, "-r", ResourceGroupName,
                "--ExportCert", certname, "--vault", VaultName, "-o", location });

        }

        /// <summary>
        /// We can encrypt a value with the just stored secret
        /// </summary>
        [TestMethod]
        public void EncryptValue()
        {
          
            var certname = "azure-keyvault-demo";           
            var value = "mysecretkey";

            Program.Main(new[] { "-c", ClientId, "-u", RedirectUri, "-s", SubscriptionId, "-t", TenantName, "-r", ResourceGroupName,
                "--vault", VaultName, "--CertificateName", certname, "--encrypt", value });

            

        }
        /// <summary>
        /// And decrypt it again.
        /// </summary>
        [TestMethod]
        public void DecryptValue()
        {

            var certname = "azure-keyvault-demo";
            var value = "MIIBnQYJKoZIhvcNAQcDoIIBjjCCAYoCAQAxggFOMIIBSgIBADAyMB4xHDAaBgNVBAMTE2F6dXJlLWtleXZhdWx0LWRlbW8CEGIxLhjl5HqbRtQy8tlihOowDQYJKoZIhvcNAQEBBQAEggEArivsdt/048LhS1f3UCAXrIf546ToJTFYoF4baoTasR/qSTxeXJX6XrzvWt8ymz261vjSMg0Wh1ZrCmQayDYLwdZ1wNXTPWayOlAcdnrFkWmtNMpvJ4Mh7QDBTm7LY0lnHvoZfwEBio13s5ob8lzCfXMLEAZGxV/9RpHWTUS3fE0ikS3JFTmMdO2QyXZYDdH7wC+7J2xdN8/HTneC+tWeAJO5sBKo4DIfZ8+hV+WPxQjMPtxV2eVTpHn/uTdMA4AsRgTnGeR84sksApx1BA9n0ker5RdF8G3Y0plgwYSrnPaE3C88tAhV9ZiKfGjCQXIjVP2twx5LaS3pued8BjUdSzAzBgkqhkiG9w0BBwEwFAYIKoZIhvcNAwcECBnM40ttbTyIgBB8ofdtyf9HqIueHRftvzfK";



            Program.Main(new[] { "-c", ClientId, "-u", RedirectUri, "-s", SubscriptionId, "-t", TenantName, "-r", ResourceGroupName,
                "--vault", VaultName, "--CertificateName", certname, "--decrypt", value });


        }


    }
}
