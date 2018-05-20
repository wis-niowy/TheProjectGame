rem 127.0.0.1 5678  %configurationGameMasterFilePath%
dotnet "%cd%\GameMaster\bin\Debug\netcoreapp2.0\GameMaster.dll" 127.0.0.1 5678  "%cd%\Championship.xml"
pause >nul