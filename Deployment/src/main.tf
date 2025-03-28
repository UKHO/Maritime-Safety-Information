data "azurerm_subnet" "main_subnet" {
  name                 = var.spoke_subnet_name
  virtual_network_name = var.spoke_vnet_name
  resource_group_name  = var.spoke_rg
}

data "azurerm_subnet" "agent_subnet" {
  provider             = azurerm.build_agent
  name                 = var.agent_subnet_name
  virtual_network_name = var.agent_vnet_name
  resource_group_name  = var.agent_rg
}

module "app_insights" {
  source              = "./Modules/AppInsights"
  name                = "${local.service_name}-${local.env_name}-insights"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tags                = local.tags
}

module "eventhub" {
  source              = "./Modules/EventHub"
  name                = "${local.service_name}-${local.env_name}-events"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tags                = local.tags
  env_name            = local.env_name
}

module "webapp_service" {
  source                    = "./Modules/Webapp"
  name                      = local.web_app_name
  admin_webapp_name         = local.admin_web_app_name
  resource_group_name       = azurerm_resource_group.rg.name
  env_name                  = local.env_name
  location                  = azurerm_resource_group.rg.location
  subnet_id                 = data.azurerm_subnet.main_subnet.id
  sku_name                  = var.sku_name[local.env_name]
  app_settings = {
    "KeyVaultSettings:ServiceUri"                              = "https://${local.key_vault_name}.vault.azure.net/"
    "EventHubLoggingConfiguration:Environment"                 = local.env_name
    "EventHubLoggingConfiguration:MinimumLoggingLevel"         = "Warning"
    "EventHubLoggingConfiguration:UkhoMinimumLoggingLevel"     = "Information"
    "APPLICATIONINSIGHTS_CONNECTION_STRING"                    = module.app_insights.connection_string
    "ASPNETCORE_ENVIRONMENT"                                   = local.env_name
    "WEBSITE_RUN_FROM_PACKAGE"                                 = "1"
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"                          = "true"
    "WEBSITE_ADD_SITENAME_BINDINGS_IN_APPHOST_CONFIG"          = "1"
  }
  tags                                                         = local.tags
  allowed_ips                                                  = var.allowed_ips
}

module "key_vault" {
  source              = "./Modules/KeyVault"
  name                = local.key_vault_name
  resource_group_name = azurerm_resource_group.rg.name
  env_name            = local.env_name
  tenant_id           = module.webapp_service.web_app_tenant_id
  allowed_ips         = var.allowed_ips
  allowed_subnet_ids  = [data.azurerm_subnet.main_subnet.id,data.azurerm_subnet.agent_subnet.id]
  location            = azurerm_resource_group.rg.location
  read_access_objects = {
     "webapp_service"       = module.webapp_service.web_app_object_id
     "webapp_slot"          = module.webapp_service.slot_principal_id
     "admin_webapp_service" = module.webapp_service.admin_web_app_object_id
     "admin_webapp_slot"    = module.webapp_service.slot_admin_principal_id
  }
  secrets = {
      "EventHubLoggingConfiguration--ConnectionString"            = module.eventhub.log_primary_connection_string
      "EventHubLoggingConfiguration--EntityPath"                  = module.eventhub.entity_path
      "RadioNavigationalWarningsContext--ConnectionString"        = local.rnw_db_connection_string
      "RadioNavigationalWarningsAdminContext--ConnectionString"   = local.rnw_db_admin_connection_string
      "CacheConfiguration--CacheStorageAccountName"               = module.cache_storage.cache_storage_name
      "CacheConfiguration--CacheStorageAccountKey"                = module.cache_storage.cache_storage_primary_access_key
 }
  tags                                                       = local.tags
}

module "azure-dashboard" {
  source              = "./Modules/AzureDashboard"
  name                = "MSI-${local.env_name}-monitoring-dashboard"
  location            = azurerm_resource_group.rg.location
  environment         = local.env_name
  resource_group      = azurerm_resource_group.rg
  web_app_name        = local.web_app_name
  tags                = local.tags
}

module "cache_storage" {
  source                                = "./Modules/CacheStorage"
  name                                  = "${local.service_name}${local.env_name}cachestorage"
  table_name                            = "MsiBannerNotificationTable"
  resource_group_name                   = azurerm_resource_group.rg.name
  allowed_ips                           = var.allowed_ips
  location                              = azurerm_resource_group.rg.location
  tags                                  = local.tags
  m_spoke_subnet                        = data.azurerm_subnet.main_subnet.id
  agent_subnet                          = data.azurerm_subnet.agent_subnet.id
}
