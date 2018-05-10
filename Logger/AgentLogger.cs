//using System;
using Loggers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using Player;
using GameArea;


using System.IO;

using System.Xml;
using System;

namespace Loggers
{
    public class AgentLogger : Logger
    {
        private Logger mainLogger;
        private new StreamWriter sw;
        private int myId;
        string nameAgentLogger = null;


        public AgentLogger()
        {

            objectName = "Agent";
            myId = counter++;
            mainLogger = Logger.Instance;
            nameAgentLogger = $"log{myId}.txt";
            //
        }
        public AgentLogger(string _nameAgentLogger)
        {

            objectName = "AgentLogger";
            //myId = counter++;
            mainLogger = Logger.Instance;
            nameAgentLogger = _nameAgentLogger;
            //
        }



        public void Log(Player.Agent agent, string action, string description, bool isAction = false)
        {
            mainLogger.Log(action, description);

            StreamWriter sw = new StreamWriter(nameAgentLogger, true);

            var dt = DateTime.Now;
            string time = String.Format(Const.timeFormat, dt);

            sw.WriteLine($"{time} {objectName} {action} {description}");

            if (isAction)
            {
                
                DrawBoard(agent.GetLocation,agent.GetBoard);
            }
            sw.Close();
                
        }

        //--- Use LogBeforeActionState and LogCurrent state if you want pre-post action board logs ------------------------
        public void LogBeforeActionState(Player.Agent agent, string action, string description)
        {
            StreamWriter sw = new StreamWriter(nameAgentLogger, true);

            sw.WriteLine($"Before Action");
            DrawBoard(agent.GetLocation,agent.GetBoard);

            var dt = DateTime.Now;
            string time = String.Format(Const.timeFormat, dt);
            sw.WriteLine($"");
            sw.WriteLine($"{time} {objectName} {action} {description}");

            sw.Close();

        }
        public void LogCurrentState(Player.Agent agent)
        {
            StreamWriter sw = new StreamWriter(nameAgentLogger, true);

            sw.WriteLine($"");
            sw.WriteLine($"After Action");
            DrawBoard(agent.GetLocation,agent.GetBoard);

            sw.Close();
        }
        //--------------------------------------------------------------------------------

        protected void DrawBoard(Location locationPlayer,Board board)
        {
            string line = Const.line;
            uint TaskAreaHeight = board.TaskAreaHeight;
            uint BoardWidth = board.BoardWidth;
            uint GoalAreaHeight = board.GoalAreaHeight;

            //indexing height from 0
            sw.WriteLine("Board :");
            sw.WriteLine("");
            //-------------------------------------------
            uint startHeight = TaskAreaHeight + 2 * GoalAreaHeight -1;
            uint endHeight = TaskAreaHeight + GoalAreaHeight -1;
            //-------------- Red Goal Area ----------------
            sw.WriteLine($"{line} Red Goal Area {line}");

            for (uint height = startHeight ; height > endHeight; height--)
            {
                string newLine = $"{height} ";//index of height

                for (uint width = 0; width < GoalAreaHeight; width++)
                {
                    //------------drawing objects--------------------------------
                    //location logger's player
                    if (locationPlayer.x == width && locationPlayer.y == height)
                    {
                        newLine += DrawMainPlayer();
                        continue;
                    }
                    else
                    {
                        var field = board.GetField(width, height);

                        //if on field there is any agent he will be drawn instead of field
                        if (field.HasAgent())
                        {
                            var player = field.Player;
                            newLine += DrawAgent(player);
                        }
                        else
                        {
                            var goalField = field as GameArea.GoalField;
                            newLine += DrawGoalField(goalField);
                        }
                    }

                    newLine += " ";
                }

                sw.WriteLine(newLine);
            }
            sw.WriteLine($"{line} Task Area {line}");
            //-------------------------------------------
            startHeight = TaskAreaHeight +  GoalAreaHeight -1;
            endHeight = GoalAreaHeight -1;
            //-------------- Task Area ----------------
            for (uint height = startHeight; height > endHeight; height--)
            {
                string newLine = $"{height} ";
                for (uint width = 0; width < BoardWidth; width++)
                {
                    //------------drawing objects--------------------------------
                    //location logger's player
                    if (locationPlayer.x == width && locationPlayer.y == height)
                    {
                        newLine += DrawMainPlayer();
                        continue;
                    }
                    else
                    {
                        var field = board.GetField(width, height);


                        //if on field there is any agent he will be drawn instead of field
                        if (field.HasAgent())
                        {
                            var player = field.Player;
                            newLine += DrawAgent(player);
                        }
                        else
                        {
                            var taskField = field as GameArea.TaskField;
                            newLine += DrawTaskField(taskField);
                        }
                    }
                   
                    newLine += " ";
                }
                sw.WriteLine(newLine);
            }
            sw.WriteLine($"{line} Blue Goal Area {line}");

            //-------------------------------------------
            startHeight = GoalAreaHeight -1;
            endHeight = 0;
            //-------------- Blue Goal Area ----------------
            for (uint height = startHeight; height >= endHeight; height--)//>= because endHeight == 0
            {
                string newLine = $"{height} ";//index of height

                for (uint width = 0; width < GoalAreaHeight; width++)
                {
                    //------------drawing objects--------------------------------
                    //location logger's player
                    if (locationPlayer.x == width && locationPlayer.y == height)
                    {
                        newLine += DrawMainPlayer();
                       
                    }
                    else
                    {
                        var field = board.GetField(width, height);

                        //if on field there is any agent he will be drawn instead of field
                        if (field.HasAgent())
                        {
                            var player = field.Player;
                            newLine += DrawAgent(player);
                        }
                        else
                        {
                            var goalField = field as GameArea.GoalField;
                            newLine += DrawGoalField(goalField);
                        }
                    }
                    
                    newLine += " ";
                }
                sw.WriteLine(newLine);
            }
            sw.WriteLine($"{line}{line}");


        }
        public string[] GetStringBoard(Location locationPlayer,Board board)//method for test purpose
                                                                           //It is rewrited DrawBoard method to return string[]
        {

            string line = Const.line;
            uint TaskAreaHeight = board.TaskAreaHeight;
            uint BoardWidth = board.BoardWidth;
            uint GoalAreaHeight = board.GoalAreaHeight;

            string[] Result = new string[TaskAreaHeight + 2 * GoalAreaHeight];

            //indexing height from 0
            //-------------------------------------------
            uint startHeight = TaskAreaHeight + 2 * GoalAreaHeight - 1;
            uint endHeight = TaskAreaHeight + GoalAreaHeight - 1;
            //-------------- Red Goal Area ----------------
            for (uint height = startHeight; height > endHeight; height--)
            {
                string newLine = $"{height} ";//index of height

                for (uint width = 0; width < GoalAreaHeight; width++)
                {
                    //------------drawing objects--------------------------------
                    //location logger's player
                    if (locationPlayer.x == width && locationPlayer.y == height)
                    {
                        newLine += DrawMainPlayer();
                        continue;
                    }
                    else
                    {
                        var field = board.GetField(width, height);

                        //if on field there is any agent he will be drawn instead of field
                        if (field.HasAgent())
                        {
                            var player = field.Player;
                            newLine += DrawAgent(player);
                        }
                        else
                        {
                            var goalField = field as GameArea.GoalField;
                            newLine += DrawGoalField(goalField);
                        }
                    }

                    newLine += " ";
                }

                Result[height] = newLine;
            }
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
                    if (locationPlayer.x == width && locationPlayer.y == height)
                    {
                        newLine += DrawMainPlayer();
                        continue;
                    }
                    else
                    {
                        var field = board.GetField(width, height);


                        //if on field there is any agent he will be drawn instead of field
                        if (field.HasAgent())
                        {
                            var player = field.Player;
                            newLine += DrawAgent(player);
                        }
                        else
                        {
                            var taskField = field as GameArea.TaskField;
                            newLine += DrawTaskField(taskField);
                        }
                    }

                    newLine += " ";
                }
                Result[height] = newLine;
            }
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
                    if (locationPlayer.x == width && locationPlayer.y == height)
                    {
                        newLine += DrawMainPlayer();

                    }
                    else
                    {
                        var field = board.GetField(width, height);

                        //if on field there is any agent he will be drawn instead of field
                        if (field.HasAgent())
                        {
                            var player = field.Player;
                            newLine += DrawAgent(player);
                        }
                        else
                        {
                            var goalField = field as GameArea.GoalField;
                            newLine += DrawGoalField(goalField);
                        }
                    }

                    newLine += " ";
                }
                Result[height] = newLine;
                if (height == 0)
                    break;
            }
            return Result;
        }


        private string DrawMainPlayer()
        {
            return "[me]";
        }
    }
}
