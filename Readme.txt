AzSvc: Azure Ping Monitoring Service
The program is installed as a Windows service and pings specific Azure Storage table reporting basic 
environment and network information about the source (ping origin) machine.

Usage:
1. Edit AzSvc.exe.config
2. Install the service using Install tools\InstallUtil.exe <path-to-service>AzSvc.exe
3. To run, type: net start "Azure Ping Service" (or use Windows gui)
4. To stop, type: net stop "Azure Ping Service"
5. To test/view progress, check out the AzSvc.log debug file

The populated table depends on the account specified. The default one work under my credentials.

Eyal