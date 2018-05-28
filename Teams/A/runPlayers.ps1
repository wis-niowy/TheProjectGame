$localname = hostname
$address = Test-Connection $localname -count 1 | select Ipv4Address | ft -HideTableHeaders | Out-String
$address = "127.0.0.1"
$gameConfigFilePath = "Championship.xml"
$portNumber = 5678




Start-Process -FilePath "dotnet" -ArgumentList  ".\Player\Player.App.dll --port $portNumber --conf $gameConfigFilePath --address $address --game Endgame --team blue --role leader --verbose"

