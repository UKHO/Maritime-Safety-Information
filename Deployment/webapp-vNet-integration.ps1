param (   
    [Parameter(Mandatory = $true)] [string] $resourcegroup,   
    [Parameter(Mandatory = $true)] [string] $webappname,
    [Parameter(Mandatory = $true)] [string] $vnetresourcegroupname,
    [Parameter(Mandatory = $true)] [string] $vnetname,
    [Parameter(Mandatory = $true)] [string] $subnetname
)

$subscriptionid = az account show --query id --output tsv
$vnet = "/subscriptions/$subscriptionid/resourceGroups/$vnetresourcegroupname/providers/Microsoft.Network/virtualNetworks/$vnetname";
$subnet = "/subscriptions/$subscriptionid/resourceGroups/$vnetresourcegroupname/providers/Microsoft.Network/virtualNetworks/$vnetname/subnets/$subnetname";

Write-Output "Integrating webapp with vNet..."
az webapp vnet-integration add -g $resourcegroup -n $webappname --vnet $vnet  --subnet $subnet
