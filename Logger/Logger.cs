using GameArea;
using Messages;
using Player;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Loggers
{




    public class Logger
    {
        protected class Const
        {
            public const string timeFormat = "{0:yyyy-mm-ddTHH:mm:ss.fff}";
            public const string line = "------------------";

        }

        // note: StreamWriter is not thread safe
        protected static StreamWriter sw; // main log
        private static Logger instance;
        protected Logger() { }
        protected static int counter = 0;
        protected string objectName = "";
        protected static string nameLog = null;

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    Console.WriteLine("should Initialize first");
                }
                return instance;
            }
        }

        public static void Initialize(string filename = "logs.txt")
        {
            if (instance == null)
            {
                instance = new Logger();
                nameLog = filename;
                //sw = new StreamWriter(filename, true);
            }
        }

        public void Log(string action,string description)
        {
            sw = new StreamWriter(nameLog, true);
            var dt = DateTime.Now;
            string time = String.Format(Const.timeFormat, dt);
            sw.WriteLine($"{time} {objectName} {action} {description}");
            sw.Close();
        }

        //method used for drawing goalfield with description in the board
        protected string DrawGoalField(GameArea.GoalField field)
        {
            string stringField = "";
            var goal = field.GoalType;

            stringField += "[";
            switch (goal)
            {
                case GoalFieldType.goal:
                    stringField += "gf";
                    break;
                case GoalFieldType.nongoal:
                    stringField += "ng";
                    break;
                case GoalFieldType.unknown:
                    stringField += "uk";
                    break;
                default:

                    break;
            }
            stringField += "]";

            return stringField;
        }

        //method used for drawing agent with description in the board
        protected string DrawAgent(Messages.Agent player)
        {
            string stringAgent = "";
            stringAgent += "[";
            if (player.team == TeamColour.red)
                stringAgent += $"r";
            else //player.team == TeamColour.blue
                stringAgent += $"b";

            if (player.type == PlayerType.leader)
                stringAgent += $"l";
            else //player.team == PlayerType.agent
                stringAgent += $"a";

            stringAgent += "]";

            return stringAgent;
        }
        protected string DrawMasterTaskField(GameArea.TaskField field)
        {
            string stringField = "";
            stringField += "[";

            if (field.GetPiece != null)
            {
                Piece piece = field.GetPiece;

                switch(piece.type)
                {
                    case PieceType.normal:
                        stringField += "vp";//there is piece
                        break;
                    case PieceType.sham:
                        stringField += "sp";//there is sham
                        break;
                    case PieceType.unknown:
                        stringField += "uk";//there is uknown
                        break;
                }

                
            }
            else
            {
                stringField += "ef";//there is no any piece
            }

            stringField += "]";
            return stringField;
        }



        ////method used for drawing taskfield with description in the board for Agent
        protected string DrawTaskField(GameArea.TaskField field)
        {
            string stringField = "";
            stringField += "[";

            if (field.GetPiece != null)
            {
                stringField += "pf";//there is piece
            }
            else
            {
                stringField += "ef";//there is no any piece
            }

            stringField += "]";
            return stringField;
        }

    }



   
    



    
}
