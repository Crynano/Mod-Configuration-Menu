# Quasimorph Mod Configuration Menu

Mod Configuration Menu (MCM) allows the player to configure mods through the game's native UI.

It adds a new button in the Main Menu called "Mods", where you will be able to configure any mod that has compatibility with it.

## Compatibility

MCM is compatible with ANY mod currently in the workshop.
It will not break between updates as it uses nearly-0 game functions and 0 code patches.

## For Developers

Adding MCM support to your mod is fairly easy.
>Note: Since 0.9.6 there's a new way to add MCM to your mod. The old one is deprecated and should not be used.

1. Register using its API call <code>ModConfigMenuAPI.RegisterModConfig(string, List<IConfigValue>, ConfigStoredDelegate)</code>. 
    1. __modName__ parameter is used for the label and internal identification.
    2. __configFilePath__ Requires a list of IConfigValue. These are your configuration settings for each of your mod variables. 
    3. __onConfigSaved__ is a callback triggered when the user stores the config for your mod. The config is sent as Dictionary<string, object>.
2. Whenever the user saves your mod config, the OnConfigSaved event is triggered, and all the values are sent indexed by their configured name.
3. You must then validate the data and store it however you please.

You can also provide localization keys in any string field.

## Support

Your supports keeps me updating my mods. You can do so via my [Ko-Fi](https://ko-fi.com/crynano) page.

## Credits

- Crynano: Design, Programming and Implementation.
- Special thanks to Raigir, NBK_RedSpy, Lynchantiure, ARZUMATA and Badryuner for their feedback.
- Thanks to Sergey for the Workshop Icon!

## Resources

- mmaletin: [Color Picker](https://github.com/mmaletin/UnityColorPicker/tree/master)
- Magnum Scriptum: Original UI, Sprites and Color Palette.
