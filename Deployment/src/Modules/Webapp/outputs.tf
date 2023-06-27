output "web_app_object_id" {
  value = azurerm_windows_web_app.webapp_service.identity.0.principal_id
}

output "web_app_tenant_id" {
  value = azurerm_windows_web_app.webapp_service.identity.0.tenant_id
}

output "default_site_hostname" {
  value = azurerm_windows_web_app.webapp_service.default_hostname
}

output "admin_webapp_name" {
  value = azurerm_windows_web_app.admin_webapp_service.name
}

output "admin_web_app_object_id" {
  value = azurerm_windows_web_app.admin_webapp_service.identity.0.principal_id
}

output "admin_default_site_hostname" {
  value = azurerm_windows_web_app.admin_webapp_service.default_hostname
}

output "slot_principal_id" {
  value = azurerm_windows_web_app_slot.webapp_service_staging.identity.0.principal_id
}

output "slot_name" {
  value = azurerm_windows_web_app_slot.webapp_service_staging.name
}

output "slot_default_site_hostname" {
  value = azurerm_windows_web_app_slot.webapp_service_staging.default_hostname
}

output "slot_admin_principal_id" {
  value = azurerm_windows_web_app_slot.admin_webapp_service_staging.identity.0.principal_id
}

output "slot_admin_name" {
  value = azurerm_windows_web_app_slot.admin_webapp_service_staging.name
}

output "slot_admin_default_site_hostname" {
  value = azurerm_windows_web_app_slot.admin_webapp_service_staging.default_hostname
}
