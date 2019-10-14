
# Daily Additions
Daily Additions is a serverless function app the lies in Azure. The application will trigger every 24 hours and perform the following operations:
1) Scan a User's list of playlists they following using [Spotify API]([https://developer.spotify.com/documentation/web-api/](https://developer.spotify.com/documentation/web-api/)).
2) Compare the current version of the playlist with the version in Cosmos DB schemaless database and log any additions or deletions (changes since yesterday) to a Azure Service Bus Queue.
3) Send out an email to the user using [SendGrid API]([https://sendgrid.com/docs/API_Reference/api_v3.html](https://sendgrid.com/docs/API_Reference/api_v3.html)), notifying them which songs have been added to which playlists.

## Requirements
* Terraform [1.32.0](https://www.terraform.io/downloads.html](https://www.terraform.io/downloads.html)
* Microsoft.Azure.WebJobs.Extensions.CosmosDB [3.0.4](https://www.nuget.org/packages/Microsoft.Azure.WebJobs.Extensions.CosmosDB/)
* Microsoft.Azure.WebJobs.Extensions.SendGrid[3.0.0](https://www.nuget.org/packages/Microsoft.Azure.WebJobs.Extensions.SendGrid/)
* Microsoft.Azure.WebJobs.Extensions.ServiceBus [3.1.1](https://www.nuget.org/packages/Microsoft.Azure.WebJobs.Extensions.ServiceBus/)
* Microsoft.NET.Sdk.Functions [1.0.29](https://www.nuget.org/packages/Microsoft.NET.Sdk.Functions/)


## Files Not Included in Version Control
### local.settings.json
The local.settings.json file stores app settings, connection strings, and settings used by local development tools. Settings in the local.settings.json file are used only when you're running projects locally. The local settings file has this structure:
```json
{
"IsEncrypted": false,
"Values": {
	"AzureWebJobsStorage": "UseDevelopmentStorage=true",
	"FUNCTIONS_WORKER_RUNTIME": "dotnet",
	"CosmosDBConnection": "INSERT_VALUE_HERE", 
	"ServiceBusConnection": "INSERT_VALUE_HERE",
	"AzureWebJobsSendGridApiKey":"INSERT_VALUE_HERE",
	"SpotifyClientId": "INSERT_VALUE_HERE",
	"SpotfiyClientSecret": "INSERT_VALUE_HERE"
	}
}
```
_Note: An Azure Cosmos DB Emulator can be used when testing locally for no cost_

### terraform.tfvars
To set lots of variables, it is more convenient to specify their values in a  _variable definitions file_  (with a filename ending in either  `.tfvars`  or  `.tfvars.json`). Terraform will automatically load the variable definition file `terraform.tfvars ` if it is located in the `terraform/` directory. The file should be structured as follows:
```
subscription_id       = "INSERT_VALUE_HERE"
tenant_id             = "INSERT_VALUE_HERE"
sendgrid_api_key      = "INSERT_VALUE_HERE"
spotify_client_id     = "INSERT_VALUE_HERE"
spotify_client_secret = "INSERT_VALUE_HERE"
```

## Contributors

* **Jeremiah Clothier** - [Email](mailto:clothiernamedjeremiah@gmail.com)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
