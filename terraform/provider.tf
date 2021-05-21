terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>2.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# resource "azurerm_resource_group" "rg" {
#   name     = "src"
#   location = "Japan East"
# }

resource "azurerm_resource_group" "dumy-rg-for-terraform" {
  name     = "dumy-rg-for-terraform"
  location = "Japan East"
}
