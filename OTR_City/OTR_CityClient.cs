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
            Tick += OnTick;
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["playerSpawned"] += new Action(OnPlayerSpawned);
            EventHandlers["ClearVehicles"] += new Action(ClearVehicles);
            EventHandlers["ResetGame"] += new Action(ResetGame);
            EventHandlers["TimerDisplay"] += new Action<int>(TimerDisplay);
            EventHandlers["MoveToLobby"] += new Action(MoveToLobby);
            EventHandlers["MoveToSpawn"] += new Action(MoveToSpawn);
        }

        //Global Variables
        //---------------------------
        string TimerLabelPrefix;
        string TimerLabelText;
        bool firstSpawn = true;

        //Coords for golf course carpark: -1382.93, 13.68, 54.98
        float[] golfCoords = { -1382, 13, 53 };


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

            SpawnCarCommand(); //Testing
            ChangePedCommand();
        }

        private void OnPlayerSpawned() //Runs on player spawn
        {


            //Give assault rifle on first session spawn ONLY
            AssaultRifleOnJoin();

            //Police ignore player
            PoliceIgnorePlayer(true);

            

        }

        public async Task OnTick()
        {
            CustomText(); //Display text from timer

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
            SpawnHunterVehicles();
            SpawnHunterHelis();
        }


        //Client Custom Functions
        //
        //---------------------------------



        public void AssaultRifleOnJoin() //give assault rifle on player spawn, show message on first spawn only
        {

            if (firstSpawn == true)
            {
                Debug.WriteLine("First spawn");
                firstSpawn = false;

                //Remove all weapons and give Weapon
                LocalPlayer.Character.Weapons.RemoveAll(); //Remove all weapons
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
                LocalPlayer.Character.Weapons.RemoveAll(); //Remove all weapons
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

            //Must change value here when adding to array
            float[][] golfSpawnCoords = new float[6][];

            golfSpawnCoords[0] = new float[] { -1399, 100, 52, 188 };
            golfSpawnCoords[1] = new float[] { -1396, 84, 52, 188 };
            golfSpawnCoords[2] = new float[] { -1396, 66, 52, 188 };
            golfSpawnCoords[3] = new float[] { -1393, 53, 52, 188 };
            golfSpawnCoords[4] = new float[] { -1392, 25, 52, 188 };
            golfSpawnCoords[5] = new float[] { -1392, 8, 52, 188 };

            //Generate random number to decide spawn point
            Random r = new Random();
            int rdmNum = r.Next(0, 5);       

            //teleport and keep vehicle
            SetPedCoordsKeepVehicle(PlayerPedId(), golfSpawnCoords[rdmNum][0], golfSpawnCoords[rdmNum][1], golfSpawnCoords[rdmNum][2]); //Need to have two functions, one for spawn command and one for moving all players
            Debug.WriteLine("Moving Player to rdm");
            Debug.WriteLine(rdmNum.ToString());

            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 66, 200, 245 },
                args = new[] { "[OnTheRun]", $"Teleported to spawn" },
            });
        }

        private async void SpawnHunterVehicles()
        {
            string hunterCarModel = "khamelion";

            var modelHash = (uint)GetHashKey(hunterCarModel);
            if (IsModelInCdimage(modelHash))
            {
  
                float[][] hunterCarCoords = new float[24][];

                //Right hand side car spawn
                hunterCarCoords[0] = new float[] {-1393, 97, 54, 132};
                hunterCarCoords[1] = new float[] { -1393, 90, 54, 132 };
                hunterCarCoords[2] = new float[] { -1393, 83, 54, 132 };
                hunterCarCoords[3] = new float[] { -1393, 76, 54, 132 };
                hunterCarCoords[4] = new float[] { -1390, 69, 54, 132 };
                hunterCarCoords[5] = new float[] { -1390, 62, 54, 132 };
                hunterCarCoords[6] = new float[] { -1390, 55, 54, 132 };
                hunterCarCoords[7] = new float[] { -1390, 48, 54, 132 };
                hunterCarCoords[8] = new float[] { -1388, 41, 54, 132 };
                hunterCarCoords[9] = new float[] { -1387, 34, 54, 132 };
                hunterCarCoords[10] = new float[] { -1387, 27, 54, 132 };
                hunterCarCoords[11] = new float[] { -1387, 20, 54, 132 };

                //Left hand side car spawn
                hunterCarCoords[12] = new float[] { -1403, 97, 54, 236 };
                hunterCarCoords[13] = new float[] { -1403, 90, 54, 236 };
                hunterCarCoords[14] = new float[] { -1403, 83, 54, 236 };
                hunterCarCoords[15] = new float[] { -1403, 76, 54, 236 };
                hunterCarCoords[16] = new float[] { -1403, 69, 54, 236 };
                hunterCarCoords[17] = new float[] { -1403, 62, 54, 236 };
                //
                hunterCarCoords[18] = new float[] { -1401, 55, 54, 236 };
                hunterCarCoords[19] = new float[] { -1401, 48, 54, 236 };
                hunterCarCoords[20] = new float[] { -1401, 41, 54, 236 };
                hunterCarCoords[21] = new float[] { -1401, 34, 54, 236 };
                hunterCarCoords[22] = new float[] { -1398, 27, 54, 236 };
                hunterCarCoords[23] = new float[] { -1398, 20, 54, 236 };

                for (int c = 0;  c < hunterCarCoords.Length; c++)
                {
                    RequestModel(modelHash);
                    while (!HasModelLoaded(modelHash))
                    {
                        await Delay(0);
                    }
                    var vehicle = CreateVehicle(modelHash, hunterCarCoords[c][0], hunterCarCoords[c][1], hunterCarCoords[c][2], hunterCarCoords[c][3], true, false);
                    SetVehicleColours(vehicle, 135, 135);
                }
            }
        }
        private async void SpawnHunterHelis()
        {
            string hunterHeliModel = "polmav";

            var modelHash = (uint)GetHashKey(hunterHeliModel);
            if (IsModelInCdimage(modelHash))
            {          
                float[][] hunterHeliCoords = new float[3][];

                //Right hand side car spawn
                hunterHeliCoords[0] = new float[] { -1400, 125, 54, 180 };
                hunterHeliCoords[1] = new float[] { -1376, 163, 57, 156 };
                hunterHeliCoords[2] = new float[] { -1387, 143, 56, 155 };

                for (int c = 0; c < hunterHeliCoords.Length; c++)
                {
                    RequestModel(modelHash);
                    while (!HasModelLoaded(modelHash))
                    {
                        await Delay(0);
                    }
                    var vehicle = CreateVehicle(modelHash, hunterHeliCoords[c][0], hunterHeliCoords[c][1], hunterHeliCoords[c][2], hunterHeliCoords[c][3], true, false);
                }
            }
        }
        async void ChangePlayerPed()
        {
            
            //var playerModel = new Model("a_m_m_soucent_03");
            await Game.Player.ChangeModel(RandomPlayerModel.RdmPlayerModel());
            Debug.WriteLine("Changing player ped to:");
            Debug.WriteLine(RandomPlayerModel.RdmPlayerModel().ToString());

        }

        public void TimerDisplay(int Time) //Get timer time from server
        {
            if (Time == 0)
            {
                TimerLabelPrefix = "";
            }
            else
            {
                TimeSpan MinSec = TimeSpan.FromSeconds(Time);
                TimerLabelPrefix = "Time Remaining: ~a~";
                TimerLabelText = MinSec.ToString(@"mm\:ss");
            }
        }

        public void CustomText() //Display text in top right corner (used for timer)
        {
            AddTextEntry("TestLabel", TimerLabelPrefix);
            BeginTextCommandDisplayText("TestLabel");
            AddTextComponentSubstringPlayerName(TimerLabelText);
            SetTextFont(4);
            SetTextOutline();
            SetTextScale(0.5f, 0.5f);
            EndTextCommandDisplayText(0.9f, 0.05f);
            
        }

        public void MoveToLobby()
        {
            Debug.WriteLine("Moving player");
            SetEntityCoords(PlayerPedId(), 1750, 3274, 41, false, false, false, false);
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

        private void SpawnCarCommand() //Testing command - to be deleted
        {

            RegisterCommand("car", new Action<int, List<object>, string>((source, args, raw) =>
            {
                SpawnHunterVehicles();
                SpawnHunterHelis();

            }), false);
        }

        private void ChangePedCommand() //Testing command - to be deleted
        {

            RegisterCommand("ped", new Action<int, List<object>, string>((source, args, raw) =>
            {
                ChangePlayerPed();

            }), false);
        }

    }
}
