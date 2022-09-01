                              # Get all variable groups
                              $groups = ConvertFrom-Json "$(az pipelines variable-group list)"
                              $groups | foreach {
                              $groupName = $_.name
                  
                              # Prepend VariableGroups folder name
                              $filePath = Join-Path "VariableGroups" "$groupName.json"
                              ls
                  
                               # Save the variable group to a file
                              ConvertTo-Json $_ | New-Item $filePath -Force
                              }
