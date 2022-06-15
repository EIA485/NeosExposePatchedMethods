# ExposePatchedMethods

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for [Neos](https://neos.com/) that exposes what mods you have loaded to your userspace.

it does this by creating a slots that's children are mods, there are 2 types of mod lists,mod names and harmonyids. there are config options to expose these lists under your userspace root and userroot. the harmonyid list has a config option to only show mods that were called before it. mods are called in mod file name alphabetical order.

このModはUserSpaceに対しスロットを追加することで導入しているModの一覧を取得できるようにする [Neos](https://neos.com/) の [NeosModLoader](https://github.com/zkxs/NeosModLoader) 向けModです。


## Installation
1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
1. Place [ExposePatchedMethods.dll](https://github.com/eia485/NeosExposePatchedMethods/releases/latest/download/ExposePatchedMethods.dll) into your `nml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Neos logs.
