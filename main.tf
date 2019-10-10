provider "azurerm" {
    version = ">=1.32.0"

    subscription_id = var.subscription_id
    # client_id       = var.client_id
    # client_secrete  = var.client_secret
    tenant_id       = var.tenant_id
}

# Creates a resource group
resource "azurerm_resource_group" "this" {
    name     = "jeremiah-terraform-DataArcus"
    location = var.location

    tags = var.tags
}

resource "azurerm_cosmosdb_account" "this" {
    name                = "jeremiah-cosmos-account"
    resource_group_name = azurerm_resource_group.this.name
    location            = var.location
    offer_type          = "Standard"
    consistency_policy {
        consistency_level = "Eventual"
    }
    geo_location {
        failover_priority = 0
        location          = "West US 2"
    }

    tags = var.tags
}

resource "azurerm_cosmosdb_sql_database" "this" {
    name                = "DailyMusic"
    resource_group_name = azurerm_resource_group.this.name
    account_name        = azurerm_cosmosdb_account.this.name
}

resource "azurerm_cosmosdb_sql_container" "playlists" {
    name                = "Playlists"
    resource_group_name = azurerm_resource_group.this.name
    account_name        = azurerm_cosmosdb_account.this.name
    database_name       = azurerm_cosmosdb_sql_database.this.name
    partition_key_path  = "/name"

}

# # Create a storage account
# resource "azurerm_storage_account" "this" {
#     name = "dailymusicstorage" # only lowercase and numbers allowed
#     resource_group_name = "${azurerm_resource_group.this.name}"
#     location = var.location
#     account_tier = "Standard"
#     account_replication_type = "LRS"
# }

# # Create an App Service Plan (Consumption Based, i.e. 'dynamic')
# resource "azurerm_app_service_plan" "this" {
#     name                = "${var.prefix}-service-plan"
#     resource_group_name = "${azurerm_resource_group.this.name}"
#     location = var.location
#     kind                = "FunctionApp"

#     sku {
#         tier = "Dynamic"
#         size = "F1"
#     }
# }

# # Create an Azure Function
# resource "azurerm_function_app" "this" {
#     name                      = "${var.prefix}-app"
#     resource_group_name = "${azurerm_resource_group.this.name}"
#     location = var.location
#     app_service_plan_id       = "${azurerm_app_service_plan.this.id}"
#     storage_connection_string = "${azurerm_storage_account.this.primary_connection_string}"
# }