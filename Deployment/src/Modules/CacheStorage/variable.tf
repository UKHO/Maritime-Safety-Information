variable "name" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

variable "tags" {
}

variable "allowed_ips" {

}

variable "m_spoke_subnet" {
  type = string
}

variable "agent_subnet" {
  type = string
}

variable "table_name" {
  type = string
}
