variable "name" {
  type = string
}

variable "admin_webapp_name" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

variable "app_settings" {
  type = map(string)
}

variable "tags" {

}

variable "sku_name" {

}

variable "env_name" {
  type = string
}

variable "subnet_id" {
  type = string
}

variable "allowed_ips" {

}


