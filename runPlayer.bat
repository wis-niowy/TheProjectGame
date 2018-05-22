rem 127.0.0.1 5678 kolor %configurationPlayerFilePath%
dotnet "%cd%\Agent\bin\Debug\netcoreapp2.0\Player.dll" 127.0.0.1 5678 blue "%cd%\Championship.xml"
pause >nul