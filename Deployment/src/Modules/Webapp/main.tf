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
     application_stack {    
     current_stack = "dotnet"
     dotnet_version = "v6.0"
    }
    always_on  = true
    ftps_state = "Disabled"

    ip_restriction {
      virtual_network_subnet_id = var.subnet_id
    }

    dynamic "ip_restriction" {
      for_each = var.allowed_ips
      content {
          ip_address  = length(split("/",ip_restriction.value)) > 1 ? ip_restriction.value : "${ip_restriction.value}/32"
      }
    }

   }
     
  app_settings = var.app_settings

  identity {
    type = "SystemAssigned"
    }

  https_only = true
  }

resource "azurerm_app_service_virtual_network_swift_connection" "webapp_vnet_integration" {
  app_service_id = azurerm_windows_web_app.webapp_service.id
  subnet_id      = var.subnet_id
}

#Admin Webapp
resource "azurerm_windows_web_app" "admin_webapp_service" {
  name                = var.admin_webapp_name
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.app_service_plan.id
  tags                = var.tags

  site_config {
     application_stack {    
     current_stack = "dotnet"
     dotnet_version = "v6.0"
    }
    always_on  = true
    ftps_state = "Disabled"

    ip_restriction {
      virtual_network_subnet_id = var.subnet_id
    }

    dynamic "ip_restriction" {
      for_each = var.allowed_ips
      content {
        ip_address  = length(split("/",ip_restriction.value)) > 1 ? ip_restriction.value : "${ip_restriction.value}/32"
      }
    }
   }
  app_settings = var.app_settings

  identity {
    type = "SystemAssigned"
    }

  https_only = true
  }

resource "azurerm_app_service_virtual_network_swift_connection" "admin_webapp_vnet_integration" {
  app_service_id = azurerm_windows_web_app.admin_webapp_service.id
  subnet_id      = var.subnet_id
}
