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
		HelpMessage="Enter deployment timeout",
		Position=8
	)][string]$DeploymentTimeoutSec="1000"

 )
 

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


if (-Not (Test-Path $BotFolder)) 
{
  Write-Host "Error: Cannot find $BotFolder"
  Exit
}

$ProjFilePath = Join-Path -Path $BotFolder -ChildPath $ProjFile
if ($Lang -eq "CSharp")
{
    if( -Not (Test-Path $ProjFilePath) ) 
    {
      Write-Host "Error: Cannot find $ProjFilePath"
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


Write-Host "Using Hard-Coded: Location=$Location,"



#debug:
#skip app creation (already created)
if($true)
{
#
# Create AD App (from example: https://blogs.msdn.microsoft.com/azuresqldbsupport/2017/09/01/how-to-create-an-azure-ad-application-in-powershell/)


# Sign in to Azure.
# Login-AzureRmAccount
# Connect-AzureRmAccount
# Other variations:
# If your Azure account is on a non-public cloud, make sure to specify the proper environment 
# example for the German cloud:
# Login-AzureRmAccount -EnvironmentName AzureGermanCloud

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

# Display the values for your application 
Write-Output "Save these values for using them in your application"
Write-Output "Subscription ID:" (Get-AzureRmContext).Subscription.SubscriptionId
Write-Output "Tenant ID:" (Get-AzureRmContext).Tenant.TenantId
Write-Output "Application ID:" $azureAdApplication.ApplicationId.Guid
Write-Output "Application Secret:" $secret


$SubId = (Get-AzureRmContext).Subscription.SubscriptionId
$TenantId = (Get-AzureRmContext).Tenant.TenantId
$AppId = $azureAdApplication.ApplicationId.Guid
$AppSecret = $secret

}
else
{
  $SubId = "SPECIFY_HARD_CODED_SUB_ID"
  $AppId = "SPECIFY_HARD_CODED_APP_ID"
  $AppSecret = "SPECIFY_HARD_CODED_SECRET"
}


Write-Output "az deployment create --location $Location --name $DeploymentName --template-file $ArmTemplate --subscription $SubId --parameters appId=$AppId appSecret=$AppSecret botId=$BotName newServerFarmName=$ServerFarm newWebAppName=$BotWebApp groupName=$ResourceGroup alwaysBuildOnDeploy=$BuildOnDeploy" 
& az deployment create --location $Location --name $DeploymentName --template-file $ArmTemplate --subscription $SubId --parameters appId=$AppId appSecret=$AppSecret botId=$BotName newServerFarmName=$ServerFarm newWebAppName=$BotWebApp groupName=$ResourceGroup alwaysBuildOnDeploy=$BuildOnDeploy



$ProjBotZip=$BotName + "Zip.zip"
$ProjDeploymentFile=$BotFolder + "\.deployment"
$ProjWebConfigFile=$BotFolder + "\web.config"

if($Lang -eq "Node")
{
  if(-not(Test-Path $ProjWebConfigFile))
  {
      Write-Output "az bot prepare-deploy --code-dir $BotFolder --lang $Lang"
      & az bot prepare-deploy --code-dir $BotFolder --lang $Lang
  }
}
elseif ($BuildOnDeploy -eq $True)
{
  if(-not(Test-Path $ProjDeploymentFile))
  {
      Write-Output "az bot prepare-deploy --code-dir $BotFolder --lang $Lang --proj-file-path $ProjFile"
      & az bot prepare-deploy --code-dir $BotFolder --lang $Lang --proj-file-path $ProjFile
  }
}

Write-Output "Compressing bot project..."

if (Test-Path $ProjBotZip) 
{
  Remove-Item $ProjBotZip
}

$ZipSrc = $BotFolder + "\*"
Compress-Archive -Path $ZipSrc -DestinationPath $ProjBotZip -Force
Get-ChildItem -Path $ProjBotZip

Write-Output "Deploying $BotWebApp into RG $ResourceGroup..."
Write-Output "az webapp deployment source  config-zip --src  $ProjBotZip -g $ResourceGroup  -n $BotWebApp -t $DeploymentTimeoutSec"
az webapp deployment source  config-zip --src  $ProjBotZip -g $ResourceGroup  -n $BotWebApp -t $DeploymentTimeoutSec