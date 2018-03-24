using GameArea;
using Loggers;
using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers
{
    public class GameMasterLogger : Logger
    {
        private Logger mainLogger;
        private new StreamWriter sw;
        private string nameGameMasterLogger;
       // private string nameFileGameMaster;

        public GameMasterLogger()
        {
            objectName = "GameMaster";
            mainLogger = Logger.Instance;
            nameGameMasterLogger = "GMlogs.txt";
           
        }

        public GameMasterLogger(string nameFileGameMaster)
        {
            this.nameGameMasterLogger = nameFileGameMaster;
            objectName = "GameMaster";
            mainLogger = Logger.Instance;
        }

        public void LogBoardPieces(GameMaster gameMaster)
        {
            sw = new StreamWriter(nameGameMasterLogger, true);
            Board board = gameMaster.GetBoard;
            DrawBoardPieces(board);
            sw.Close();
        }
        public void LogBoardAgents(GameMaster gameMaster)
        {
            sw = new StreamWriter(nameGameMasterLogger, true);
            Board board = gameMaster.GetBoard;
            DrawBoardAgents(board);
            sw.Close();
        }

        //This method draws agents and ignored fields on a board
        protected void DrawBoardAgents(Board board)//with GoalField
        {
            string line = Const.line;
            uint TaskAreaHeight = board.TaskAreaHeight;
            uint BoardWidth = board.BoardWidth;
            uint GoalAreaHeight = board.GoalAreaHeight;

            //indexing height from 0
            sw.WriteLine("Board :");
            sw.WriteLine("");
            //-------------------------------------------
            uint startHeight = TaskAreaHeight + 2 * GoalAreaHeight - 1;
            uint endHeight = TaskAreaHeight + GoalAreaHeight - 1;
            //-------------- Red Goal Area ----------------
            sw.WriteLine($"{line} Red Goal Area {line}");

            for (uint height = startHeight; height > endHeight; height--)
            {
                string newLine = $"{height} ";//index of height

                for (uint width = 0; width < GoalAreaHeight; width++)
                {
                    //------------drawing objects--------------------------------



                    var field = board.GetField(width, height);

                    if (field.HasAgent())
                    {
                        var player = field.Player;
                        newLine += DrawAgent(player);
                    }
                    else
                    {
                        
                        newLine += DrawIgnoredField();
                    }


                    newLine += " ";
                }

                sw.WriteLine(newLine);
            }
            sw.WriteLine($"{line} Task Area {line}");
            //-------------------------------------------
            startHeight = TaskAreaHeight + GoalAreaHeight - 1;
            endHeight = GoalAreaHeight - 1;
            //-------------- Task Area ----------------
            for (uint height = startHeight; height > endHeight; height--)
            {
                string newLine = $"{height} ";
                for (uint width = 0; width < BoardWidth; width++)
                {
                    //------------drawing objects--------------------------------

                    var field = board.GetField(width, height);

                    if (field.HasAgent())
                    {
                        var player = field.Player;
                        newLine += DrawAgent(player);
                    }
                    else
                    {
                        newLine += DrawIgnoredField();
                    }

                    newLine += " ";
                }
                sw.WriteLine(newLine);
            }
            sw.WriteLine($"{line} Blue Goal Area {line}");

            //-------------------------------------------
            startHeight = GoalAreaHeight - 1;
            endHeight = 0;
            //-------------- Blue Goal Area ----------------
            for (uint height = startHeight; height >= endHeight; height--)//>= because endHeight == 0
            {
                string newLine = $"{height} ";//index of height

                for (uint width = 0; width < GoalAreaHeight; width++)
                {
                    //------------drawing objects--------------------------------

                    var field = board.GetField(width, height);

                    if (field.HasAgent())
                    {
                        var player = field.Player;
                        newLine += DrawAgent(player);
                    }
                    else
                    {
                        newLine += DrawIgnoredField();
                    }


                    newLine += " ";
                }
                sw.WriteLine(newLine);
                if (height == 0)
                    break;
            }
            sw.WriteLine($"{line}{line}");
        }





        private string DrawIgnoredField()
        {
            return "[ig]";
        }





        protected void DrawBoardPieces( Board board)//with GoalField
        {
            string line = Const.line;
            uint TaskAreaHeight = board.TaskAreaHeight;
            uint BoardWidth = board.BoardWidth;
            uint GoalAreaHeight = board.GoalAreaHeight;

            //indexing height from 0
            sw.WriteLine("Board :");
            sw.WriteLine("");
            //-------------------------------------------
            uint startHeight = TaskAreaHeight + 2 * GoalAreaHeight - 1;
            uint endHeight = TaskAreaHeight + GoalAreaHeight - 1;
            //-------------- Red Goal Area ----------------
            sw.WriteLine($"{line} Red Goal Area {line}");

            for (uint height = startHeight; height > endHeight; height--)
            {
                string newLine = $"{height} ";//index of height

                for (uint width = 0; width < GoalAreaHeight; width++)
                {
                    //------------drawing objects--------------------------------


                    var field = board.GetField(width, height);

                    //if on field there is any agent he will be drawn instead of field
                    

                    var goalField = field as GameArea.GoalField;
                    newLine += DrawGoalField(goalField);



                    newLine += " ";
                }

                sw.WriteLine(newLine);
            }
            sw.WriteLine($"{line} Task Area {line}");
            //-------------------------------------------
            startHeight = TaskAreaHeight + GoalAreaHeight - 1;
            endHeight = GoalAreaHeight - 1;
            //-------------- Task Area ----------------
            for (uint height = startHeight; height > endHeight; height--)
            {
                string newLine = $"{height} ";
                for (uint width = 0; width < BoardWidth; width++)
                {
                    //------------drawing objects--------------------------------
                    //location logger's player
                    var field = board.GetField(width, height);

                    //if on field there is any agent he will be drawn instead of field
                   
                    var taskField = field as GameArea.TaskField;
                    newLine += DrawMasterTaskField(taskField);


                    newLine += " ";
                }
                sw.WriteLine(newLine);
            }
            sw.WriteLine($"{line} Blue Goal Area {line}");

            //-------------------------------------------
            startHeight = GoalAreaHeight - 1;
            endHeight = 0;
            //-------------- Blue Goal Area ----------------
            for (uint height = startHeight; height >= endHeight; height--)//>= because endHeight == 0
            {
                string newLine = $"{height} ";//index of height

                for (uint width = 0; width < GoalAreaHeight; width++)
                {
                    //------------drawing objects--------------------------------
                    //location logger's player
                    var field = board.GetField(width, height);

                    //if on field there is any agent he will be drawn instead of field
                   
                    var goalField = field as GameArea.GoalField;
                    newLine += DrawGoalField(goalField);


                    newLine += " ";
                }
                sw.WriteLine(newLine);
                if (height == 0)
                    break;
            }
            sw.WriteLine($"{line}{line}");


        }


        public void Log(GameMaster gameMaster, string decision, string description)
        {
            mainLogger.Log(decision, description);

            StreamWriter sw = new StreamWriter(nameGameMasterLogger, true);
            var dt = DateTime.Now;
            string time = String.Format(Const.timeFormat, dt);

            sw.WriteLine($"{time} {objectName} {decision} {description}");
            sw.Close();
        }





    }

}
