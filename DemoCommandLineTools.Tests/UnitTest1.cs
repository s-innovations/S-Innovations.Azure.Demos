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
        /// Step One : Run this to get your subscription Ids and read the output of the test.
        /// </summary>
        [TestMethod]
        public void ListSubscriptions()
        {
            var clientId = "dedd37c0-c2c2-40fe-b1db-0b0c21d4b55a";
            var redirectUri = "https://devops.car2cloud.dk";
            var tenantName = "car2cloud.onmicrosoft.com";  //This is what I spend the most time debugging - common vs specific endpoints for liveids.

            Program.Main(new[] { "-c", clientId, "-u", redirectUri, "--listsubscriptions", "-t", tenantName });
        }

        /// <summary>
        /// Step Two: Run this to see if you have any vaults
        /// </summary>
        [TestMethod]
        public void ListVaults()
        {
            var clientId = "dedd37c0-c2c2-40fe-b1db-0b0c21d4b55a";
            var redirectUri = "https://devops.car2cloud.dk";            
            var subscriptionId = "6d8083fe-97c5-4f4d-b5d4-cc4bf001bfcb";
            var tenantName = "car2cloud.onmicrosoft.com"; 
            Program.Main(new[] { "-c", clientId, "-u", redirectUri, "--listvaults", "-s", subscriptionId, "-t" , tenantName });
        }

        /// <summary>
        /// Heres the code to create a keyvault, but there are also many othr options. Continue to next unittest
        /// 
        /// It will create it in a resource group and give the authenticated user and this applicaton acess to all secrets and keys
        /// </summary>
        [TestMethod]
        public void CreateVault()
        {
            var clientId = "dedd37c0-c2c2-40fe-b1db-0b0c21d4b55a";
            var redirectUri = "https://devops.car2cloud.dk";
            var subscriptionId = "6d8083fe-97c5-4f4d-b5d4-cc4bf001bfcb";
            var resourcegroup = "azure-keyvault-demo-temp";
            var vaultName = "cnugdemovaulttemp";
            var location = "West Europe"; //if i creates the resource group

            var tenantName = "car2cloud.onmicrosoft.com";
            Program.Main(new[] { "-c", clientId, "-u", redirectUri, "-r", resourcegroup, "--AllowUpdate", "--CreateVault", vaultName, "-s", subscriptionId, "-t", tenantName, "--location", location });

        }

        /// <summary>
        /// This is good for giving a azure ad application accesss to a keyvault, continue to next unittes.
        /// </summary>
        [TestMethod]
        public void AddApplicationToVault()
        {
            var clientId = "dedd37c0-c2c2-40fe-b1db-0b0c21d4b55a";
            var redirectUri = "https://devops.car2cloud.dk";
            var tenantName = "car2cloud.onmicrosoft.com";

            var vaultName = "vhsq33ko36y6k";
            var appId = "fb4fbba5-6fac-404e-b7b6-d7bf4b56a7ea";// "9b93480d-8263-4e97-9e35-0fd857859d0b";
            var subscriptionId = "6d8083fe-97c5-4f4d-b5d4-cc4bf001bfcb";
            var resourcegroup = "azure-keyvaut-demo";

            Program.Main(new[] { "-c", clientId, "-u", redirectUri, "-r", resourcegroup, "-s", subscriptionId, "--AddApplication", appId, "--Vault", vaultName, "-t", tenantName });

        }

        /// <summary>
        /// The master unittest that do cool stuff like deploying stuff.
        /// </summary>
        [TestMethod]
        public void Deploy()
        {
           
           

            var clientId = "dedd37c0-c2c2-40fe-b1db-0b0c21d4b55a";
            var redirectUri = "https://devops.car2cloud.dk";

            var tenantName = "car2cloud.onmicrosoft.com";
            var subscriptionId = "6d8083fe-97c5-4f4d-b5d4-cc4bf001bfcb";

            var resourcegroup = "azure-keyvault-demo";
            var location = "West Europe";
            var template = @"..\..\..\src\AzureKeyVaultDemo.Deploy\Templates\WebSite.json";
            var websiteName = "cnug-demo-azurekeyvault";
            var websitePlan = "Free";
            var StorageAccountType = "Standard_LRS";
            var hostingPlanName = "cnug-demo-azurekeyvault";
            var deployName = "mydeployment";
            var appId = "fb4fbba5-6fac-404e-b7b6-d7bf4b56a7ea";
            




            Program.Main(new[] { "-c", clientId, "-u", redirectUri, "-r", resourcegroup, "-s", subscriptionId, "-t",tenantName, "--location", location,
                "--deploy", template,"--deployName", deployName, "--siteName", websiteName,"--websitePlan",websitePlan,"--storageAccountType",StorageAccountType,"--hostingPlanName",hostingPlanName, "--applicationId",appId});

           

        }

        /// <summary>
        /// We can create a certificate and store it as a secret in the keyvault
        /// </summary>
        [TestMethod]
        public void CreateCert()
        {
            var clientId = "dedd37c0-c2c2-40fe-b1db-0b0c21d4b55a";
            var redirectUri = "https://devops.car2cloud.dk";

            var tenantName = "car2cloud.onmicrosoft.com";
            var subscriptionId = "6d8083fe-97c5-4f4d-b5d4-cc4bf001bfcb";

            var certname = "azure-keyvault-demo";
            var resourcegroup = "azure-keyvault-demo";
            var vaultName = "vwzdgctm7ywb6";

            Program.Main(new[] { "-c", clientId, "-u", redirectUri, "-r", resourcegroup, "-t", tenantName, "-s", subscriptionId, "--vault", vaultName, "--MakeCert", "--CertificateName", certname});

        }
        /// <summary>
        /// We can export it to file system if we really need it, but hey lets try to not store these things on your laptops using a empty password :)
        /// </summary>
        [TestMethod]
        public void ExportCert()
        {

            var clientId = "dedd37c0-c2c2-40fe-b1db-0b0c21d4b55a";
            var redirectUri = "https://devops.car2cloud.dk";

            var tenantName = "car2cloud.onmicrosoft.com";
            var subscriptionId = "6d8083fe-97c5-4f4d-b5d4-cc4bf001bfcb";

            var certname = "azure-keyvault-demo";
            var resourcegroup = "azure-keyvault-demo";
            var vaultName = "vwzdgctm7ywb6";

            var location = "c:\\dev\\" + certname + ".pfx";

            Program.Main(new[] { "-c", clientId, "-u", redirectUri, "-r", resourcegroup, "-s", subscriptionId, "-t",tenantName, "--ExportCert", certname, "--vault", vaultName, "-o", location });

        }

        /// <summary>
        /// We can encrypt a value with the just stored secret
        /// </summary>
        [TestMethod]
        public void EncryptValue()
        {
            var clientId = "dedd37c0-c2c2-40fe-b1db-0b0c21d4b55a";
            var redirectUri = "https://devops.car2cloud.dk";

            var tenantName = "car2cloud.onmicrosoft.com";
            var subscriptionId = "6d8083fe-97c5-4f4d-b5d4-cc4bf001bfcb";

            var certname = "azure-keyvault-demo";
            var resourcegroup = "azure-keyvault-demo";
            var vaultName = "vwzdgctm7ywb6";
            var value = "mysecretkey";

            Program.Main(new[] { "-c", clientId, "-u", redirectUri, "-r", resourcegroup, "-s", subscriptionId, "-t",tenantName, "--vault", vaultName, "--CertificateName", certname, "--encrypt", value });

            

        }
        /// <summary>
        /// And decrypt it again.
        /// </summary>
        [TestMethod]
        public void DecryptValue()
        {
            var clientId = "dedd37c0-c2c2-40fe-b1db-0b0c21d4b55a";
            var redirectUri = "https://devops.car2cloud.dk";

            var tenantName = "car2cloud.onmicrosoft.com";
            var subscriptionId = "6d8083fe-97c5-4f4d-b5d4-cc4bf001bfcb";

            var certname = "azure-keyvault-demo";
            var resourcegroup = "azure-keyvault-demo";
            var vaultName = "vwzdgctm7ywb6";
            var value = "MIIBnQYJKoZIhvcNAQcDoIIBjjCCAYoCAQAxggFOMIIBSgIBADAyMB4xHDAaBgNVBAMTE2F6dXJlLWtleXZhdWx0LWRlbW8CEEilhJ3CxxOhQGFWdRaUCMwwDQYJKoZIhvcNAQEBBQAEggEAFqmdqoiZNcfV/8sdoPxVXUruExoP85Zkgm1Vu4tQoWeewWGbF9dUTI9eV1yMl9lPFNkDy/qRbnjtocpqCQG5Tu1oWvzIOpnjpx6apJ9DQ0U5zbTxlAe8xKLRYIAnL6Hs7XsSBvhVWlGIgturjlnZE1eh0btGbxokjDY4tM7cs9fJoiavNFcxYfOyaUnVxF+8ybyNNPJHyoltfmAQV2RkclpVW+ArUiKiWLG+R3zlkeVwzDW43zAw1KCDMzvqJC3XdyQEttIJMAv9SwW5DNVSqf48JzoFxUmHZJgrL3dkRVu39lTZYzd2Iy2qG870PPXZr+r7ujredE2GS4oNMvddrDAzBgkqhkiG9w0BBwEwFAYIKoZIhvcNAwcECP8IJme8fYA2gBDEGoC9TzQD5LTx29f6A/VN";


            Program.Main(new[] { "-c", clientId, "-u", redirectUri, "-r", resourcegroup, "-s", subscriptionId, "-t", tenantName, "--vault", vaultName, "--CertificateName", certname, "--decrypt", value });


        }


    }
}
