# Manages a ServiceBus Namespace.
resource "azurerm_servicebus_namespace" "this" {
    name                = "dailyadditions"
    resource_group_name = var.resource_group_name
    location            = var.location
    sku                 = "Basic"
}

# Manages a ServiceBus Queue.
resource "azurerm_servicebus_queue" "this" {
    name                = "additions"
    resource_group_name = var.resource_group_name
    namespace_name      = azurerm_servicebus_namespace.this.name
}