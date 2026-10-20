variable "location" {
  type    = string
  default = "uksouth"
}

variable "resource_group_name" {
  type    = string
  default = "msi"
}

variable "rnw_db_server" {
}

variable "rnw_db_name" {
}

variable "rnw_db_app_sqluser" {
}

variable "rnw_db_app_sqlpass" {
}

variable "rnw_db_admin_app_sqluser" {
}

variable "rnw_db_admin_app_sqlpass" {
}

locals {
  env_name           = lower(terraform.workspace)
  service_name       = "msi"
  web_app_name       = "${local.service_name}-${local.env_name}-webapp"
  admin_web_app_name = "${local.service_name}-admin-${local.env_name}-webapp"
  key_vault_name     = "${local.service_name}-ukho-${local.env_name}-kv"
  rnw_db_connection_string = "Server=${var.rnw_db_server};Initial Catalog=${var.rnw_db_name};Persist Security Info=False;User ID=${var.rnw_db_app_sqluser};Password=${var.rnw_db_app_sqlpass};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  rnw_db_admin_connection_string = "Server=${var.rnw_db_server};Initial Catalog=${var.rnw_db_name};Persist Security Info=False;User ID=${var.rnw_db_admin_app_sqluser};Password=${var.rnw_db_admin_app_sqlpass};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  tags = {
    SERVICE                   = "Maritime Safety Information"
    ENVIRONMENT               = local.env_name
    SERVICE_OWNER             = "UKHO"
    RESPONSIBLE_TEAM          = "Mastek"
    CALLOUT_TEAM              = "On-Call_N/A"
    COST_CENTRE               = "A.008.02"
  }
}

variable "sku_name" {
  type = map(any)
  default = {
             "dev"  =  "P1v2"
             "qa"   =  "P1v3"
             "live" =  "P1v3"
            }
}

variable "spoke_rg" {
  type = string
}

variable "spoke_vnet_name" {
  type = string
}

variable "spoke_subnet_name" {
  type = string
}

variable "agent_rg" {
  type = string
}

variable "agent_vnet_name" {
  type = string
}

variable "agent_subnet_name" {
  type = string
}

variable "agent_subscription_id" {
  type = string
}

variable "allowed_ips" {
  type = list
}

# Front Door; set per environment in config/<workspace>.tfvars.
variable "front_door_enabled" {
  description = "Whether to configure Front Door delivery routing for the MSI and MSI admin web apps."
  type        = bool
  default     = false
}

variable "front_door_route_enabled" {
  description = "Whether the Front Door routes are enabled. Allows the routes to be disabled without removing resources. Only applies when front_door_enabled is true."
  type        = bool
  default     = true
}

variable "front_door_ip_restriction_enabled" {
  description = "Whether to restrict the Front Door routes to the web apps' IP allowlist via a rule set."
  type        = bool
  default     = false
}

variable "front_door_subscription_id" {
  description = "The subscription ID where the shared Azure Front Door profile is deployed."
  type        = string
  default     = ""
}

variable "front_door_profile_name" {
  description = "The name of the existing shared Azure Front Door profile created by the platform team."
  type        = string
  default     = ""
}

variable "front_door_resource_group" {
  description = "The resource group containing the shared Azure Front Door profile."
  type        = string
  default     = ""
}

variable "front_door_shared_endpoint_name" {
  description = "The name of the shared Front Door endpoint to attach routes to."
  type        = string
  default     = ""
}

variable "front_door_custom_domain_hostname" {
  description = "The public hostname for the MSI web app. Leave empty to use the shared endpoint's default domain instead."
  type        = string
  default     = ""
}

variable "front_door_custom_domain_dns_zone_id" {
  description = "The resource ID of the Azure DNS zone used for MSI custom domain validation. May be empty while DNS is hosted outside Azure."
  type        = string
  default     = ""
}

variable "front_door_admin_custom_domain_hostname" {
  description = "The public hostname for the MSI admin (RNW) web app. Leave empty to use the shared endpoint's default domain instead."
  type        = string
  default     = ""
}

variable "front_door_admin_custom_domain_dns_zone_id" {
  description = "The resource ID of the Azure DNS zone used for MSI admin custom domain validation. May be empty while DNS is hosted outside Azure."
  type        = string
  default     = ""
}
