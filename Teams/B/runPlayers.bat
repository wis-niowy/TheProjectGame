start "" java -jar game-agent.jar --address localhost --port 5678 --game Endgame --team blue --role leader --conf playerSettings.xml
sleep 3
start "" java -jar game-agent.jar --address localhost --port 5678 --game Endgame --team blue --role member --conf playerSettings.xml
sleep 3
start "" java -jar game-agent.jar --address localhost --port 5678 --game Endgame --team blue --role member --conf playerSettings.xml
sleep 3
start "" java -jar game-agent.jar --address localhost --port 5678 --game Endgame --team red --role leader --conf playerSettings.xml
sleep 3
start "" java -jar game-agent.jar --address localhost --port 5678 --game Endgame --team red --role member --conf playerSettings.xml
sleep 3
start "" java -jar game-agent.jar --address localhost --port 5678 --game Endgame --team red --role member --conf playerSettings.xml