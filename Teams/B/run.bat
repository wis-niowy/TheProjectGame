start "" java -jar game-communication-server.jar --port 6969 --conf communicationServerSettings.xml
sleep 2
start "" java -jar game-master.jar --port 6969 --address localhost --conf gameMasterSettings.xml
sleep 2
start "" java -jar game-agent.jar --address localhost --port 6969 --game SimpleGame --team blue --role leader --conf playerSettings.xml
start "" java -jar game-agent.jar --address localhost --port 6969 --game SimpleGame --team blue --role member --conf playerSettings.xml
start "" java -jar game-agent.jar --address localhost --port 6969 --game SimpleGame --team blue --role member --conf playerSettings.xml
start "" java -jar game-agent.jar --address localhost --port 6969 --game SimpleGame --team red --role leader --conf playerSettings.xml
start "" java -jar game-agent.jar --address localhost --port 6969 --game SimpleGame --team red --role member --conf playerSettings.xml
start "" java -jar game-agent.jar --address localhost --port 6969 --game SimpleGame --team red --role member --conf playerSettings.xml