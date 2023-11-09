using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile;

namespace CharacterWhitelist.Patches
{
    [HarmonyPatch(typeof(CharacterSelect))]
    internal static class CharacterSelectPatch
    {
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
