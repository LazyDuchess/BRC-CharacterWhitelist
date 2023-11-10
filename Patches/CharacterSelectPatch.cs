using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace CharacterWhitelist.Patches
{
    [HarmonyPatch(typeof(CharacterSelect))]
    internal static class CharacterSelectPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(CharacterSelect.SetState))]
        private static void SetState_Postfix(CharacterSelect __instance, CharacterSelect.CharSelectState setState)
        {
            if (setState != CharacterSelect.CharSelectState.MAIN_STATE)
            {
                if (CharacterWhitelistUI.Instance != null)
                    CharacterWhitelistUI.Instance.Deactivate();
            }
            else
            {
                if (CharacterWhitelistUI.Instance != null && CharacterWhitelistPlugin.InGameUI)
                    CharacterWhitelistUI.Instance.Activate(__instance.CharactersInCircle[__instance.selectionInCircle].character);
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(nameof(CharacterSelect.MoveSelection))]
        private static void MoveSelection_Postfix(CharacterSelect __instance)
        {
            if (CharacterWhitelistUI.Instance != null && CharacterWhitelistUI.Instance.isActiveAndEnabled)
                CharacterWhitelistUI.Instance.UpdateLabels(__instance.CharactersInCircle[__instance.selectionInCircle].character);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(CharacterSelect.StopCharacterSelect))]
        private static void StopCharacterSelect_Postfix()
        {
            if (CharacterWhitelistUI.Instance != null)
                CharacterWhitelistUI.Instance.Deactivate();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(CharacterSelect.CharSelectUpdate))]
        private static void CharSelectUpdate_Postfix(CharacterSelect __instance)
        {
            if (__instance.state != CharacterSelect.CharSelectState.MAIN_STATE)
                return;
            if (!CharacterWhitelistPlugin.InGameUI)
                return;
            if (CharacterWhitelistUI.Instance != null && CharacterWhitelistUI.Instance.isActiveAndEnabled)
                CharacterWhitelistUI.Instance.UpdateUI(__instance);
            // Button 3 = Cancelled (B)
            // Button 2 = Accepted (A)
            // Button 64 = X
            // Button 48 = Y
            // Button 45 = Left Bumper?
            // Button 47 = Right Bumper?
            /*
            for(var i=0;i<100;i++)
            {
                if (__instance.gameInput.GetButtonNew(i, 0))
                    Debug.Log($"Pressed button {i}");
            }*/
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(CharacterSelect.PopulateListOfSelectableCharacters))]
        private static void PopulateListOfSelectableCharacters_Postfix(CharacterSelect __instance, Player player)
        {
            var filteredCharacterList = __instance.selectableCharacters.Where(CharacterWhitelistPlugin.IsCharacterAllowed).ToList();

            // At no point in the story we have less than 2 characters to choose from, so the game doesn't handle this and just crashes by default. We pad the list here as a quick and dirty fix.
            if (filteredCharacterList.Count < 2)
            {
                var padCharacter = player.character;

                if (filteredCharacterList.Count > 0)
                    padCharacter = filteredCharacterList[0];

                while (filteredCharacterList.Count < 2)
                    filteredCharacterList.Add(padCharacter);
            }

            __instance.selectableCharacters = filteredCharacterList;
        }
    }
}
