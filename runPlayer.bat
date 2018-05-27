rem 127.0.0.1 5678 kolor %configurationPlayerFilePath%
dotnet "%cd%\Agent\bin\Debug\netcoreapp2.0\Player.dll" --address 127.0.0.1 --port 5678 --team blue --conf "%cd%\PlayerSettings.xml" --role member
pause >nul