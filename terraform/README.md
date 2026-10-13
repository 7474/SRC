# Terraform

Terraform Cloud（現 HCP Terraform）と接続している。

Static Web Apps を管理できる範囲が拡充されたらカスタムドメイン他を管理するかもしれない程度。

使っている azurerm プロバイダは 2.60 系で、リソースは `azurerm_static_site`。新しいプロバイダでは `azurerm_static_web_app` に置き換わり、カスタムドメインは `azurerm_static_web_app_custom_domain` で管理できる。

- https://registry.terraform.io/providers/hashicorp/azurerm/2.60.0/docs/resources/static_site
- https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/static_web_app
- https://developer.hashicorp.com/terraform/cloud-docs
