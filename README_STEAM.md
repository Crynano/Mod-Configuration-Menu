[h1]Quasimorph Mod Configuration Menu[/h1]

Mod Configuration Menu (MCM) allows the player to configure mods through the game's native UI.

It adds a new button in the Main Menu called "Mods", where you will be able to configure any mod that has compatibility with it.

[h2]Compatibility[/h2]

This mod should be compatible with any mod.

[h2]For Developers[/h2]

Adding MCM support to your mod is fairly easy.
[olist]
[*]Register using its API call [code]ModConfigMenuAPI.RegisterModConfig(modName, configFilePath, onConfigSaved)[/code]
[olist]
[*][b]modName[/b] parameter is used for the label and internal identification.
[*][b]configFilePath[/b] must be the full path to the config file including the [b].ini[/b] extension.
[*][b]onConfigSaved[/b] is a callback triggered when the user stores the config for your mod. The config is sent as Dictionary<string, object>.
[/olist]
[*]The mod will create a file located in the same folder as your config. This file will be handled by MCM, but can be edited manually by the user with no issue. The name will be your config file name ended with [b]"_mcm.ini"[/b].
[*]Read your updated config file whenever you want or use the callback to update the config at your liking.
[/olist]

NOTE: To recover your MCM controlled file name, call [code]ModConfigMenuAPI.GetNameForConfigFile(fileName)[/code]

[h2]Support[/h2]

If you enjoy what I do and want me to keep doing it, you can check my [url=https://ko-fi.com/crynano]Ko-Fi[/url] page.
I accept suggestions or we can talk if you wish a for a specific mod.

[hr]
[h2]Credits[/h2]
[list]
[*]Crynano: Design, Programming and Implementation.
[*]Special thanks to Raigir, NBK_RedSpy, Lynchantiure and Badryuner for their feedback.
[*]Thanks to Sergey for the Workshop Icon!
[/list]

[h2]Resources[/h2]
[list]
[*]mmaletin: [url=https://github.com/mmaletin/UnityColorPicker/tree/master]Color Picker[/url]
[*]Magnum Scriptum: Original UI, Sprites and Color Palette.
[/list]
