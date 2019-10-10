variable "location" {
    type = "string"
    default = "West US 2"
}

variable "prefix" {
    type = "string"
    default = "daily-music"
}

variable "tags" {
    type = "map"
    default = {
        app_name = "Data Arcus"
    }
}

variable subscription_id {}
variable tenant_id {}