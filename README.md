# BRC-CharacterWhitelist
BepInEx plugin that whitelists or blacklists characters from Cyphers. Useful in cases where you might have a lot of characters you don't use to see other players in SlopCrew.

# Configuration
When you enter the Character Select Screen at a Cypher in-game you will be greeted by an UI with button prompts that allow you to manipulate the list. By default, it will be an empty disabled list, so you can see all characters, add them to the list and make it a whitelist or a blacklist.

You will see the changes reflected next time you enter a Cypher. The game will also generate a configuration file named **"CharacterWhitelist.cfg"** in your **"BepInEx/config"** folder, so alternatively you can modify this file to manage your whitelist/blacklist. You can also hide the in-game Cypher blacklist/whitelist UI from this configuration file.

![CharacterWhitelist UI](https://github.com/LazyDuchess/BRC-CharacterWhitelist/assets/42678262/964929c2-f4a3-4f0b-a1c1-90c548a5ef89)

When editing the configuration file, for CrewBoom characters, you have to type their filenames into the list, minus the ".cbb" extension. The list is case sensitive and comma-separated.

# Building from source
You will need to provide a publicized version of the **"Assembly-CSharp.dll"** file which can be found in your "Bomb Rush Cyberfunk_Data/Managed" folder. To publicize it, you can use the [BepInEx.AssemblyPublicizer](https://github.com/BepInEx/BepInEx.AssemblyPublicizer) tool, and paste the result into **"lib/Assembly-CSharp-publicized.dll"** in this project's root directory.

You will also need to provide the **"CrewBoomAPI.dll"** and **"CrewBoomMono.dll"** assemblies in the **"/lib"** folder, from the CrewBoom plugin, and a publicized version of **"CrewBoom.dll"** as **"CrewBoom-publicized.dll"**

The assemblies **"Unity.TextMeshPro.dll"** and **"UnityEngine.UI.dll"** are also necessary. These can be found in your "Bomb Rush Cyberfunk_Data/Managed" folder as well.
