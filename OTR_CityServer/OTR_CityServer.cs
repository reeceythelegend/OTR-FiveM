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
        }

        //Global Variables
        //---------------------------
        bool TimerIsUp;
        string TimerLabelPrefix;
        string TimerLabelText;

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
    }
}
