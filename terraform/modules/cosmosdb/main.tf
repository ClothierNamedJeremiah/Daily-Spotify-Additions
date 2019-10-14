# Manages a CosmosDB (formally DocumentDB) Account.
resource "azurerm_cosmosdb_account" "this" {
    name                = "dailyadditionscmdbacc"
    resource_group_name = var.resource_group_name
    location            = var.location
    offer_type          = "Standard"
    consistency_policy {
        consistency_level = "Eventual"
    }
    geo_location {
        location          = var.location
        failover_priority = 0
    }
}

# Manages a SQL Database within a Cosmos DB Account.
resource "azurerm_cosmosdb_sql_database" "this" {
    name                = "dailymusic"
    resource_group_name = var.resource_group_name
    account_name        = azurerm_cosmosdb_account.this.name
}

# Manages a SQL Container within a Cosmos DB Account.
resource "azurerm_cosmosdb_sql_container" "this" {
    name                = "playlists"
    resource_group_name = var.resource_group_name
    account_name        = azurerm_cosmosdb_account.this.name
    database_name       = azurerm_cosmosdb_sql_database.this.name
    partition_key_path  = "/name"
}