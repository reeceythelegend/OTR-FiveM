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
            
        }

        //Global Variables
        //---------------------------



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
                TriggerClientEvent("Timer", 60); 

            }), false);
        }







    }
}
