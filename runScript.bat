set workingDirectory="%cd%"

if "%~1"=="" (
    echo No parameters have been provided default number of player in team is 2.
	SET numberPlayerInTeam=4
) else (
    SET numberPlayerInTeam=%1
)

SET pathToDLLFile=\bin\Debug\netcoreapp2.0
SET configurationGameMasterFilePath=%workingDirectory%\ExampleConfig.xml

rem argumenty w komentarzach

rem dotnet %workingDirectory%\Agent%pathToDLLFile%\Player.dll 127.0.0.1 5678 blue %configurationPlayerFilePath%

start dotnet %workingDirectory%\CommunicationServer%pathToDLLFile%\CommunicationServer.dll 127.0.0.1 1100  %configurationPlayerFilePath%

rem 127.0.0.1 5678  %configurationGameMasterFilePath%
rem dotnet %workingDirectory%\GameMaster%pathToDLLFile%\GameMaster.dll 127.0.0.1 1100  %configurationGameMasterFilePath%


rem 127.0.0.1 5678 blue %configurationPlayerFilePath%
rem FOR /l %%G in (1, 1, %numberPlayerInTeam%) DO start dotnet %workingDirectory%\Agent%pathToDLLFile%\Player.dll

rem 127.0.0.1 5678 red %configurationPlayerFilePath%
rem FOR /l %%G in (1, 1, %numberPlayerInTeam%) DO start dotnet %workingDirectory%\Agent%pathToDLLFile%\Player.dll



pause >nul


