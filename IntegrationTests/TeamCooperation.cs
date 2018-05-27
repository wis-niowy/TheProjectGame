using GameArea.GameObjects;
using GameArea.AppConfiguration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Configuration;
using System.Threading;
using GameMasterMain;
using GameMaster;
using System.Net;
using System;
using CommunicationServer;
using Player;
using Messages;
using System.Threading.Tasks;
using GameArea;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace IntegrationTests
{
    [TestClass]
    public class TeamCooperation
    {
        /// <summary>
        /// CS - nasz
        /// GM - Rysiek
        /// </summary>
        [TestMethod]
        public void test1()
        {
            var ip = IPAddress.Parse("127.0.0.1");
            Int32 port = 5678;

            CS.TcpHelper.StartServer(ip, port);
            Thread t = new Thread(new ThreadStart(CS.TcpHelper.Listen));
            t.Start();
            Thread.Sleep(400);

            Thread t2 = new Thread(new ThreadStart(ExecuteCommand));
            t2.Start();
            Thread.Sleep(400);

            var dupa = CS.TcpHelper.manager.defaultController.gameDefinitions.Count;
            System.Diagnostics.Debug.WriteLine(dupa);
        }


        static void ExecuteCommand()
        {
            string str = "C:/Users/Michal/Documents/Visual Studio 2017/Projects/IO/master/runGM.bat";
            
            if (File.Exists(str))
            {
                ProcessStartInfo processInfo;
                Process proc;
                processInfo = new ProcessStartInfo(str);

                processInfo.CreateNoWindow = false;
                processInfo.UseShellExecute = true;
                //processInfo.RedirectStandardError = true;
                //processInfo.RedirectStandardOutput = true;
                proc = Process.Start(processInfo);
               
                Thread.Sleep(10000);
                proc.Kill();
            }
            else throw new FileNotFoundException();
                
        }

    }
}
