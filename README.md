# ExposePatchedMethods

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for [Neos](https://neos.com/) that exposes what mods you have loaded to your userspace. it does this by creating a slot called "Loaded mods" with its children being all of the harmony ids that had been used to patch a method at the time this mod was called. mods are called in mod file name alphabetical order. if you want this mod to exposes all mods then prefix it's file name with zz. if you also want to expose what mods you have loaded under your userroot you can enable key "Show Everywhare" in Config. 

このModはUserSpaceに対しスロットを追加することで導入しているModの一覧を取得できるようにする [Neos](https://neos.com/) の [NeosModLoader](https://github.com/zkxs/NeosModLoader) 向けModです。


## Installation
1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
1. Place [ExposePatchedMethods.dll](https://github.com/eia485/NeosExposePatchedMethods/releases/latest/download/ExposePatchedMethods.dll) into your `nml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Neos logs.