#!/bin/bash
cd "$(dirname "$0")"
echo ""
echo "  ============================================"
echo "   RPGGame 自動測試套件"
echo "  ============================================"
echo ""
dotnet run -- --test
