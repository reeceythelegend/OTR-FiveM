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
    public class RandomPlayerModel
    {
        public static Model RdmPlayerModel()
        {
            int modelCount = 0;

            Model[] playerModel = new Model[]
            {
                "a_f_m_eastsa_01",
                "a_f_m_eastsa_02",
                "a_f_m_fatbla_01",
                "a_f_y_bevhills_01",
                "a_f_y_bevhills_02",
                "a_f_y_bevhills_03",
                "a_f_y_business_01",
                "a_f_y_business_04",
                "a_f_y_hipster_02",
                "a_f_y_hipster_03",
                "a_f_y_hipster_04",
                "a_f_y_vinewood_03",
                "a_f_y_vinewood_04",
                "a_m_m_bevhills_01",
                "a_m_m_bevhills_02",
                "a_m_m_business_01",
                "a_m_m_genfat_01",
                "a_m_m_hasjew_01",
                "a_m_m_ktown_01",
                "a_m_m_malibu_01",
                "a_m_m_prolhost_01",
                "a_m_m_rurmeth_01",
                "a_m_m_salton_03",
                "a_m_m_skidrow_01",
                "a_m_m_soucent_04",
                "a_m_m_tourist_01",
                "a_m_m_tramp_01",
                "a_m_o_soucent_01",
                "a_m_y_beach_01",
                "a_m_y_beachvesp_02",
                "a_m_y_bevhills_01",
                "a_m_y_bevhills_02",
                "a_m_y_business_01",
                "a_m_y_business_02",
                "a_m_y_clubcust_01",
                "a_m_y_downtown_01",
                "a_m_y_eastsa_02",
                "a_m_y_vinewood_01",
                "a_m_y_vinewood_02",
                "a_m_y_vinewood_03",
                "a_m_y_vinewood_04",
            };
          
            foreach (Model m in playerModel)
            {
                modelCount = modelCount + 1;
            }

            //Generate random number to select player model
            Random r = new Random();
            int rdmNum = r.Next(0, modelCount);

            Model rdmPlayerModel = playerModel[rdmNum];
            return rdmPlayerModel;
        }

    }

}