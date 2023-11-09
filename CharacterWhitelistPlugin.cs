using BepInEx;
using System.Linq;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using CrewBoomAPI;
using CrewBoom;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Reptile;
using System.IO;

namespace CharacterWhitelist
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(CrewBoomGUID, BepInDependency.DependencyFlags.SoftDependency)]
    internal class CharacterWhitelistPlugin : BaseUnityPlugin
    {
        public static ListType ListType => _listType.Value;
        public static bool AlwaysAllowBaseCharacters => _alwaysAllowBaseCharacters.Value;
        public static HashSet<string> CharacterSet
        {
            get;
            private set;
        }
        private const string CrewBoomGUID = "CrewBoom";
        private static ConfigEntry<bool> _alwaysAllowBaseCharacters;
        private static ConfigEntry<ListType> _listType;
        private static ConfigEntry<string> _characterList;

        public static bool IsCharacterAllowed(Characters character)
        {
            var baseCharacter = character <= Characters.MAX;
            if (baseCharacter && AlwaysAllowBaseCharacters)
                return true;
            var characterName = character.ToString();
            if (!baseCharacter)
            {
                if (CrewBoomAPIDatabase.IsInitialized)
                {
                    if (!CrewBoomAPIDatabase.GetUserGuidForCharacter((int)character, out Guid characterGuid))
                        return true;
                    characterName = GetNameForCrewBoomCharacter(characterGuid);
                }
                else
                    return true;
            }
            if (CharacterSet.Contains(characterName))
                return ListType == ListType.Whitelist;
            else
                return ListType == ListType.Blacklist;
        }

        static string GetNameForCrewBoomCharacter(Guid characterGuid)
        {
            // Sucks a bit that I have to publicize CrewBoom but the API doesn't expose this.
            if (CharacterDatabase._characterBundlePaths.TryGetValue(characterGuid, out string filePath))
                return Path.GetFileNameWithoutExtension(filePath);
            return null;
        }

        void Awake()
        {
            Configure();

            if (!IsCrewBoomInstalled())
            {
                Logger.LogWarning($"CrewBoom ain't installed - initializing {PluginInfo.PLUGIN_NAME} now");
                Initialize();
            }
            else
            {
                // IDK if this is necessary but just to be sure, and have our patches run after CrewBoom.
                Logger.LogInfo($"CrewBoom is installed, will wait for it before patching.");
                if (CrewBoomAPIDatabase.IsInitialized)
                    Initialize();
                else
                    CrewBoomAPIDatabase.OnInitialized += Initialize;
            }
        }

        void Configure()
        {
            _alwaysAllowBaseCharacters = Config.Bind("General",
                "AlwaysAllowBaseCharacters",
                true,
                "Makes basegame non-custom characters not affected, so they will be displayed at all times even if not whitelisted."
                );

            _listType = Config.Bind("General",
                "ListType",
                ListType.Whitelist,
                "Type of list, in case you want a whitelist (only allow listed characters) or a blacklist (allow any character except those in the list)"
                );

            _characterList = Config.Bind("General",
                "CharacterList",
                "",
                "Case sensitive comma separated list of character filenames (without the .cbb extension) to whitelist/blacklist (e.g. patrick, akko, reiko, spaceGirl, eightBall)."
                );

            UpdateCharacterList();
            _characterList.SettingChanged += (object sender, EventArgs e) => { UpdateCharacterList(); };
        }

        void UpdateCharacterList()
        {
            var charSet = new HashSet<string>();
            var splitList = _characterList.Value.Split(',');
            foreach(var split in splitList)
            {
                var trimmed = split.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;
                charSet.Add(trimmed);
            }
            CharacterSet = charSet;
        }

        bool IsCrewBoomInstalled()
        {
            return Chainloader.PluginInfos.Keys.Any(x => x == CrewBoomGUID);
        }

        void Initialize()
        {
            CrewBoomAPIDatabase.OnInitialized -= Initialize;
            try
            {
                var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
                harmony.PatchAll();
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} is loaded!");
            }
            catch(Exception e)
            {
                Logger.LogError($"Plugin {PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} failed to load!{Environment.NewLine}{e}");
            }
        }
    }
}
