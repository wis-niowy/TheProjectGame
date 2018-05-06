set workingDirectory=%cd%

if "%~1"=="" (
    echo No parameters have been provided default number of player in team is 2.
	SET numberPlayerInTeam=4
) else (
    SET numberPlayerInTeam=%1
)

SET pathToDLLFile=\bin\Debug\netcoreapp2.0

rem argumenty w komentarzach

rem dotnet %workingDirectory%\Agent%pathToDLLFile%\Player.dll 127.0.0.1 5678 blue %configurationPlayerFilePath%

start dotnet %workingDirectory%\CommunicationServer%pathToDLLFile%\CommunicationServer.dll

rem 127.0.0.1 5678  %configurationGameMasterFilePath%
start dotnet %workingDirectory%\GameMaster%pathToDLLFile%\GameMaster.dll 


rem 127.0.0.1 5678 blue %configurationPlayerFilePath%
FOR /l %%G in (1, 1, %numberPlayerInTeam%) DO start dotnet %workingDirectory%\Agent%pathToDLLFile%\Player.dll

rem 127.0.0.1 5678 red %configurationPlayerFilePath%
FOR /l %%G in (1, 1, %numberPlayerInTeam%) DO start dotnet %workingDirectory%\Agent%pathToDLLFile%\Player.dll



pause >nul


