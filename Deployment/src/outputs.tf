output "web_app_name" {
  value = local.web_app_name
}

output "env_name" {
  value = local.env_name
}

output "webapp_rg" {
  value = azurerm_resource_group.rg.name
}

output "Website_Url" {
  value = "https://${module.webapp_service.default_site_hostname}/"
}

output "Website_Admin_Url" {
  value = "https://${module.webapp_service.default_site_hostname}/RadioNavigationalWarningsAdmin"
}

output "admin_webapp_name" {
   value = module.webapp_service.admin_webapp_name
}
