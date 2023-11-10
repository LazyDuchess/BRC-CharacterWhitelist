using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;

namespace CharacterWhitelist.Patches
{
    [HarmonyPatch(typeof(GameplayUI))]
    internal static class GameplayUIPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayUI.Init))]
        private static void Init_Postfix(GameplayUI __instance)
        {
            CharacterWhitelistUI.InitializeUI(__instance);
        }
    }
}
