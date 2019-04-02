
 ######
 # Bot Framework ARM Deployment Script
 # Prepares bot: deployment scripts (web.config, .deployment), build, deploy
 # Supports: CSharp, Node.js Bots
 ######
 
 param (
    [Parameter(
				Mandatory=$False,
				HelpMessage="Enter bot name, otherwise will auto-generate",
				Position=1
			)][string]$AppName,
    [Parameter(
			Mandatory=$False,
			HelpMessage="Enter AD App password, otherwise will auto-generate",
			Position=2
		)][string]$Password,
    [Parameter(
		Mandatory=$True,
		HelpMessage="Enter bot solution folder (where .csproj or index.js located)",
		Position=3
	)][string]$BotFolder,
    [Parameter(
		Mandatory=$False,
		HelpMessage="Mandatory for CSharp, ommit for Node",
		Position=4
	)][string]$ProjFile,
    [Parameter(
		Mandatory=$True,
		HelpMessage="Enter location ofr ARM template",
		Position=5
	)][string]$ArmTemplate,
    [ValidateSet("CSharp","Node")]
    [Parameter(
		Mandatory=$True,
		HelpMessage="Enter project language: 'CSharp' or 'Node'",
		Position=6
	)][string]$Lang,
    [Parameter(
		Mandatory=$False,
		HelpMessage="Enter deployment location",
		Position=7
	)][string]$Location = "WestUS",
    [Parameter(
		Mandatory=$False,
		HelpMessage="Enter whether deployment should be performed by Kudu",
		Position=8
	)][string]$BuildOnDeploy="false",
    [Parameter(
		Mandatory=$False,
		HelpMessage="Enter deployment timeout in seconds",
		Position=9
	)][string]$DeploymentTimeoutSec

 )
 


 ##
 #Step 0. Process Parameters and defaults
 ##



$timestamp = (Get-Date).ToString("MMddHHmm")

#Set some test defaults if not specified

if($AppName -eq "")
{
  # use default values
  $AppName = "ESShoulder"
  $AppName += $timestamp
  
}

if($Password -eq "")
{
    Write-Host "Generating password..."
    Add-Type -AssemblyName System.Web
    $pwd = [System.Web.Security.Membership]::GeneratePassword(20,0)
    $pwd += $timestamp
    $Password = $pwd
}

if($DeploymentTimeoutSec -ne "")
{
    # turn into command line param
    $DeploymentTimeoutSec = "-t " + $DeploymentTimeoutSec
}

if (-Not (Test-Path $BotFolder)) 
{
  Write-Host "Error: Cannot find $BotFolder"
  Exit
}

$ProjFilePath = Join-Path -Path $BotFolder -ChildPath $ProjFile
if ($Lang -eq "CSharp")
{
    if( $ProjFile -eq "" -or -Not (Test-Path $ProjFilePath) ) 
    {
      Write-Host "Error: Cannot find .csproj in $ProjFilePath"
      Exit
    }

    Write-Host "Building dotnet project..."
    Push-Location $BotFolder
    & dotnet build 
    Pop-Location
}
elseif ($Lang -eq "Node")
{
    Write-Host "Building Node project..." 
    Push-Location $BotFolder
    & npm install
    Pop-Location
}
else
{
  Write-Host "Error: Aborting. Unknown language $Lang"
  Exit
}



$Uri = "http://" + $AppName + ".microsoft.com"; 
$secret = $Password 
$DeploymentName = $AppName + "-deployment"
$ResourceGroup = $AppName + "RG"
$BotName=$AppName + "Bot"
$BotWebApp=$BotName + "WebApp"
$ServerFarm="CowBotsFarm" + $timestamp

Write-Host "AppName = $AppName"
Write-Host "ResourceGroup = $ResourceGroup"
Write-Host "BotName = $BotName"
Write-Host "BotWebApp = $BotWebApp"
Write-Host "ServerFarm = $ServerFarm"
Write-Host "BotFolder = $BotFolder"
Write-Host "ProjFile = $ProjFile"
Write-Host "ArmTemplate = $ArmTemplate"
Write-Host "Lang = $Lang"
Write-Host "Location = $Location"


###
# Connect to Azure Resource Manager
##
Connect-AzureRmAccount



###
#Step 1. Create AD App (from example: https://blogs.msdn.microsoft.com/azuresqldbsupport/2017/09/01/how-to-create-an-azure-ad-application-in-powershell/)
###

# Sign in to Azure.
# Login-AzureRmAccount
# Connect-AzureRmAccount

# If you have multiple subscriptions, uncomment and set to the subscription you want to work with:
# $subscriptionId = "11111111-aaaa-bbbb-cccc-222222222222"
# Set-AzureRmContext -SubscriptionId $subscriptionId

# Provide these values for your new Azure AD app:
# $AppName is the display name for your app, must be unique in your directory
# $uri does not need to be a real URI
# $secret is a password you create

$secsecret = ConvertTo-SecureString -String $secret -AsPlainText -Force
# Create the Azure AD app
$azureAdApplication = New-AzureRmADApplication -DisplayName $AppName -HomePage $Uri -IdentifierUris $Uri -Password $secsecret -AvailableToOtherTenants $true

<####
# Service Principal for the app
$svcprincipal = New-AzureRmADServicePrincipal -ApplicationId $azureAdApplication.ApplicationId

# Assign the Contributor RBAC role to the service principal
# If you get a PrincipalNotFound error: wait 15 seconds, then rerun the following until successful
Write-Output "Waiting 30 seconds to add contributor role to new principal
Start-Sleep -Seconds 30
$roleassignment = New-AzureRmRoleAssignment -RoleDefinitionName Contributor -ServicePrincipalName $azureAdApplication.ApplicationId.Guid
####>

Write-Output "Subscription ID:" (Get-AzureRmContext).Subscription.SubscriptionId
Write-Output "Tenant ID:" (Get-AzureRmContext).Tenant.TenantId
Write-Output "Application ID:" $azureAdApplication.ApplicationId.Guid
Write-Output "Application Secret:" $secret

$SubId = (Get-AzureRmContext).Subscription.SubscriptionId
$TenantId = (Get-AzureRmContext).Tenant.TenantId
$AppId = $azureAdApplication.ApplicationId.Guid
$AppSecret = $secret

##
#Step 2. Create ARM resources
##

Write-Output "az deployment create --location $Location --name $DeploymentName --template-file $ArmTemplate --subscription $SubId --parameters appId=$AppId appSecret=$AppSecret botId=$BotName newServerFarmName=$ServerFarm newWebAppName=$BotWebApp groupName=$ResourceGroup alwaysBuildOnDeploy=$BuildOnDeploy" 
& az deployment create --location $Location --name $DeploymentName --template-file $ArmTemplate --subscription $SubId --parameters appId=$AppId appSecret=$AppSecret botId=$BotName newServerFarmName=$ServerFarm newWebAppName=$BotWebApp groupName=$ResourceGroup alwaysBuildOnDeploy=$BuildOnDeploy


##
#Step 3. Prepared bot deployment files
##

$ProjBotZip=$BotName + "Zip.zip"
$ProjDeploymentFile=$BotFolder + "\.deployment"
$ProjWebConfigFile=$BotFolder + "\web.config"


if(Test-Path $ProjDeploymentFile)
{
  Remove-Item $ProjDeploymentFile
}


if($Lang -eq "Node")
{
  if(-not(Test-Path $ProjWebConfigFile))
  {
      Write-Output "az bot prepare-deploy --code-dir $BotFolder --lang $Lang"
      & az bot prepare-deploy --code-dir $BotFolder --lang $Lang
  }
}
else
{
     Write-Output "az bot prepare-deploy --code-dir $BotFolder --lang $Lang --proj-file-path $ProjFile"
      & az bot prepare-deploy --code-dir $BotFolder --lang $Lang --proj-file-path $ProjFile
}

<# BUGBUG: This logic should have worked but it doesn't...
elseif ($BuildOnDeploy -eq $True)
{
  if(-not(Test-Path $ProjDeploymentFile))
  {
      Write-Output "az bot prepare-deploy --code-dir $BotFolder --lang $Lang --proj-file-path $ProjFile"
      & az bot prepare-deploy --code-dir $BotFolder --lang $Lang --proj-file-path $ProjFile
  }
}
#>

Write-Output "Compressing bot project..."

if (Test-Path $ProjBotZip) 
{
  Remove-Item $ProjBotZip
}

$ZipSrc = $BotFolder + "\*"
Compress-Archive -Path $ZipSrc -DestinationPath $ProjBotZip -Force
Get-ChildItem -Path $ProjBotZip

##
#Step 4. Deploy to Azure
##

Write-Output "Deploying $BotWebApp into RG $ResourceGroup..."
Write-Output "az webapp deployment source  config-zip --src  $ProjBotZip -g $ResourceGroup  -n $BotWebApp $DeploymentTimeoutSec"
az webapp deployment source  config-zip --src  $ProjBotZip -g $ResourceGroup  -n $BotWebApp $DeploymentTimeoutSec

##
#Step 5. Test in webchat/emulator
##