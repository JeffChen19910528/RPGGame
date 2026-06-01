#!/bin/bash
cd "$(dirname "$0")"
clear
echo ""
echo " ============================================"
echo "  RAGE: Chronicles of Darkness"
echo "  暴走：黑暗年代記"
echo " ============================================"
echo ""
echo " Starting game..."
echo ""
dotnet run --project RPGGame.csproj
