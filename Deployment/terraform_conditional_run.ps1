param (
    [Parameter(Mandatory = $true)] [string] $deploymentResourceGroupName,
    [Parameter(Mandatory = $true)] [string] $deploymentStorageAccountName,
    [Parameter(Mandatory = $true)] [string] $workSpace,
    [Parameter(Mandatory = $true)] [boolean] $continueEvenIfResourcesAreGettingDestroyed,
    [Parameter(Mandatory = $true)] [string] $terraformJsonOutputFile
)

cd $env:AGENT_BUILDDIRECTORY/terraformartifact/src

terraform --version

Write-output "Executing terraform scripts for deployment in $workSpace enviroment"
terraform init -backend-config="resource_group_name=$deploymentResourceGroupName" -backend-config="storage_account_name=$deploymentStorageAccountName" -backend-config="key=terraform.deployment.tfplan"
if ( !$? ) { echo "Something went wrong during terraform initialization"; throw "Error" }

Write-output "Selecting workspace"

$ErrorActionPreference = 'SilentlyContinue'
terraform workspace new $WorkSpace 2>&1 > $null
$ErrorActionPreference = 'Continue'

terraform workspace select $workSpace
if ( !$? ) { echo "Error while selecting workspace"; throw "Error" }

Write-output "Validating terraform"
terraform validate
if ( !$? ) { echo "Something went wrong during terraform validation" ; throw "Error" }

Write-output "Execute Terraform plan"
terraform plan -out "terraform.deployment.tfplan" | tee terraform_output.txt
if ( !$? ) { echo "Something went wrong during terraform plan" ; throw "Error" }

$totalDestroyLines=(Get-Content -Path terraform_output.txt | Select-String -Pattern "destroy" -CaseSensitive |  where {$_ -ne ""}).length
if($totalDestroyLines -ge 2) 
{
    write-Host("Terraform is destroying some resources, please verify...................")
    if ( !$ContinueEvenIfResourcesAreGettingDestroyed) 
    {
        write-Host("exiting...................")
        Write-Output $_
        exit 1
    }
    write-host("Continue executing terraform apply - as continueEvenIfResourcesAreGettingDestroyed param is set to true in pipeline")
}

Write-output "Executing terraform apply"
terraform apply  "terraform.deployment.tfplan"
if ( !$? ) { echo "Something went wrong during terraform apply" ; throw "Error" }

Write-output "Terraform output as json"
$terraformOutput = terraform output -json | ConvertFrom-Json

write-output "Set JSON output into pipeline variables"
Write-Host "##vso[task.setvariable variable=MsiWafUrl;isOutput=true]$env:SERVICE_DNS_URL"
Write-Host "##vso[task.setvariable variable=Admin_DNS_URL;isOutput=true]$env:SERVICE_ADMIN_DNS_URL"
Write-Host "##vso[task.setvariable variable=WEB_APP_NAME;isOutput=true]$($terraformOutput.web_app_name.value)"
Write-Host "##vso[task.setvariable variable=ADMIN_WEB_APP_NAME;isOutput=true]$($terraformOutput.admin_webapp_name.value)"
Write-Host "##vso[task.setvariable variable=webapp_rg;isOutput=true]$($terraformOutput.webapp_rg.value)"
Write-Host "##vso[task.setvariable variable=storage_name]$($terraformOutput.storage_name.value)"
Write-Host "##vso[task.setvariable variable=table_storage_name]$($terraformOutput.table_storage_name.value)"
Write-Host "##vso[task.setvariable variable=WEB_APP_SLOT_NAME;isOutput=true]$($terraformOutput.web_app_slot_name.value)"
Write-Host "##vso[task.setvariable variable=WEB_APP_SLOT_HOST_NAME;isOutput=true]$($terraformOutput.web_app_slot_default_site_hostname.value)"
Write-Host "##vso[task.setvariable variable=WEB_ADMIN_APP_SLOT_NAME;isOutput=true]$($terraformOutput.web_admin_app_slot_name.value)"
Write-Host "##vso[task.setvariable variable=WEB_ADMIN_APP_SLOT_HOST_NAME;isOutput=true]$($terraformOutput.web_admin_app_slot_default_site_hostname.value)"

$terraformOutput | ConvertTo-Json -Depth 5 > $terraformJsonOutputFile

