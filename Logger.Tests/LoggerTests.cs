using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loggers;
using System.IO;
using Player;
using Messages;
using Configuration;

namespace LoggerTests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void CreateFileLog()
        {
            string nameFile = "test.txt";
            Logger.Initialize(nameFile);

            Assert.AreEqual(true, File.Exists(Directory.GetCurrentDirectory() + $@"\{nameFile}"), "File exists");



        }
        [TestMethod]
        public void OpenCreatedFileLog()
        {
            string nameFile = "test.txt";
            Logger l = Logger.Instance;

            Assert.IsNotNull(l);
            Assert.AreEqual(true, File.Exists(Directory.GetCurrentDirectory() + $@"\{nameFile}"), "File exists");

        }
        [TestMethod]
        public void LoggerIsWritingCorrectly()
        {
            string nameFile = "test.txt";
            //Clear the log
            FileStream fileStream = File.Open(Directory.GetCurrentDirectory() + $@"\{nameFile}", FileMode.OpenOrCreate);
            fileStream.SetLength(0);
            fileStream.Close();

            string test = "test";
            string description = "LogIsNotEmptyAfterWriting";


            Logger.Initialize(nameFile);
            Logger l = Logger.Instance;
            l.Log(test, description);
            StreamReader sr = new StreamReader(nameFile, true);
            string ret = sr.ReadLine();

            string[] splittedRet = ret.Split(' ');

            //splittedRet[0] == DateTime
            Assert.AreEqual(test, splittedRet[2]);
            Assert.AreEqual(description, splittedRet[3]);


        }

        [TestMethod]
        public void AgentLoggerIsWritingToLogger()
        {
            string nameFile = "test.txt";
            //Clear the log
            FileStream fileStream = File.Open(Directory.GetCurrentDirectory() + $@"\{nameFile}", FileMode.OpenOrCreate);
            fileStream.SetLength(0);
            fileStream.Close();

            Logger.Initialize(nameFile);

            string test = "test";
            string description = "AgentLoggerIsWritingToLogger";

            AgentLogger al = new AgentLogger();
            al.Log(test, description);

            StreamReader sr = new StreamReader(nameFile, true);
            string ret = sr.ReadLine();

            string[] splittedRet = ret.Split(' ');
            //splittedRet[0] == DateTime
            Assert.AreEqual("Agent", splittedRet[1]);
            Assert.AreEqual(test, splittedRet[2]);
            Assert.AreEqual(description, splittedRet[3]);


        }
        [TestMethod]
        public void AgentLoggerIsWritingCorrectly()
        {
            string nameFile = "test.txt";
            string nameFileAgent = "testAgentLogger.txt";
            //Clear the log
            FileStream fileStream = File.Open(Directory.GetCurrentDirectory() + $@"\{nameFile}", FileMode.OpenOrCreate);
            fileStream.SetLength(0);
            fileStream.Close();
            //Clear Agent log
            fileStream = File.Open(Directory.GetCurrentDirectory() + $@"\{nameFileAgent}", FileMode.OpenOrCreate);
            fileStream.SetLength(0);
            fileStream.Close();

            Logger.Initialize(nameFile);


            Player.Agent a = new Player.Agent(TeamColour.blue);
            string nameAgent = "AgentLogger";
            string test = "test";
            string description = "AgentLoggerIsWritingToLogger";

            AgentLogger al = new AgentLogger(nameFileAgent);
            al.Log(a, test, description);

            StreamReader sr = new StreamReader(nameFileAgent, true);
            string ret = sr.ReadLine();

            string[] splittedRet = ret.Split(' ');

            //splittedRet[0] == DateTime

            //Assert.AreEqual(nameAgent, splittedRet[1]);
            //Assert.AreEqual(test, splittedRet[2]);
            //Assert.AreEqual(description, splittedRet[3]);

            Assert.AreEqual(nameAgent, splittedRet[1]);
            Assert.AreEqual(test, splittedRet[2]);
            Assert.AreEqual(description, splittedRet[3]);

        }
        //[TestMethod]
        //public void GameMasterLoggerIsWritingToLogger()
        //{

        //}
        [TestMethod]
        public void GameMasterLoggerIsWritingCorrectly()
        {
            string nameFile = "test.txt";
            string nameFileGameMaster = "testGameMasterLogger.txt";
            //Clear the log
            FileStream fileStream = File.Open(Directory.GetCurrentDirectory() + $@"\{nameFile}", FileMode.OpenOrCreate);
            fileStream.SetLength(0);
            fileStream.Close();
            //Clear GameMaster log
            fileStream = File.Open(Directory.GetCurrentDirectory() + $@"\{nameFileGameMaster}", FileMode.OpenOrCreate);
            fileStream.SetLength(0);
            fileStream.Close();

            Logger.Initialize(nameFile);


            GameMasterSettingsGameDefinition defaultSettings = new GameMasterSettingsGameDefinition();
            GameArea.GameMaster gm = new GameArea.GameMaster(defaultSettings);
            string nameGameMaster = "GameMaster";
            string test = "test";
            string description = "GameMasterLoggerIsWritingCorrectly";

            GameMasterLogger gml = new GameMasterLogger(nameFileGameMaster);
            gml.Log(gm, test, description);

            StreamReader sr = new StreamReader(nameFileGameMaster, true);
            string ret = sr.ReadLine();

            string[] splittedRet = ret.Split(' ');

            //splittedRet[0] == DateTime

            Assert.AreEqual(nameGameMaster, splittedRet[1]);
            Assert.AreEqual(test, splittedRet[2]);
            Assert.AreEqual(description, splittedRet[3]);
        }
        [TestMethod]
        public void AgentLoggerIsDrawingEmptyBoard()
        {
            string nameFile = "test.txt";
            string nameFileAgent = "testGameMasterLogger.txt";
            //Clear the log
            FileStream fileStream = File.Open(Directory.GetCurrentDirectory() + $@"\{nameFile}", FileMode.OpenOrCreate);
            fileStream.SetLength(0);
            fileStream.Close();
            //Clear Agent log
            fileStream = File.Open(Directory.GetCurrentDirectory() + $@"\{nameFileAgent}", FileMode.OpenOrCreate);
            fileStream.SetLength(0);
            fileStream.Close();

            AgentLogger al = new AgentLogger(nameFileAgent);

            //initialize agent with board
            Player.Agent a = new Player.Agent(TeamColour.blue);

            uint widthBoard = 3;
            uint pieceAreaHeight = 2;
            uint goalAreaHeight = 3;

            GameArea.Board board = new GameArea.Board(3, 2, 3);//random numbers
            a.SetBoard(board);

            //initialize expectedResult
            string[] expectedResult = new string[pieceAreaHeight + goalAreaHeight * 2];
            uint startHeight = pieceAreaHeight + goalAreaHeight * 2 - 1;
            uint endHeight = pieceAreaHeight + goalAreaHeight - 1;

            //TODO to function()
            //---goal Red Area--------------
            for (uint height = startHeight; endHeight < height; height--)
            {
                string line = $"{height} ";
                for (uint width = 0; width < widthBoard; width++)
                {
                    line += "[ng] ";
                }

                expectedResult[height] = line;

            }

            //-------------------------------------------
            startHeight = pieceAreaHeight + goalAreaHeight - 1;
            endHeight = goalAreaHeight - 1;

            for (uint height = startHeight; endHeight < height; height--)
            {
                string line = $"{height} ";
                for (uint width = 0; width < widthBoard; width++)
                {
                    line += "[ef] ";
                }

                expectedResult[height] = line;
            }

            //-----goal blue area -----------------------------
            startHeight = goalAreaHeight - 1;
            endHeight = 0;
            for (uint height = startHeight; endHeight <= height; height--)// <= due to endHeight = 0
            {
                string line = $"{height} ";
                for (uint width = 0; width < widthBoard; width++)
                {
                    if (height == 0 && width == 0)
                    {
                        line += "[me] ";
                    }
                    else
                    {
                        line += "[ng] ";
                    }

                }
                expectedResult[height] = line;
                if (height == 0)
                    break;

            }
            string[] result = al.GetStringBoard(a.GetLocation, a.GetBoard);

            Assert.AreEqual(expectedResult.Length, result.Length);
            for (int i = 0; i < expectedResult.Length; i++)
            {
                Assert.AreEqual(expectedResult[i], result[i]);

            }



        }
        //[TestMethod]
        //public void GameMasterLoggerDrawingEmptyPieceBoard()
        //{

        //}
        //[TestMethod]
        //public void GameMasterLoggerDrawingEmptyAgentBoard()
        //{

        //}


    }
}
