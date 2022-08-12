param (
    [Parameter(Mandatory = $true)] [string] $businessUnit,
    [Parameter(Mandatory = $true)] [string] $resourceGroup,
    [Parameter(Mandatory = $true)] [string] $webappName,
    [Parameter(Mandatory = $true)] [string] $isCacheEnabled
)

Write-Output "Set MSI Business Unit in appsetting..."
az webapp config appsettings set -g $resourceGroup -n $webappName --settings FileShareService:BusinessUnit=$businessUnit CacheConfiguration:IsFssCacheEnabled=$isCacheEnabled
az webapp restart --name $webappName --resource-group $resourceGroup
