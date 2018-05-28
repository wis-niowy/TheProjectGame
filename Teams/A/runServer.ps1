$localname = hostname
$address = Test-Connection $localname -count 1 | select Ipv6Address | ft -HideTableHeaders | Out-String
$address = $address.Trim()
$gameConfigFilePath = "Championship.xml"
$portNumber = 5678


Start-Process -FilePath "dotnet" -ArgumentList  ".\CS\CommunicationServer.App.dll --port $portNumber --conf $gameConfigFilePath --verbose"
