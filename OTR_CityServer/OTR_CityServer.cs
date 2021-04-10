using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace OTR_CityServer
{
    public class OTR_CityServer : BaseScript
    {
        public OTR_CityServer()
        {
            //Functions
            ServerStartup();

            //Commands

            ClearCommand();
            ResetCommand();
            TimerCommand();

            //Event Handlers
            EventHandlers.Add("Timer", new Action<int, string, bool>(Timer));
            EventHandlers["playerConnecting"] += new Action(OnPlayerConnecting);
            EventHandlers["playerDropped"] += new Action(OnPlayerDropped);
            //EventHandlers["Lobby"] += new Action<bool>(Lobby);
        }

        //Global Variables
        //---------------------------
        bool TimerIsUp;
        string TimerLabelPrefix;
        string TimerLabelText;
        bool timerHasRun = false;
        bool killTimer = false;

        //Game States
        bool LobbyActive;
        bool PreGameActive;
        bool MainGameActive;

        int PlayerCount = 1;

        //Commands/Functions Defined
        //
        //
        //--------------------

        public void ServerStartup() //Runs on resource startup
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Debug.WriteLine("[OTR_City] started.");
            System.Threading.Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void ClearCommand() //Delete all loaded vehicles on all clients
        {


            RegisterCommand("clear", new Action(() =>
            {
                TriggerClientEvent("ClearVehicles");

            }), true);
        }

        public void ResetCommand() //Delete all loaded vehicles and move all players to spawn
        {
            RegisterCommand("reset", new Action(() =>
            {
                TriggerClientEvent("ResetGame");
                

            }), true);
        }

        public void TimerCommand() //Start and display timer for all players
        {
            RegisterCommand("hidetimer", new Action(() =>
            {
                Timer(180, "hideStart", true);
                killTimer = false;

            }), true);

            RegisterCommand("gametimer", new Action(() =>
            {
                Timer(1800, "gameStart", true);
                killTimer = false;

            }), true);
            
            RegisterCommand("stoptimer", new Action(() =>
            {
                Timer(0, "stopTimer", false);
                killTimer = true;

            }), true);
        }

        public void Timer(int Time, string timerType, bool timerHasRun) //create timer
        {
            //timerHasRun = false;
            Debug.WriteLine(Time.ToString());
            Debug.WriteLine(timerType.ToString());
            Debug.WriteLine(timerHasRun.ToString());
            Debug.WriteLine(killTimer.ToString());

            Thread timerSetThread = new Thread(TimerSet);

            if (timerType == "stopTimer")
            {
                killTimer = true;
                Time = 0;
                timerSetThread.Abort();
                //TriggerClientEvent("TimerDisplay", Time, timerType, false);
                Debug.WriteLine("Stopping timer thread");
            }
            else if (timerType == "hideStart" | timerType == "gameStart") 
            {
                killTimer = false;
                timerSetThread.Start();
                Debug.WriteLine("Timer set thread started");                    
            }

            void TimerSet()
            {
                //Time int passed through Timer function
                while (Time != 0) // | killTimer == false
                {
                    if (killTimer == true | Time == 0)
                    {
                        Time = 0;                        
                        timerType = "stopTimer";
                        TriggerClientEvent("TimerDisplay", Time, timerType, false);
                        Debug.WriteLine("breaking while loop");
                        break;
                    }

                    Thread.Sleep(1000);
                    Time = Time - 1;
                    TriggerClientEvent("TimerDisplay", Time, timerType, true);

                }
            }
        }    

        //Game States
        //
        //---------------------

        private async void OnPlayerConnecting()
        {
            await Delay(0);
            PlayerCount = PlayerCount + 1;
            Debug.WriteLine("Playercount:");
            Debug.WriteLine(PlayerCount.ToString());
        }


        private void OnPlayerDropped()
        {
            PlayerCount = PlayerCount - 1;
            Debug.WriteLine("Playercount:");
            Debug.WriteLine(PlayerCount.ToString());
        }


    } 
}
