provider "azurerm" {
    version = ">=1.32.0"

    subscription_id = var.subscription_id
    tenant_id       = var.tenant_id
}

resource "azurerm_resource_group" "rg" {
    name     = "DailyAdditionsRG"
    location = var.location
}

module "cosmosdb" {
    source              = "./modules/cosmosdb"
    resource_group_name = azurerm_resource_group.rg.name
    location            = var.location
}

module "servicebus" {
    source              = "./modules/servicebus"
    resource_group_name = azurerm_resource_group.rg.name
    location            = var.location
}

# Create a storage account
resource "azurerm_storage_account" "this" {
    name                     = "dailyadditionssa" # only lowercase and numbers allowed
    resource_group_name      = azurerm_resource_group.rg.name
    location                 = var.location
    account_tier             = "Standard"
    account_replication_type = "LRS"
}

# Create an App Service Plan (Consumption Based, i.e. 'dynamic')
resource "azurerm_app_service_plan" "this" {
    name                = "dailyadditionssp"
    resource_group_name = azurerm_resource_group.rg.name
    location            = var.location
    kind                = "FunctionApp"

    sku {
        tier = "Dynamic"
        size = "F1"
    }
}

# Manages a Function App.
resource "azurerm_function_app" "this" {
    name                      = "DailyAdditions"
    resource_group_name       = azurerm_resource_group.rg.name
    location                  = var.location
    app_service_plan_id       = azurerm_app_service_plan.this.id
    storage_connection_string = azurerm_storage_account.this.primary_connection_string

    app_settings = {
        FUNCTION_WORKER_RUNTIME    = "dotnet"
        CosmosDBConnection         = module.cosmosdb.connection_string
        ServiceBusConnection       = module.servicebus.connection_string
        AzureWebJobsSendGridApiKey = var.sendgrid_api_key
        SpotifyClientId            = var.spotify_client_id
        SpotfiyClientSecret        = var.spotify_client_secret
    }
}

