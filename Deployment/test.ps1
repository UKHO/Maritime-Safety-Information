$ProjectName = "Maritime Safety Information"
$Organization = "https://dev.azure.com/ukhydro" 
# Get variable-groups list
$vars = az pipelines variable-group list --organization $Organization --project $ProjectName

# Get variable-groups names list
$v = $vars | ConvertFrom-Json -AsHashtable
$variable_group_names = $v.name

# Go though every variable-group
foreach ($variable_group_name in $variable_group_names) {
    $group = az pipelines variable-group list --organization $Organization --project $ProjectName --group-name $variable_group_name
    
    # Get every variable-group Id
    $g = $group | ConvertFrom-Json -AsHashtable
    $groupId = $g.id
   
    
    # Check if VariableGroups folder exist, if not - create it
    if (!(Test-Path .\VariableGroups)){
        New-Item -itemType Directory -Path .\VariableGroups
    } 
    
    # Get every variable-group content and place it in corresponding folder
    az pipelines variable-group show --organization $Organization --project $ProjectName --id $groupId  --output json>.\VariableGroups\$variable_group_name.json
    Write-Host created .\VariableGroups\$variable_group_name.json
}
