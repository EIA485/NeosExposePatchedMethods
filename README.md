# ExposePatchedMethods

A [BepisLoader](https://github.com/ResoniteModding/BepisLoader) mod for [Resonite](https://resonite.com/) that exposes what mods you have loaded to your userspace.

it does this by creating a slots that's children are mods, there are 2 types of mod lists, mod names and harmonyids. there are config options to expose these lists under your UserSpace root and UserRoot.

このModはUserSpaceに対しスロットを追加することで導入しているModの一覧を取得できるようにする [Resonite](https://resonite.com/) の [BepisLoader](https://github.com/ResoniteModding/BepisLoader) 向けModです。
## Installation
1. Install [BepisLoader](https://github.com/ResoniteModding/BepisLoader).
1. Place [ExposePatchedMethods.dll](https://github.com/eia485/ExposePatchedMethods/releases/latest/download/ExposePatchedMethods.dll) into your `plugins` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\BepInEx\plugins` for a default install.
1. Start the game. If you want to verify that the mod is working you can check your BepInEx log at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\BepInEx\LogOutput.log`