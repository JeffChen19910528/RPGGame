@echo off
chcp 65001 >nul
echo.
echo  ============================================
echo   RPGGame 自動測試套件
echo  ============================================
echo.
dotnet run -- --test
echo.
pause
