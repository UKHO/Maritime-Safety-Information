output cache_storage_name {
  value = azurerm_storage_account.cache_storage.name
}

output cache_storage_connection_string {
  value = azurerm_storage_account.cache_storage.primary_connection_string
  sensitive = true
}

output cache_storage_primary_access_key {
  value = azurerm_storage_account.cache_storage.primary_access_key
  sensitive = true
}

output table_storage_name {
  value = azurerm_storage_table.banner_notification_table.name
}
