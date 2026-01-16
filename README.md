NoTime game - EditorAdditor
====
Mod to make game modding be more comfortable.

For now just for Level Editor.

Move, rotate and resize selected objects by keyboard keys (with modifiers and combinations). Have a config file (will appear at *BepInEx/config*) to remap keys and change modifiers.

Default keys:
- Arrows - move by X-Z axis.
- RCtrl, RShift (hold) - modifiers (tiny and big adjust);
- Space (hold) + arrows: left-right - scale, up-down - height by Y axis;
- Slash (/, hold) + arrows: left-right - rotate by Y axis, up-down - rotate by Z axis.

Can hold arrows for prolong changes.

Known issues: 
- not forget selected object after diselection, so it can still be moved without active Inspector window.

INSTALL
===
- Download BepInEx_UnityIL2CPP_x64_6.0.0-pre.1 version from there: https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.1
- Unzip into game folder *NoTime/64-Bit*
- Run game once to let BepinEx create needful files
- Download mod dll from Releases page: https://github.com/Kastuk/NoTimeEditorAdditor/releases
- Put it into folder *NoTime/64-Bit/BepInEx/plugins*
- Run game, editor and try features!
