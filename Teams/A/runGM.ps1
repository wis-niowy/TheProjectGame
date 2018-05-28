$port = 5678
$address = "127.0.0.1"
$gameConfigFilePath = "Championship.xml"

Start-Process -FilePath "dotnet" -ArgumentList  ".\GM\GameMaster.App.dll --port $port --conf $gameConfigFilePath --address $address --game Endgame"
Start-Sleep -s 2

