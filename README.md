NoTime game - EditorAdditor
====
Mod to make game modding be more comfortable.

For now just Level Editor.

Move, rotate and resize selected objects by keyboard keys (with modifiers and combinations). Have a config file (will appear at *BepInEx/config*) to remap keys and change modifiers.

Default keys (to use when an object is selected):
- Arrows - move by X-Z axis.
- RCtrl, RShift (hold) - modifiers (tiny and big adjust);
- Space (hold) + arrows: left-right - scale, up-down - height by Y axis;
- Slash (/, hold) + arrows: left-right - rotate by Y axis, up-down - rotate by Z axis.
- End key to move camera to be near selected object (not tested enough, may break the cam orientation)

Can hold arrows for prolong changes.

Known issues: 
- water is not seen somehow, so waterlevel fix is disabled by default

Credits: Thanks for **GardenXased** with SpawnMenu mod, I did follow his trail of game code modding: https://www.nexusmods.com/notime/mods/17
https://steamcommunity.com/sharedfiles/filedetails/?id=3638580806

INSTALL
===
- Download BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.755+3fab71a.zip (for Win 10-11 x64 or others) version from there: https://builds.bepinex.dev/projects/bepinex_be (scroll down)
- Unzip into game folder *NoTime/64-Bit*
- Run game once to let BepinEx create needful files
- Download mod dll from Releases page: https://github.com/Kastuk/NoTimeEditorAdditor/releases
- Put it into folder *NoTime/64-Bit/BepInEx/plugins*
- Run game, editor and try features!
