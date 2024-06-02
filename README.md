# ExposePatchedMethods

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that exposes what mods you have loaded to your userspace.

it does this by creating a slots that's children are mods, there are 2 types of mod lists,mod names and harmonyids. there are config options to expose these lists under your userspace root and userroot. the harmonyid list has a config option to only show mods that were called before it. mods are called in mod file name alphabetical order.

このModはUserSpaceに対しスロットを追加することで導入しているModの一覧を取得できるようにする [Resonite](https://resonite.com/) の [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) 向けModです。


## Installation
1. Install [ResoniteModLoader]([https://github.com/zkxs/NeosModLoader](https://github.com/resonite-modding-group/ResoniteModLoader)).
2. Place [ExposePatchedMethods.dll](https://github.com/Nytra/ResoniteExposePatchedMethods/releases/latest/download/ExposePatchedMethods.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Resonite logs.
