terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>2.60.0"
    }
  }
  backend "remote" {
    hostname     = "app.terraform.io"
    organization = "koudenpa"

    workspaces {
      name = "SRC"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "src"
  location = "Japan East"
}
