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
using System.Text;
using System.Collections;

namespace CharacterWhitelist
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(CrewBoomGUID, BepInDependency.DependencyFlags.SoftDependency)]
    internal class CharacterWhitelistPlugin : BaseUnityPlugin
    {
        public static bool InGameUI => _inGameUI.Value;
        public static ListType ListType
        {
            get
            {
                return _listType.Value;
            }

            set
            {
                _listType.Value = value;
            }
        }
        public static bool AlwaysAllowBaseCharacters
        {
            get
            {
                return _alwaysAllowBaseCharacters.Value;
            }

            set
            {
                _alwaysAllowBaseCharacters.Value = value;
            }
        }
        public static HashSet<string> CharacterSet
        {
            get;
            private set;
        }
        private const string CrewBoomGUID = "CrewBoom";
        private static ConfigEntry<bool> _alwaysAllowBaseCharacters;
        private static ConfigEntry<ListType> _listType;
        private static ConfigEntry<string> _characterList;
        private static ConfigEntry<bool> _inGameUI;

        static string GetCharacterInternalName(Characters character)
        {
            var baseCharacter = character <= Characters.MAX;
            var characterName = character.ToString();
            if (!baseCharacter)
            {
                if (!IsCrewBoomInstalled())
                    return "";
                if (CrewBoomAPIDatabase.IsInitialized)
                {
                    if (!CrewBoomAPIDatabase.GetUserGuidForCharacter((int)character, out Guid characterGuid))
                        return "";
                    characterName = GetNameForCrewBoomCharacter(characterGuid);
                }
                else
                    return "";
            }
            return characterName;
        }

        public static void ClearCharacterList()
        {
            CharacterSet.Clear();
            UpdateCharacterListFromCharacterSet();
        }

        public static void RemoveCharacterFromList(Characters character)
        {
            CharacterSet.Remove(GetCharacterInternalName(character));
            UpdateCharacterListFromCharacterSet();
        }

        public static void AddCharacterToList(Characters character)
        {
            CharacterSet.Add(GetCharacterInternalName(character));
            UpdateCharacterListFromCharacterSet();
        }

        static void UpdateCharacterListFromCharacterSet()
        {
            var charSetList = CharacterSet.ToList();
            var charListBuilder = new StringBuilder();
            for(var i=0;i<charSetList.Count;i++)
            {
                charListBuilder.Append(charSetList[i]);
                if (i < charSetList.Count - 1)
                    charListBuilder.Append(", ");
            }
            _characterList.SettingChanged -= UpdateCharacterListEvent;
            _characterList.Value = charListBuilder.ToString();
            _characterList.SettingChanged += UpdateCharacterListEvent;
        }

        public static bool IsCharacterInList(Characters character)
        {
            return CharacterSet.Contains(GetCharacterInternalName(character));
        }

        public static bool IsCharacterAllowed(Characters character)
        {
            if (ListType == ListType.Disabled)
                return true;
            var baseCharacter = character <= Characters.MAX;
            if (baseCharacter && AlwaysAllowBaseCharacters)
                return true;
            var characterName = GetCharacterInternalName(character);
            if (CharacterSet.Contains(characterName))
                return ListType == ListType.Whitelist;
            else
                return ListType == ListType.Blacklist;
        }

        static string GetNameForCrewBoomCharacter(Guid characterGuid)
        {
            if (CharacterDatabase._characterBundlePaths.TryGetValue(characterGuid, out string filePath))
                return Path.GetFileNameWithoutExtension(filePath);
            return null;
        }

        void Awake()
        {
            Configure();
            Initialize();
        }

        void Configure()
        {
            _inGameUI = Config.Bind("General",
                "InGameUI",
                true,
                "Displays a UI for blacklisting/whitelisting characters at the cypher in-game."
                );

            _alwaysAllowBaseCharacters = Config.Bind("General",
                "AlwaysAllowBaseCharacters",
                false,
                "Makes basegame non-custom characters not affected, so they will be displayed at all times even if not whitelisted."
                );

            _listType = Config.Bind("General",
                "ListType",
                ListType.Disabled,
                "Type of list, in case you want a whitelist (only allow listed characters), a blacklist (allow any character except those on the list) or to disable it, which has no effect."
                );

            _characterList = Config.Bind("General",
                "CharacterList",
                "",
                "Case sensitive comma separated list of character filenames (without the .cbb extension) to whitelist/blacklist (e.g. patrick, akko, reiko, spaceGirl, eightBall)."
                );

            UpdateCharacterList();
            _characterList.SettingChanged += UpdateCharacterListEvent;
        }

        void Initialize()
        {
            StartCoroutine(DelayedInitialization());
        }

        // Wait 1 frame so we patch after all other mods have been initialized, in case they add characters to the cypher or something so our whitelist takes priority.
        IEnumerator DelayedInitialization()
        {
            yield return null;
            try
            {
                var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
                harmony.PatchAll();
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} is loaded!");
            }
            catch (Exception e)
            {
                Logger.LogError($"Plugin {PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} failed to load!{Environment.NewLine}{e}");
            }
        }

        static void UpdateCharacterListEvent(object sender, EventArgs e)
        {
            UpdateCharacterList();
        }

        static void UpdateCharacterList()
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
        internal static bool IsCrewBoomInstalled()
        {
            return Chainloader.PluginInfos.Keys.Any(x => x == CrewBoomGUID);
        }
    }
}
