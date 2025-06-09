module "data_protection" {
  source = "github.com/DFE-Digital/terraform-azurerm-aspnet-data-protection?ref=v1.3.0"

  data_protection_key_vault_assign_role                 = false
  data_protection_key_vault_subnet_prefix               = local.data_protection_subnet_prefix
  data_protection_key_vault_access_ipv4                 = local.key_vault_access_ipv4
  data_protection_resource_prefix                       = "${local.environment}${local.project_name}"
  data_protection_azure_location                        = local.azure_location
  data_protection_tags                                  = local.tags
  data_protection_resource_group_name                   = local.existing_resource_group
  data_protection_vnet_name                             = local.existing_virtual_network
  data_protection_diagnostic_log_analytics_workspace_id = module.azure_container_apps_hosting.azurerm_log_analytics_workspace_container_app.id
}
