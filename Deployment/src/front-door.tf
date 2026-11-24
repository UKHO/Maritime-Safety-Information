locals {
  front_door_name_prefix                 = "msi-${local.env_name}"
  front_door_admin_name_prefix           = "msi-admin-${local.env_name}"
  front_door_custom_domain_enabled       = var.front_door_custom_domain_hostname != ""
  front_door_admin_custom_domain_enabled = var.front_door_admin_custom_domain_hostname != ""

  # Same IP allowlist as the web apps; VNet subnet rules have no Front Door equivalent.
  front_door_allowed_ips = [for ip in var.allowed_ips : length(split("/", ip)) > 1 ? ip : "${ip}/32"]

  # Legacy URL redirects (301) carried over from the FortiWeb MaritimeSafetyInformation1-18 rewrite policies.
  # path: URL path without the leading "/", lowercase (matched with a Lowercase transform).
  # query_contains: optional lowercase query fragment that must also be present in the request URL.
  # Rules are evaluated in list order, so query-specific entries must precede the bare path they share.
  # An empty destination_hostname keeps the incoming host so redirects work on every environment's domain.
  front_door_legacy_redirects = [
    { name = "NmsAnnualTab", path = "maritime-safety-information/admiralty-notices-to-mariners", query_contains = "selecttab=annual", destination_path = "/NoticesToMariners/Annual" },
    { name = "NmsAboutTab", path = "maritime-safety-information/admiralty-notices-to-mariners", query_contains = "selecttab=about", destination_path = "/NoticesToMariners/About" },
    { name = "NmsLeisureTab", path = "maritime-safety-information/admiralty-notices-to-mariners", query_contains = "selecttab=leisure", destination_path = "/NoticesToMariners/Leisure" },
    { name = "MsiRoot", path = "maritime-safety-information", destination_path = "/" },
    { name = "Nms", path = "maritime-safety-information/admiralty-notices-to-mariners", destination_path = "/NoticesToMariners/Weekly" },
    { name = "NmsLowBandwidth", path = "maritime-safety-information/admiralty-notices-to-mariners/low-bandwidth", destination_path = "/NoticesToMariners/Weekly" },
    { name = "NmsLowBandwidthDaily", path = "maritime-safety-information/admiralty-notices-to-mariners/low-bandwidth/daily-nms", destination_path = "/NoticesToMariners/Daily" },
    { name = "Rnw", path = "maritime-safety-information/radio-navigational-warnings", destination_path = "/RadioNavigationalWarnings" },
    { name = "RnwLowBandwidthAll", path = "maritime-safety-information/radio-navigational-warnings/low-bandwidth/all-warnings", destination_path = "/RadioNavigationalWarnings" },
    { name = "RnwLowBandwidthNavarea1", path = "maritime-safety-information/radio-navigational-warnings/low-bandwidth/navarea-1", destination_path = "/RadioNavigationalWarnings", destination_fragment = "NAVAREA1" },
    { name = "RnwLowBandwidthUkCoastal", path = "maritime-safety-information/radio-navigational-warnings/low-bandwidth/ukcoastal", destination_path = "/RadioNavigationalWarnings", destination_fragment = "UKCoastal" },
    { name = "LeisureSc5623", path = "nms/leisure/5623sc5623_ireland_-_south_west_coast.pdf", destination_hostname = "assets.admiralty.co.uk", destination_path = "/public/2022-07/5623SC5623_Ireland_-_South_West_Coast.pdf", query_string = "VersionId=KMT0KONo.YvTWt2C30xO94fQaoXV1Yfc" },
    { name = "LeisureSc5604", path = "nms/leisure/5604sc5604_the_channel_islands.pdf", destination_hostname = "assets.admiralty.co.uk", destination_path = "/public/2022-07/5604SC5604_The_Channel_Islands.pdf", query_string = "VersionId=F9FXa0kUAbBfx5Si1e6z3kva_ZcPHazM" },
    { name = "LeisureSc5607", path = "nms/leisure/5607sc5607_thames_estuary_essex_and_suffolk_coast.pdf", destination_hostname = "assets.admiralty.co.uk", destination_path = "/public/2022-07/5607SC5607_Thames_Estuary_Essex_and_Suffolk_Coast.pdf", query_string = "VersionId=S181vDqMY_1pDW3if4AwHjOXDmxm8ioR" },
    { name = "LeisureSc5614", path = "nms/leisure/5614sc5614_east_coast_-_orford_ness_to_whitby.pdf", destination_hostname = "assets.admiralty.co.uk", destination_path = "/public/2022-07/5614SC5614_East_Coast_-_Orford_Ness_to_Whitby.pdf", query_string = "VersionId=qSUAX0X8sE59Ln8k3C8Ceq_LdftyRT9O" },
    { name = "LeisureSc5617", path = "nms/leisure/5617sc5617_fife_ness_to_inverness_and_the_caledonian_canal.pdf", destination_hostname = "assets.admiralty.co.uk", destination_path = "/public/2022-07/5617SC5617_Fife_Ness_to_Inverness_and_the_Caledonian_Canal.pdf", query_string = "VersionId=55eq.xNgwBx39wrB5DMAiPmARnEW2UWr" },
    { name = "LeisureSc5622", path = "nms/leisure/5622sc5622_ireland_-_south_coast.pdf", destination_hostname = "assets.admiralty.co.uk", destination_path = "/public/2022-07/5622SC5622_Ireland_-_South_Coast.pdf", query_string = "VersionId=z8MS71rs7ZDAbpmy7IBl1fNvPMCF4aj4" },
  ]

  front_door_legacy_redirect_rules = {
    for i, r in local.front_door_legacy_redirects : "legacy_${lower(r.name)}" => {
      name              = "Legacy${r.name}"
      rule_set_key      = "legacy_redirects"
      order             = i + 1
      behavior_on_match = "Stop"
      actions = {
        url_redirect_action = {
          redirect_type        = "Moved"
          redirect_protocol    = "Https"
          destination_hostname = try(r.destination_hostname, "")
          destination_path     = r.destination_path
          query_string         = try(r.query_string, null)
          destination_fragment = try(r.destination_fragment, null)
        }
      }
      conditions = {
        url_path_condition = {
          operator         = "Equal"
          negate_condition = false
          match_values     = [r.path]
          transforms       = ["Lowercase"]
        }
        request_uri_condition = try(r.query_contains, null) == null ? null : {
          operator         = "Contains"
          negate_condition = false
          match_values     = [r.query_contains]
          transforms       = ["Lowercase"]
        }
      }
    }
  }
}

module "front_door_delivery" {
  count  = var.front_door_enabled ? 1 : 0
  source = "github.com/UKHO/tfmodule-alz-front-door-compliant//modules/front-door-delivery?ref=v922453"

  providers = {
    azurerm = azurerm.frontdoor
  }

  front_door_profile_name   = var.front_door_profile_name
  front_door_resource_group = var.front_door_resource_group
  shared_endpoint_name      = var.front_door_shared_endpoint_name

  origin_groups = {
    msi = {
      name                     = "${local.front_door_name_prefix}-origin-group"
      session_affinity_enabled = false
      load_balancing = {
        sample_size                        = 4
        successful_samples_required        = 3
        additional_latency_in_milliseconds = 50
      }
      health_probe = {
        protocol            = "Https"
        interval_in_seconds = 30
        request_type        = "GET"
        path                = "/health"
      }
    }
  }

  # Private Link creates a pending private endpoint connection on the web app that must be approved after apply.
  origins = {
    msi_webapp = {
      name                           = "${local.front_door_name_prefix}-origin"
      origin_group_key               = "msi"
      enabled                        = true
      host_name                      = module.webapp_service.default_site_hostname
      http_port                      = 80
      https_port                     = 443
      origin_host_header             = module.webapp_service.default_site_hostname
      priority                       = 1
      weight                         = 1000
      certificate_name_check_enabled = true
      private_link = {
        request_message        = "MSI ${local.env_name} Front Door private link request"
        target_type            = "sites"
        location               = module.webapp_service.webapp_service.location
        private_link_target_id = module.webapp_service.webapp_service.id
      }
    }
  }

  custom_domains = local.front_door_custom_domain_enabled ? {
    msi_domain = {
      name        = "${local.front_door_name_prefix}-domain"
      dns_zone_id = var.front_door_custom_domain_dns_zone_id != "" ? var.front_door_custom_domain_dns_zone_id : null
      host_name   = var.front_door_custom_domain_hostname
      tls         = {} # Defaults: ManagedCertificate with TLS12
    }
  } : {}

  rule_sets = merge(
    {
      legacy_redirects = {
        name = "msi${local.env_name}legacyredirects"
      }
    },
    var.front_door_ip_restriction_enabled ? {
      ip_restriction = {
        name = "msi${local.env_name}iprestriction"
      }
    } : {}
  )

  rules = merge(
    local.front_door_legacy_redirect_rules,
    var.front_door_ip_restriction_enabled ? {
      block_non_allowed_ips = {
        name              = "BlockNonAllowedIPs"
        rule_set_key      = "ip_restriction"
        order             = 1
        behavior_on_match = "Stop"
        actions = {
          url_redirect_action = {
            redirect_type        = "TemporaryRedirect"
            redirect_protocol    = "Https"
            destination_hostname = ""
          }
        }
        conditions = {
          remote_address_condition = {
            operator         = "IPMatch"
            negate_condition = true
            match_values     = local.front_door_allowed_ips
          }
        }
      }
    } : {}
  )

  routes = {
    msi_route = {
      name                   = "${local.front_door_name_prefix}-route"
      origin_group_key       = "msi"
      origin_keys            = ["msi_webapp"]
      enabled                = var.front_door_route_enabled
      forwarding_protocol    = "HttpsOnly"
      https_redirect_enabled = true
      patterns_to_match      = ["/*"]
      supported_protocols    = ["Http", "Https"]
      custom_domain_keys     = local.front_door_custom_domain_enabled ? ["msi_domain"] : []
      link_to_default_domain = !local.front_door_custom_domain_enabled
      rule_set_keys          = var.front_door_ip_restriction_enabled ? ["ip_restriction", "legacy_redirects"] : ["legacy_redirects"]
    }
  }
}

module "front_door_delivery_admin" {
  count  = var.front_door_enabled ? 1 : 0
  source = "github.com/UKHO/tfmodule-alz-front-door-compliant//modules/front-door-delivery?ref=v922453"

  providers = {
    azurerm = azurerm.frontdoor
  }

  front_door_profile_name   = var.front_door_profile_name
  front_door_resource_group = var.front_door_resource_group
  shared_endpoint_name      = var.front_door_shared_endpoint_name

  origin_groups = {
    msi_admin = {
      name                     = "${local.front_door_admin_name_prefix}-origin-group"
      session_affinity_enabled = false
      load_balancing = {
        sample_size                        = 4
        successful_samples_required        = 3
        additional_latency_in_milliseconds = 50
      }
      health_probe = {
        protocol            = "Https"
        interval_in_seconds = 30
        request_type        = "GET"
        path                = "/health"
      }
    }
  }

  # Private Link creates a pending private endpoint connection on the web app that must be approved after apply.
  origins = {
    msi_admin_webapp = {
      name                           = "${local.front_door_admin_name_prefix}-origin"
      origin_group_key               = "msi_admin"
      enabled                        = true
      host_name                      = module.webapp_service.admin_default_site_hostname
      http_port                      = 80
      https_port                     = 443
      origin_host_header             = module.webapp_service.admin_default_site_hostname
      priority                       = 1
      weight                         = 1000
      certificate_name_check_enabled = true
      private_link = {
        request_message        = "MSI Admin ${local.env_name} Front Door private link request"
        target_type            = "sites"
        location               = module.webapp_service.admin_webapp_service.location
        private_link_target_id = module.webapp_service.admin_webapp_service.id
      }
    }
  }

  custom_domains = local.front_door_admin_custom_domain_enabled ? {
    msi_admin_domain = {
      name        = "${local.front_door_admin_name_prefix}-domain"
      dns_zone_id = var.front_door_admin_custom_domain_dns_zone_id != "" ? var.front_door_admin_custom_domain_dns_zone_id : null
      host_name   = var.front_door_admin_custom_domain_hostname
      tls         = {} # Defaults: ManagedCertificate with TLS12
    }
  } : {}

  rule_sets = var.front_door_ip_restriction_enabled ? {
    ip_restriction = {
      name = "msiadmin${local.env_name}iprestriction"
    }
  } : {}

  rules = var.front_door_ip_restriction_enabled ? {
    block_non_allowed_ips = {
      name              = "BlockNonAllowedIPs"
      rule_set_key      = "ip_restriction"
      order             = 1
      behavior_on_match = "Stop"
      actions = {
        url_redirect_action = {
          redirect_type        = "TemporaryRedirect"
          redirect_protocol    = "Https"
          destination_hostname = ""
        }
      }
      conditions = {
        remote_address_condition = {
          operator         = "IPMatch"
          negate_condition = true
          match_values     = local.front_door_allowed_ips
        }
      }
    }
  } : {}

  routes = {
    msi_admin_route = {
      name                   = "${local.front_door_admin_name_prefix}-route"
      origin_group_key       = "msi_admin"
      origin_keys            = ["msi_admin_webapp"]
      enabled                = var.front_door_route_enabled
      forwarding_protocol    = "HttpsOnly"
      https_redirect_enabled = true
      patterns_to_match      = ["/*"]
      supported_protocols    = ["Http", "Https"]
      custom_domain_keys     = local.front_door_admin_custom_domain_enabled ? ["msi_admin_domain"] : []
      link_to_default_domain = !local.front_door_admin_custom_domain_enabled
      rule_set_keys          = var.front_door_ip_restriction_enabled ? ["ip_restriction"] : []
    }
  }
}
