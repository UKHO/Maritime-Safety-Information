param (
    [Parameter(Mandatory = $true)] [string] $businessUnit,
    [Parameter(Mandatory = $true)] [string] $resourceGroup,
    [Parameter(Mandatory = $true)] [string] $webappName
)

Write-Output "Set MSI Business Unit in appsetting..."
az webapp config appsettings set -g $resourceGroup -n $webappName --settings FileShareService:BusinessUnit=$businessUnit
az webapp restart --name $webappName --resource-group $resourceGroup
