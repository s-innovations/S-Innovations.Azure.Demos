
# Azure Meetup 17 / Nov Demo


## Notes from Lasse

[powerpoint](https://github.com/s-innovations/S-Innovations.Azure.Demos/raw/master/docs/Azure%20User%20Group%20-%20Azure%20Key%20Vault%2017%20nov.pptx)

[https://social.msdn.microsoft.com/Forums/azure/en-US/56bed4bd-6320-4f1f-9af5-556ae91bdf0f/storing-sql-connection-string-passwords-in-key-vault-for-my-cloud-services?forum=AzureKeyVault&prof=required](https://social.msdn.microsoft.com/Forums/azure/en-US/56bed4bd-6320-4f1f-9af5-556ae91bdf0f/storing-sql-connection-string-passwords-in-key-vault-for-my-cloud-services?forum=AzureKeyVault&prof=required)

There are multiple aspects involved in managing application config secrets well. The goal of Azure Key Vault is to help you across this entire spectrum.

    The obvious one is they need a secure storage location, with very restricted access, separate from the application source code.
    They should be available to the corresponding application without human intervention. It’s common to have apps deployed via automated pipelines these days.
    The secret owners should be able to rotate secrets without needing a full redeploy of the app. We have some customers who rotate some secrets every 20 mins!
    For some kinds of secrets, such as encryption keys, updates must preserve older versions. That is so that the application can continue to decrypt data encrypted with an older version of the key.
    The secret owner may want a log of accesses for monitoring & compliance. (This feature is not available in Key Vault yet.)
    Teams that manage multiple applications / roles / VMs benefit from managing their secrets in a consistent fashion, in a central place, as that reduces mistakes.

Your observation is spot on, that to access a key vault, an application needs a bootstrap ‘secret’ to authenticate to Azure AD. The best choice for that depends on what deployment tools you use. For most scenarios, a certificate is the best bootstrap. Azure provides mechanisms for you to store a PFX file in a safe place and inject it to your application’s VMs just when that VM spins up. Specifically

    If your application runs in VMs deployed through the new Azure Resource Manager, then see this article: http://blogs.technet.com/b/kv/archive/2015/07/14/vm_2d00_certificates.aspx
    If your application is a Cloud Service or runs in VMs deployed through the old Azure Service Manager, then see “Service certificates” in this article: https://azure.microsoft.com/en-us/documentation/articles/cloud-services-certs-create/

For this bootstrap certificate you get a subset of the benefits outlined above. For the rest of your secrets you can avail of all of the benefits outlined above by managing them via Azure Key Vault. 

In the Azure Key Vault samples [http://www.microsoft.com/en-us/download/details.aspx?id=45343](http://www.microsoft.com/en-us/download/details.aspx?id=45343) see the SampleAzureWebService sample for an example of how an application can use a certificate as bootstrap. See the script GetServiceConfigSettings.ps1 in the scripts folder for how to register the cert as the application's credential in Azure AD.


## Notes From Poul

Code have been debugged and runs. Will write detailed how to here in the coming days. 

One cool example of using the keyvault is as explained in this [blog post](http://www.dushyantgill.com/blog/2015/04/26/say-goodbye-to-key-management-manage-access-to-azure-storage-data-using-azure-ad/) where a automation job rolls the storage keys and updates the keyvault accordingly and applications automaticly gets the rolled keys without redeployment.

