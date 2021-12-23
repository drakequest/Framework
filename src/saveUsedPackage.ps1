# This is an automatic variable set to the current file's/module's directory
SET-LOCATION $PSScriptRoot

Get-ChildItem 'packages' -Recurse  -ErrorAction SilentlyContinue | `
    Where-Object {($_.PSIsContainer -eq $False) -and ($_.Name -like '*.nupkg') } | ` 
    ForEach-Object {
        Write-Host  $_.Name;
        Copy-Item -Path $_.Fullname -Destination 'local_packages' `
            -Force `
            -ErrorAction Ignore
        };