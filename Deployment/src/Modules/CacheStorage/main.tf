resource "azurerm_storage_account" "cache_storage" {
  name                               = var.name
  resource_group_name                = var.resource_group_name
  location                           = var.location
  account_tier                       = "Standard"
  account_replication_type           = "LRS"
  account_kind                       = "StorageV2"
  allow_nested_items_to_be_public    = false
  network_rules {
    default_action                   = "Deny"
    ip_rules                         = var.allowed_ips
    bypass                           = ["Logging", "Metrics", "AzureServices"]
    virtual_network_subnet_ids       = [var.m_spoke_subnet,var.agent_subnet]
                }

  tags                               = var.tags
}

resource "azurerm_storage_table" "banner_notification_table" {
  name                 = var.table_name
  storage_account_name = azurerm_storage_account.cache_storage.name
}
