######
# Bot Framework: Provision, train and publish a LUIS App to service
######
 
 param (
    [Parameter(
            Mandatory=$True,
            HelpMessage="LUIS Authoring key from luis.ai portal"
			)][string]$LUISAuthKey,
    [Parameter(
			Mandatory=$False,
			HelpMessage="LUIS App name, otherwise will auto-generate"
		)][string]$AppName,
    [Parameter(
			Mandatory=$True,
			HelpMessage="LUIS .json model file"
		)][string]$LuisModelFile,
    [Parameter(
			Mandatory=$False,
			HelpMessage="Azure region for LUIS model (default: 'westus')"
		)][string]$Region = "westus"

 )
 


 ##
 #Step 0. Process Parameters and defaults
 ##

$timestamp = (Get-Date).ToString("MMddHHmm")

#Set some test defaults if not specified

if($AppName -eq "")
{
  # use default values
  $AppName = "ESLouIsReed"
  $AppName += $timestamp
  
}


if (-Not (Test-Path $LuisModelFile)) 
{
  Write-Host "Error: Cannot find $LuisModelFile"
  Exit
}

$LuisAppInfo = &luis import application --region "$Region" --authoringKey "$LUISAuthKey" --appName "$AppName" --in "$LuisModelFile"
$LuisAppInfoJson = $LuisAppInfo | ConvertFrom-Json

$lid = $LuisAppInfoJson.id
$lver = $LuisAppInfoJson.activeVersion

Write-Host "** Luis App Info:"
Write-Host " ID: $lid "
Write-Host " Version: $lver"
Write-Host "`n---"
Write-Host "Training..."


$LuisTrainedInfo = &luis train version --region "$Region" --authoringKey "$LUISAuthKey"  --appId "$lid" --versionId "$lver" --wait
$LuisTrainedInfo

Write-Host "`n---"
Write-Host "Publishing..."
#BUGBUG: "region & publishRegion are assumed to be the same in this test script. In reality they don't have to be the same.
$LuisPublishInfo = &luis publish version --region "$Region" --authoringKey "$LUISAuthKey" --appId "$lid" --versionId "$lver" --publishRegion "$Region"
$LuisPublishInfo