@echo off
@rem
@rem Create Sample Bots
@rem


@pushd c:\workspace\Other\src\BotArm

@echo Creating Node.JS Bot
powershell -ExecutionPolicy Bypass -File .\ProvisionBotTest.ps1 -BotFolder C:\workspace\botroot\BBSamples4.3stgum\samples\javascript_nodejs\03.welcome-users -ArmTemplate C:\workspace\botroot\BBSamples4.3stgum\samples\javascript_nodejs\03.welcome-users\deploymentTemplates\all-up-template.json -Lang Node -Password WhoIsThe1stPresidentOf.SpainT0day?

@echo Creating CSharp Bot
powershell -ExecutionPolicy Bypass -File  .\ProvisionBotTest.ps1 -BotFolder C:\workspace\botroot\BBSamples4.3\samples\csharp_dotnetcore\03.welcome-user -ArmTemplate ArmTemplate.json -Lang CSharp -Password WhoIsThe1stPresidentOf.SpainT0day? -ProjFile WelcomeUser.csproj


@echo Creating CSharp Bot
powershell -ExecutionPolicy Bypass -File  .\ProvisionBotTest.ps1 -BotFolder c:\workspace\botroot\TempPlay\ABot32919\ABot32919 -ArmTemplate ArmTemplate.json -Lang CSharp -Password ThereAre10Planets-Somewhere. -ProjFile ABot32919.csproj

@echo creating JS bot from yeoman template (yo botbuilder)
powershell -ExecutionPolicy Bypass -File .\ProvisionBotTest.ps1 -BotFolder c:\workspace\botroot\TempPlay\asdf-32919 -ArmTemplate c:\workspace\botroot\TempPlay\asdf-32919\deploymentTemplates\all-up-template.json -Lang Node -Password WhoIsThe1stPresidentOf.SpainT0day?


popd