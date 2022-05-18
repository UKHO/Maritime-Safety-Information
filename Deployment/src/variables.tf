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

variable "rnw_db_app_user" {

}

variable "rnw_db_app_pass" {

}

locals {
  env_name                    = lower(terraform.workspace)
  service_name                = "msi"
  web_app_name                = "${local.service_name}-${local.env_name}-webapp"
  key_vault_name              = "${local.service_name}-ukho-${local.env_name}-kv"
  rnw_db_connection_string    = "Server=${var.rnw_db_server};Initial Catalog=${var.rnw_db_name};Persist Security Info=False;User ID=${var.rnw_db_app_user};Password=${var.rnw_db_app_pass};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication="Active Directory Password";"
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
