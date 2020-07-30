using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; //required for RemoveWantedLevel()
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace OTR_City
{
    public class OTR_CityClient : BaseScript
    {
        public OTR_CityClient()
        {
            //Event Handlers
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["playerSpawned"] += new Action(OnPlayerSpawned);
            EventHandlers["ClearVehicles"] += new Action(ClearVehicles);
            EventHandlers["ResetGame"] += new Action(ResetGame);
            
        }

        //FiveM Native Functions
        //
        //-----------------------------------

        private void OnClientResourceStart(string ManHunt_Helper) //runs on client join, client only commands should be defined here
        {
            if (GetCurrentResourceName() != ManHunt_Helper) return;

            //Start thread to set wanted level to 0
            RemoveWantedLevel();

            //Moves player to golf spawn
            TeleportToSpawnCommand();

        }

        private void OnPlayerSpawned() //Runs on player spawn
        {
            //Give assault rifle on first session spawn ONLY
            AssaultRifleOnJoin();

            //Police ignore player
            PoliceIgnorePlayer(true);

        }


        //Server To Client Functions
        //
        //-----------------------------------

        private void ClearVehicles() //Clear all vehicles on all clients, triggered by "clear" server command
        {
            Vehicle[] allvehicles = World.GetAllVehicles();
            int deletedCount = 0;

            foreach (Vehicle v in allvehicles)
            {
                v.Delete();
                deletedCount = deletedCount + 1;
            }
            string deletedCountString = deletedCount.ToString();
            Debug.WriteLine(deletedCountString);
            
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 66, 200, 245 },
                args = new[] { "[OnTheRun]", $"{deletedCountString} vehicles cleared globally" },
            });

        }
        
        private void ResetGame() //Triggered via server "reset" command, will delete all vehicles and teleport all players to spawn
        {      
            ClearVehicles();
            MoveToSpawn();
        }



        //Client Custom Functions
        //
        //---------------------------------
        
        bool firstSpawn = true;
        public void AssaultRifleOnJoin() //give assault rifle on player spawn, show message on first spawn only
        {

            if (firstSpawn == true)
            {
                Debug.WriteLine("First spawn");
                firstSpawn = false;

                //Give Weapon
                LocalPlayer.Character.Weapons.Give(WeaponHash.AssaultRifle, 9999, false, true);
                SetPedInfiniteAmmo(PlayerPedId(), true, (uint)WeaponHash.AssaultRifle);
                
                TriggerEvent("chat:addMessage", new
                {
                    color = new[] { 66, 200, 245 },
                    args = new[] { "[OnTheRun]", $"Welcome, you have been given an assault rifle with infinite ammo" },
                });

            }
            else
            {
                Debug.WriteLine("Not First spawn");
                LocalPlayer.Character.Weapons.Give(WeaponHash.AssaultRifle, 9999, false, true);
                SetPedInfiniteAmmo(PlayerPedId(), true, (uint)WeaponHash.AssaultRifle);
            }
        }

        public void PoliceIgnorePlayer(bool isIgnored) //Police ignore player
        {
            //isIgnored should be true for police to ignore player
            SetPoliceIgnorePlayer(PlayerId(), isIgnored);
            SetPoliceRadarBlips(false);

        }

        public void RemoveWantedLevel() //Start a thread to constantly set wanted level to 0
        {
            new Thread(new ThreadStart(SetWantedLevel)).Start();
            Debug.WriteLine("removeWantedLevel thread started");
            
            void SetWantedLevel()
            {
                int i = 0;
                while (i == 0)
                {
                    SetMaxWantedLevel(0);
                }
            }
        }

        private void MoveToSpawn() //Move Player to golf course
        {
            var playerPed = PlayerPedId();

            //Coords for golf course carpark: -1382.93, 13.68, 54.98
            float[] golfCoords = { -1382, 13, 53 };
            //teleport and keep vehicle
            SetPedCoordsKeepVehicle(PlayerPedId(), golfCoords[0], golfCoords[1], golfCoords[2]); //Need to have two functions, one for spawn command and one for moving all players

            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 66, 200, 245 },
                args = new[] { "[OnTheRun]", $"Teleported to spawn" },
            });

        }

        //Client Only Commands
        //
        //---------------------------

        private void TeleportToSpawnCommand() //Command to take player to golf course via MoveToSpawn Function
        {
            
            RegisterCommand("spawn", new Action<int, List<object>, string>((source, args, raw) =>
            {
                MoveToSpawn();
                
            }), false);
        }





    }
}
