param (
    [Parameter(Mandatory = $true)] [string] $Storageaccountname,
	[Parameter(Mandatory = $true)] [string] $Tablename
)

Write-Output "Storage account name : $Storageaccountname ..."
Write-Output "Table name : $Tablename ..."

Write-Output "Checking if an entity is present in table storage..."
$ErrorActionPreference = 'Continue'
az storage entity show --account-name $Storageaccountname --table-name $Tablename --partition-key "1" --row-key "BannerNotificationKey" 2>&1 | Tee-Object -FilePath '.\output.txt'

if ( Select-String -Path output.txt -Pattern "^ERROR:.*")
{
	Write-Output "Inserting an entity in table storage..."
	az storage entity insert --account-name $Storageaccountname --entity PartitionKey=1 RowKey="BannerNotificationKey" ExpiryDate="2021-08-02T12:54:25.5224203Z" ExpiryDate@odata.type=Edm.DateTime StartDate="2021-05-02T12:54:25.5224203Z" StartDate@odata.type=Edm.DateTime Message="column properties which require for Banner Notification Table" Message@odata.type=Edm.String IsNotificationEnabled="false" IsNotificationEnabled@odata.type=Edm.Boolean --table-name $Tablename

    if ( !$? ) 
            { 
        Write-Output "Error while running Az command from the script ..." 
        Write-Output "exiting ..."
        exit 1
    }
}
else
{
	Write-Output "Already found an existing entity in table storage..."
}
