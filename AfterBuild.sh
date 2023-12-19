#!/usr/bin/bash
rm -r KSPDropShadow
mkdir KSPDropShadow
cp bin/Debug/KSPDropShadow.dll KSPDropShadow/
rm -r "$KSP_DEV_FOLDER/GameData/KSPDropShadow/"
cp -r KSPDropShadow/ "$KSP_DEV_FOLDER/GameData/KSPDropShadow"
cd "$KSP_DEV_FOLDER"
./KSP.x86_64
