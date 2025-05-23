# Quasimorph Mod Configuration Menu

Mod Configuration Menu (MCM) allows the player to configure mods through the game's native UI.

It adds a new button in the Main Menu called "Mods", where you will be able to configure any mod that has compatibility with it.

## Compatibility

This mod should be compatible with any mod.

## For Developers

Adding MCM support to your mod is fairly easy.

1. Register using its API call <code>ModConfigMenuAPI.RegisterModConfig(modName, configFilePath, onConfigSaved)</code>. 
    1. __modName__ parameter is used for the label and internal identification.
    2. __configFilePath__ must be the full path to the config file including the __.ini__ extension.
    3. __onConfigSaved__ is a callback triggered when the user stores the config for your mod. The config is sent as Dictionary<string, object>.
2. The mod will create a file located in the same folder as your config. This file will be handled by MCM, but can be edited manually by the user with no issue. The name will be your config file name ended with **"_mcm.ini"**.
3. Read your updated config file whenever you want or use the callback to update the config at your liking.

NOTE: To recover your MCM controlled file name, call <code>ModConfigMenuAPI.GetNameForConfigFile(fileName)</code>.

## Support

If you enjoy what I do and want me to keep doing it, you can check my [Ko-Fi](https://ko-fi.com/crynano) page.
I accept suggestions or we can talk if you wish a for a specific mod.

## Credits

- Crynano: Design, Programming and Implementation.
- Special thanks to Raigir, NBK_RedSpy, Lynchantiure and Badryuner for their feedback.
- Thanks to Sergey for the Workshop Icon!

## Resources

- mmaletin: [Color Picker](https://github.com/mmaletin/UnityColorPicker/tree/master)
- Magnum Scriptum: Original UI, Sprites and Color Palette.