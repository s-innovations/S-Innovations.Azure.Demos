using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Azure;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Management.KeyVault;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Subscriptions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DemoCommandLineTools
{
    public class Options
    {


   


        [Option('c', "ClientID", Required = true, HelpText = "AAD Client ID")]
        public string ClientID { get; set; }

        [Option('u', "RedirectUri", HelpText = "The redirect Uri")]
        public string RedirectUri { get; set; }
        [Option("listvaults", HelpText = "List Vaults")]
        public bool ListVaults { get; set; }

        [Option("listsubscriptions", HelpText = "List Subscriptions")]
        public bool ListSubscriptions { get; set; }

        [Option('s', "SubscriptionId", HelpText = "SubscriptionId")]
        public string SubscriptionId { get; set; }

        [Option('t', "TenantName", HelpText = "TenantName")]
        public string TenantName { get; set; }

        [Option('r', "ResourceGroup", HelpText = "The resource group ")]
        public string ResourceGroup { get; set; }
        [Option("AddApplication", HelpText = "AddApplication")]
        public string AddApplication { get; set; }

        [Option("vault", HelpText = "the keyvault name")]
        public string Vault { get; set; }

        [Option("AllowUpdate", HelpText = "AllowUpdate")]
        public bool AllowUpdate { get; set; }

        [Option("CreateVault", HelpText = "the Secret value")]
        public string CreateVault { get; set; }

        [Option("location", HelpText = "Location")]
        public string Location { get; set; }



        [Option("deploy", HelpText = "Deploy Template")]
        public string Deploy { get; set; }
        [Option("deployName", HelpText = "Deploy Name")]
        public string DeployName { get; set; }

        [Option("siteName", HelpText = "SiteName")]
        public string SiteName { get; set; }

        [Option("hostingPlanName", HelpText = "HostingPlanName")]
        public string HostingPlanName { get; set; }

        [Option("storageAccountType", HelpText = "StorageAccountType")]
        public string StorageAccountType { get; set; }

        [Option("websitePlan", HelpText = "WebsitePlan")]
        public string WebsitePlan { get; set; }


        [Option("applicationId", HelpText = "The target ApplicationID")]
        public string ApplicaitonId { get; set; }


        [Option("MakeCert", HelpText = "MakeCert")]
        public bool MakeCert { get; set; }

        [Option("CertificateName", HelpText = "CertificateName")]
        public string CertificateName { get; set; }


        [Option("ExportCert", HelpText = "ExportCert")]
        public string ExportCert { get; set; }

        [Option('o', "Out", HelpText = "Out")]
        public string Out { get; set; }

        [Option("Encrypt", HelpText = "Encrypt")]
        public string Encrypt { get; set; }
        [Option("Decrypt", HelpText = "Decrypt")]
        public string Decrypt { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {

            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                var authContext = new AuthenticationContext(string.Format("https://login.windows.net/{0}", options.TenantName ?? "common"), new FileCache());
                var token = authContext.AcquireToken("https://management.core.windows.net/", options.ClientID, new Uri(options.RedirectUri), PromptBehavior.Auto);

                HandleListSubscriptions(options, token);

                if (!string.IsNullOrEmpty(options.SubscriptionId))
                {
                    using (var client = new KeyVaultManagementClient(new TokenCloudCredentials(options.SubscriptionId, token.AccessToken)))
                    {

                        var vaultInfo = client.Vaults.Get(options.ResourceGroup, options.Vault);
                        var vaultToken = authContext.AcquireToken("https://vault.azure.net", "1950a258-227b-4e31-a9cf-717495945fc2", new Uri("urn:ietf:wg:oauth:2.0:oob"));
                        var keyvaultClient = new KeyVaultClient((_, b, c) => Task.FromResult(vaultToken.AccessToken));

                        if (!string.IsNullOrEmpty(options.ExportCert))
                        {

                            var secret = keyvaultClient.GetSecretAsync(vaultInfo.Vault.Properties.VaultUri, options.ExportCert).GetAwaiter().GetResult();
                            var cert = new X509Certificate2(Convert.FromBase64String(secret.Value), new SecureString(), X509KeyStorageFlags.Exportable);

                            File.WriteAllBytes(options.Out, cert.Export(X509ContentType.Pfx));
                        }


                        if (!string.IsNullOrEmpty(options.Encrypt))
                        {



                            var secret = keyvaultClient.GetSecretAsync(vaultInfo.Vault.Properties.VaultUri, options.CertificateName).GetAwaiter().GetResult();
                            var cert = new X509Certificate2(Convert.FromBase64String(secret.Value));


                            byte[] encoded = System.Text.UTF8Encoding.UTF8.GetBytes(options.Encrypt);
                            var content = new ContentInfo(encoded);
                            var env = new EnvelopedCms(content);
                            env.Encrypt(new CmsRecipient(cert));

                            string encrypted64 = Convert.ToBase64String(env.Encode());

                            Console.WriteLine("Encrypting: {0}", options.Encrypt);
                            Console.WriteLine("Encrypted Base64 String: {0}", encrypted64);


                        }

                        if (!string.IsNullOrEmpty(options.Decrypt))
                        {
                            var secret = keyvaultClient.GetSecretAsync(vaultInfo.Vault.Properties.VaultUri, options.CertificateName).GetAwaiter().GetResult();
                            var cert = new X509Certificate2(Convert.FromBase64String(secret.Value));

                            var encryptedBytes = Convert.FromBase64String(options.Decrypt);
                            var envelope = new EnvelopedCms();
                            envelope.Decode(encryptedBytes);
                            envelope.Decrypt(new X509Certificate2Collection(cert));


                            Console.WriteLine("Decrypting: {0}", options.Decrypt);
                            Console.WriteLine("Decrypted String: {0}", Encoding.UTF8.GetString(envelope.ContentInfo.Content));
                        }

                        if (options.MakeCert)
                        {

                            var cert = Convert.ToBase64String(Certificate.CreateSelfSignCertificatePfx(string.Format("CN={0}", options.CertificateName), DateTime.UtcNow, DateTime.UtcNow.AddYears(2)));
                            var cert1 = new X509Certificate2(Convert.FromBase64String(cert));
                            var secrets = keyvaultClient.GetSecretsAsync(vaultInfo.Vault.Properties.VaultUri).GetAwaiter().GetResult();
                            if (secrets.Value == null || !secrets.Value.Any(s => s.Id == vaultInfo.Vault.Properties.VaultUri + "secrets/" + options.CertificateName))
                            {

                                Console.WriteLine(
                                       JsonConvert.SerializeObject(keyvaultClient.SetSecretAsync(vaultInfo.Vault.Properties.VaultUri, options.CertificateName, cert, null, "application/pkcs12").GetAwaiter().GetResult()
                                       , Formatting.Indented));
                            }
                        }
                    }

                
                

                    using (var resourceManagementClient = new ResourceManagementClient(new TokenCloudCredentials(options.SubscriptionId, token.AccessToken)))
                    {
                        HandleListVaults(options, resourceManagementClient);
                        HandleAddToApplication(options, authContext, token);
                        HandleCreateVault(options, authContext, token, resourceManagementClient);

                        if (!string.IsNullOrWhiteSpace(options.Deploy))
                        {
                            ResourceGroupExtended rg = GetResourceGroup(options, resourceManagementClient);
                            //Fix location to displayname from template
                            using (var subscriptionClient = new SubscriptionClient(new TokenCloudCredentials(token.AccessToken)))
                            {
                                var a = subscriptionClient.Subscriptions.ListLocations(options.SubscriptionId);
                                rg.Location = a.Locations.Single(l => l.Name == rg.Location).DisplayName;                                    
                            }

                            var graphtoken = authContext.AcquireToken("https://graph.windows.net/", options.ClientID, new Uri(options.RedirectUri), PromptBehavior.Auto);
                            var graph = new ActiveDirectoryClient(new Uri("https://graph.windows.net/" + graphtoken.TenantId), () => Task.FromResult(graphtoken.AccessToken));
                            var principal = graph.ServicePrincipals.Where(p => p.AppId == options.ApplicaitonId).ExecuteSingleAsync().GetAwaiter().GetResult();


                            DeploymentExtended deploymentInfo = null;
                            if (!resourceManagementClient.Deployments.CheckExistence(options.ResourceGroup, options.DeployName).Exists)
                            {

                                var deployment = new Deployment
                                {
                                    Properties = new DeploymentProperties
                                    {
                                        Mode = DeploymentMode.Incremental, //Dont Delete other resources
                                        Template = File.ReadAllText(options.Deploy),
                                        Parameters = new JObject(
                                            new JProperty("siteName", CreateValue(options.SiteName)),
                                            new JProperty("hostingPlanName", CreateValue(options.HostingPlanName)),
                                            new JProperty("storageAccountType", CreateValue(options.StorageAccountType)),
                                            new JProperty("siteLocation", CreateValue(rg.Location)),
                                            new JProperty("sku", CreateValue(options.WebsitePlan)),
                                            new JProperty("tenantId",CreateValue(token.TenantId)),
                                            new JProperty("objectId",CreateValue(token.UserInfo.UniqueId)),
                                            new JProperty("appOwnerTenantId", CreateValue(principal.AppOwnerTenantId.Value.ToString())),
                                            new JProperty("appOwnerObjectId", CreateValue(principal.ObjectId))
                                            ).ToString(),

                                    }
                                };

                                var result = resourceManagementClient.Deployments.CreateOrUpdate(options.ResourceGroup, options.DeployName, deployment);
                                deploymentInfo = result.Deployment;
                            }
                            else
                            {
                                var deploymentStatus = resourceManagementClient.Deployments.Get(options.ResourceGroup, options.DeployName);
                                deploymentInfo = deploymentStatus.Deployment;
                                
                            }

                            while(!(deploymentInfo.Properties.ProvisioningState == "Succeeded" || deploymentInfo.Properties.ProvisioningState == "Failed"))
                            {
                                var deploymentStatus = resourceManagementClient.Deployments.Get(options.ResourceGroup, options.DeployName);
                                deploymentInfo = deploymentStatus.Deployment;
                                Thread.Sleep(5000);
                            }

                            var outputs = JObject.Parse(deploymentInfo.Properties.Outputs);
                            var storageAccountName = outputs["storageAccount"]["value"].ToString();
                            var keyvaultName = outputs["keyvault"]["value"].ToString();

                            using (var client = new KeyVaultManagementClient(new TokenCloudCredentials(options.SubscriptionId, token.AccessToken)))
                            {
                                using (var storageClient = new StorageManagementClient(new TokenCloudCredentials(options.SubscriptionId, token.AccessToken)))
                                {
                                    var keys = storageClient.StorageAccounts.ListKeys(options.ResourceGroup, storageAccountName);

                                    var vaultInfo = client.Vaults.Get(options.ResourceGroup, keyvaultName);
                                   //CHEATING (using powershell application id to get token on behhalf of user);
                                    var vaultToken = authContext.AcquireToken("https://vault.azure.net", "1950a258-227b-4e31-a9cf-717495945fc2", new Uri("urn:ietf:wg:oauth:2.0:oob"));
                                    var keyvaultClient = new KeyVaultClient((_, b, c) => Task.FromResult(vaultToken.AccessToken));

                                    var secrets = keyvaultClient.GetSecretsAsync(vaultInfo.Vault.Properties.VaultUri).GetAwaiter().GetResult();
                                    if (secrets.Value == null || !secrets.Value.Any(s => s.Id == vaultInfo.Vault.Properties.VaultUri +"secrets/storage"))
                                    {
                                        keyvaultClient.SetSecretAsync(vaultInfo.Vault.Properties.VaultUri, "storage", $"{storageAccountName}:{keys.StorageAccountKeys.Key1}").GetAwaiter().GetResult();
                                        keyvaultClient.SetSecretAsync(vaultInfo.Vault.Properties.VaultUri, "storage", $"{storageAccountName}:{keys.StorageAccountKeys.Key2}").GetAwaiter().GetResult();


                                        var secret = keyvaultClient.GetSecretVersionsAsync(vaultInfo.Vault.Properties.VaultUri, "storage").GetAwaiter().GetResult();
                                    }
                                }

                                
                               
                            }


                        }
                    }


                }

            }
        }

        private static ResourceGroupExtended GetResourceGroup(Options options, ResourceManagementClient resourceManagementClient)
        {
            var rge = resourceManagementClient.ResourceGroups.CheckExistence(options.ResourceGroup);

            if (!rge.Exists)
            {
                var result = resourceManagementClient.ResourceGroups.CreateOrUpdate(options.ResourceGroup, new ResourceGroup { Location = options.Location });
            }
            var rg = resourceManagementClient.ResourceGroups.Get(options.ResourceGroup).ResourceGroup;
            return rg;
        }

        private static JObject CreateValue(string value)
        {
            return new JObject(new JProperty("value", value));
        }

        private static void HandleCreateVault(Options options, AuthenticationContext authContext, AuthenticationResult token, ResourceManagementClient resourceManagementClient)
        {
            if (!string.IsNullOrEmpty(options.CreateVault))
            {



                var vaults = resourceManagementClient.Resources.List(new ResourceListParameters { ResourceType = "Microsoft.KeyVault/vaults" });

                if (options.AllowUpdate || !vaults.Resources.Any(v => v.Name == options.CreateVault))
                {
                    ResourceGroupExtended rg = GetResourceGroup(options, resourceManagementClient);

                    var graphtoken = authContext.AcquireToken("https://graph.windows.net/", options.ClientID, new Uri(options.RedirectUri), PromptBehavior.Auto);
                    var graph = new ActiveDirectoryClient(new Uri("https://graph.windows.net/" + graphtoken.TenantId), () => Task.FromResult(graphtoken.AccessToken));
                    var principals = graph.ServicePrincipals.Where(p => p.AppId == options.ClientID).ExecuteSingleAsync().GetAwaiter().GetResult();


                    // var a = graph.GetObjectsByObjectIdsAsync(new[] { "ffe34afb-c9e7-45bc-a8d2-c1dedc649b7c" }, new string[] { }).GetAwaiter().GetResult().ToArray();
                    using (var client = new KeyVaultManagementClient(new TokenCloudCredentials(options.SubscriptionId, token.AccessToken)))
                    {

                        Console.WriteLine(JsonConvert.SerializeObject(
                        client.Vaults.CreateOrUpdate(options.ResourceGroup, options.CreateVault, new VaultCreateOrUpdateParameters
                        {
                            Properties = new VaultProperties
                            {
                                EnabledForDeployment = true,
                                Sku = new Sku { Name = "Premium", Family = "A" },
                                AccessPolicies = new List<AccessPolicyEntry>{
                                            new AccessPolicyEntry{
                                                 TenantId = Guid.Parse(token.TenantId),
                                                 ObjectId = Guid.Parse(principals.ObjectId),
                                                 PermissionsToSecrets=new []{"all"},
                                                  PermissionsToKeys = new []{"all"}
                                            }, new AccessPolicyEntry{
                                                TenantId =  Guid.Parse(token.TenantId),
                                                ObjectId = Guid.Parse(token.UserInfo.UniqueId),
                                                PermissionsToSecrets=new []{"all"},
                                                PermissionsToKeys = new []{"all"}
                                            }
                            },
                                TenantId = Guid.Parse(token.TenantId)
                            },
                            Location = rg.Location,
                        }),
                     Formatting.Indented));
                    }
                }
                else
                {
                    using (var client = new KeyVaultManagementClient(new TokenCloudCredentials(options.SubscriptionId, token.AccessToken)))
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(
                        client.Vaults.Get(options.ResourceGroup, options.CreateVault).Vault, Formatting.Indented));
                    }
                }
            }
        }

        private static void HandleListSubscriptions(Options options, AuthenticationResult token)
        {
            if (options.ListSubscriptions)
            {
                using (var subscriptionClient = new SubscriptionClient(new TokenCloudCredentials(token.AccessToken)))
                {
                   
                    var subscriptions = subscriptionClient.Subscriptions.ListAsync().GetAwaiter().GetResult().Subscriptions;

                    foreach (var subscription in subscriptions)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(subscription, Formatting.Indented));
                    }


                }
            }
        }

        private static void HandleAddToApplication(Options options, AuthenticationContext authContext, AuthenticationResult token)
        {
            if (!string.IsNullOrEmpty(options.AddApplication))
            {
                using (var client = new KeyVaultManagementClient(new TokenCloudCredentials(options.SubscriptionId, token.AccessToken)))
                {
                   

                    var graphtoken = authContext.AcquireToken("https://graph.windows.net/", options.ClientID, new Uri(options.RedirectUri), PromptBehavior.Auto);
                    var graph = new ActiveDirectoryClient(new Uri("https://graph.windows.net/" + graphtoken.TenantId), () => Task.FromResult(graphtoken.AccessToken));
                    var principals = graph.ServicePrincipals.Where(p => p.AppId == options.AddApplication).ExecuteSingleAsync().GetAwaiter().GetResult();
               
                    var vault = client.Vaults.GetAsync(options.ResourceGroup, options.Vault).GetAwaiter().GetResult();
                    if (!vault.Vault.Properties.AccessPolicies.Any(a => a.ObjectId == Guid.Parse(principals.ObjectId)))
                    {
                        vault.Vault.Properties.AccessPolicies.Add(new AccessPolicyEntry
                        {
                            TenantId = Guid.Parse(principals.AppOwnerTenantId.ToString()),
                            ObjectId = Guid.Parse(principals.ObjectId),
                            PermissionsToSecrets = new[] { "all" },
                            PermissionsToKeys = new[] { "all" }

                        });
                    }
                    Console.WriteLine(JsonConvert.SerializeObject(
                          client.Vaults.CreateOrUpdate(options.ResourceGroup, options.Vault,
                          new VaultCreateOrUpdateParameters(vault.Vault.Properties, vault.Vault.Location)
                          {
                              Tags = vault.Vault.Tags,
                          }), Formatting.Indented));



                }
            }
        }

        private static void HandleListVaults(Options options, ResourceManagementClient resourceManagementClient)
        {
            if (options.ListVaults)
            {
                resourceManagementClient.Providers.Register("Microsoft.KeyVault");
                var vaults = resourceManagementClient.Resources.List(new ResourceListParameters { ResourceType = "Microsoft.KeyVault/vaults" });
                Console.WriteLine(JsonConvert.SerializeObject(vaults, Formatting.Indented));
            }
        }

    }
}
