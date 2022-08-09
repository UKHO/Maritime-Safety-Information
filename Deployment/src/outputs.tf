output "web_app_name" {
  value = local.web_app_name
}

output "env_name" {
  value = local.env_name
}

output "webapp_rg" {
  value = azurerm_resource_group.rg.name
}

output "admin_webapp_name" {
   value = module.webapp_service.admin_webapp_name
}

output "storage_name" {
   value = module.cache_storage.cache_storage_name
}

output "table_storage_name" {
   value = module.cache_storage.table_storage_name
}
