resource "azurerm_resource_group" "rg" {
  name     = "${var.resource_group_name}-${local.env_name}-testrg"
  location = var.location
  tags     = local.tags
}
