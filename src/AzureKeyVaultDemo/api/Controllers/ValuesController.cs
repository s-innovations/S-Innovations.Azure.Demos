using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SInnovations.ConfigurationManager;
using SInnovations.ConfigurationManager.Providers;

namespace AzureKeyVaultDemo.Controllers
{
   
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ConfigurationManager config;
        public ValuesController(ConfigurationManager config)
        {
            this.config = config;
        }
        private async Task<string> GetAccessToken(string authority, string resource, string scope)
        {

            try
            {
                var clientid = "84eeeca9-408f-477d-9af8-9de3de6923c0"; var secret = "";
              
                var authContext = new AuthenticationContext(authority);
              
                var task = authContext.AcquireTokenAsync(resource, new ClientCredential(clientid, secret));
                task.Wait(5000);
                var result = await task;

                return result.AccessToken;
            }
            catch (Exception ex)
            {
             
                throw;
            }
        }
        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {

            if (false) //DONT RUN
            {
                var keyvaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
                var secrets = await keyvaultClient.GetSecretsAsync("https://ascend-xyz-testing-weu.vault.azure.net");
            }

            //Using the abstraction we can do
            var storageKey = config.GetAzureKeyVaultSecret("storage");

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
