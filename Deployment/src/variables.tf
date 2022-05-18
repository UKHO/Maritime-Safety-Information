variable "location" {
  type    = string
  default = "uksouth"
}

variable "resource_group_name" {
  type    = string
  default = "msi"
}

locals {
  env_name                    = lower(terraform.workspace)
  service_name                = "msi"
  web_app_name                = "${local.service_name}-${local.env_name}-webapp"
  key_vault_name              = "${local.service_name}-ukho-${local.env_name}-kv"
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
