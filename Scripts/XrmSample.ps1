
# Provisioning
# Set-ExecutionPolicy -ExecutionPolicy ByPass
# Install-Module -Name Microsoft.Xrm.Tooling.CrmConnector.PowerShell
# Update-Module -Name Microsoft.Xrm.Tooling.CrmConnector.PowerShell
# Get-Help “Crm”

# $Cred = Get-Credential
# $CRMOrgs = Get-CrmOrganizations -Credential $Cred -DeploymentRegion NorthAmerica –OnlineType Office365
# $CRMOrgs | get-member  # get all members


$CRMOrgs | Sort-Object FriendlyName | Format-table -AutoSize -Property FriendlyName, WebApplicationUrl 