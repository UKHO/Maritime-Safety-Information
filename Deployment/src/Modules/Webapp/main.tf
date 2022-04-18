resource "azurerm_service_plan" "app_service_plan" {
  name                = "${var.name}-asp"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku_name            = var.sku_name
  os_type             = "Windows"
  tags                = var.tags
}

resource "azurerm_windows_web_app" "webapp_service" {
  name                = var.name
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.app_service_plan.id
  tags                = var.tags

  site_config {
        current_stack = "dotnet"
        dotnet_version = "v6.0"
       }
    always_on  = true
    ftps_state = "Disabled"

   }
  app_settings = var.app_settings

  identity {
    type = "SystemAssigned"
    }

  https_only = true
  }
