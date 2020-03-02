param (
    [Parameter(Mandatory=$true)]
    [string]
    $ArmOutputString
)

if ($PSBoundParameters['Verbose']) {
    Write-Host "Arm output json is:"
    Write-Host $ArmOutputString
}

$armOutputObj = $ArmOutputString | ConvertFrom-Json

$armOutputObj.PSObject.Properties | ForEach-Object {
    $type = ($_.value.type).ToLower()
    $keyname = "ArmOutputs.$($_.name)"
    $value = $_.value.value

    if ($type -eq "securestring") {
        Write-Host "##vso[task.setvariable variable=$keyname;issecret=true]$value"
        Write-Host "Added Azure DevOps secret variable '$keyname' ('$type')"
    } elseif ($type -eq "string") {
        Write-Host "##vso[task.setvariable variable=$keyname]$value"
        Write-Host "Added Azure DevOps variable '$keyname' ('$type') with value '$value'"
    } else {
        Throw "Type '$type' is not supported for '$keyname'"
    }
}