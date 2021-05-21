
resource "azurerm_static_site" "srcs_data_viewer" {
  name                = "srcs-data-viewer"
  resource_group_name = "src"
  location            = "eastasia"
}
