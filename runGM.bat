rem 127.0.0.1 5678  %configurationGameMasterFilePath%
dotnet "%cd%\GameMaster\bin\Debug\netcoreapp2.0\GameMaster.dll" --address 127.0.0.1 --port 5678 --conf "%cd%\Championship.xml"
pause >nul