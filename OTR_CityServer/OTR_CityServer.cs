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
            EventHandlers.Add("Timer", new Action<int>(Timer));
            EventHandlers["playerConnecting"] += new Action(OnPlayerConnecting);
            EventHandlers["playerDropped"] += new Action(OnPlayerDropped);
            //EventHandlers["Lobby"] += new Action<bool>(Lobby);
        }

        //Global Variables
        //---------------------------
        bool TimerIsUp;
        string TimerLabelPrefix;
        string TimerLabelText;

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
            RegisterCommand("timer", new Action(() =>
            {
                Timer(90);

            }), false);
        }

        public void Timer(int Time) //create timer
        {
            new Thread(new ThreadStart(TimerSet)).Start();
            Debug.WriteLine("Timer set thread started");

            void TimerSet()
            {
                //Time int passed through Timer function
                while (Time != 0)
                {
                    Thread.Sleep(1000);
                    Time = Time - 1;
                    TriggerClientEvent("TimerDisplay", Time);

                    if (Time == 0)
                    {
                        TriggerClientEvent("TimerDisplay", Time);
                        break;
                    }
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
