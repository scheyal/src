######
# Bot Framework: Create LUIS Endpoint
######
 
 param (
     [Parameter(
            Mandatory=$True,
            HelpMessage="LUIS Authoring key from luis.ai portal"
			)][string]$LUISAuthKey,
     [Parameter(
            Mandatory=$True,
            HelpMessage="LUIS App ID. Get from the portal or from 'luis import command'"
			)][string]$LUISAppId,

    [Parameter(
            Mandatory=$True,
            HelpMessage="AzSubId"
			)][string]$AzSubId,
    [Parameter(
			Mandatory=$False,
			HelpMessage="Root Seed for generating LUIS App and Resource Group name, otherwise will auto-generate"
		)][string]$AppName,
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

$ResourceGroup = $AppName + "RG"



Write-Host "`n---"
Write-Host "Creating Resource Group..."
Write-Host 'az group create --subscription "$AzSubId" --location "$Region" --name "$ResourceGroup"'

$RGInfo = &az group create --subscription "$AzSubId" --location "$Region" --name "$ResourceGroup"

$RGInfo

Write-Host "`n---"
Write-Host "Creating LUIS App..."
Write-Host 'az cognitiveservices account create --kind "LUIS" --name "$AppName" --sku "S0" --location "$Region" --subscription "$AzSubId" -g "$ResourceGroup"  --yes'

$LuisSvcInfo = &az cognitiveservices account create --kind "LUIS" --name "$AppName" --sku "S0" --location "$Region" --subscription "$AzSubId" -g "$ResourceGroup" --yes

Write-Host "** Luis Service Info:"
$LuisSvcInfo

$LuisSvcInfoJson = $LuisSvcInfo | ConvertFrom-Json


Write-Host "`n---"
Write-Host "Getting access token LUIS App..."
Write-Host 'az account get-access-token --subscription "$AzSubId"'
$AccessTokenInfo = &az account get-access-token --subscription "$AzSubId"
$AccessTokenInfoJson = $AccessTokenInfo | ConvertFrom-Json
$AccessToken = $AccessTokenInfoJson.accessToken
Write-Host "Access Token = $AccessToken"


$JsonObj = @{ azureSubscriptionId = "$AzSubId"; resourceGroup = "$ResourceGroup"; accountName="$AppName" }
$ReqFileName = "$AppName" + "Req.json"
$jsonReq = $JsonObj | ConvertTo-Json 
$jsonReq | Out-File -Force  -FilePath $ReqFileName
# $jsonReqStr = $jsonReq | ConvertFrom-Json


Write-Host "`n---"
Write-Host "Adding Azure Application Account..."
Write-Host 'luis add appazureaccount --in "$ReqFileName"  --appId "$LUISAppId" --authoringKey "$LUISAuthKey" --armToken "$AccessToken"'
$AddAccountInfo = &luis add appazureaccount --in "$ReqFileName"  --appId "$LUISAppId" --authoringKey "$LUISAuthKey" --armToken "$AccessToken"
$AddAccountInfo

Write-Host "`n---"
Write-Host "Showing LUIS Secrets!"
Write-Host 'az cognitiveservices account keys list --name "AppName" --subscription "AzSubId" -g "ResourceGroup"'
$LuisKeysInfo = &az cognitiveservices account keys list --name "$AppName" --subscription "$AzSubId" -g "$ResourceGroup"
$LuisKeysJson = $LuisKeysInfo | ConvertFrom-Json
$k1 = $LuisKeysJson.key1
$k2 = $LuisKeysJson.key2
Write-Host "Key1 = $k1"
Write-Host "Key2 = $k2"

Write-Host "`n---"
Write-Host "Done."

