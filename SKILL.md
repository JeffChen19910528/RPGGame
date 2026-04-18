# Skill: Console RPG Generator（純程式 RPG 產生器）

## 🎯 目標

建立一個「可執行的 Console RPG 遊戲」，使用純 C# 程式碼，不依賴遊戲引擎。

---

## 🧱 技術要求

* 語言：C#
* 執行環境：.NET Console Application
* 可在 Visual Studio 2022 直接執行

---

## 📦 專案結構

/ProjectRoot
├── Program.cs
├── GameManager.cs
├── Player.cs
├── Enemy.cs
├── BattleSystem.cs
├── SkillSystem.cs
├── StoryManager.cs
└── Utils.cs

---

## 🎮 核心系統

### 1. 玩家系統

* HP / MP
* Attack / Defense
* 技能列表

---

### 2. 戰鬥系統

* 回合制戰鬥
* 玩家 vs 敵人
* 指令選單（Attack / Skill / Defend）

---

### 3. 技能系統

* 消耗 MP
* 傷害計算
* 特殊效果（如暴擊）

---

### 4. 劇情系統

* 使用 Console 輸出文字
* 玩家選擇影響劇情

---

### 5. 特殊系統（強制）

「暴走系統」

* 玩家攻擊累積能量
* 滿了進入強化狀態
* 增加攻擊但可能失控

---

## 🗺️ 遊戲流程

1. 劇情開始
2. 遭遇戰鬥
3. 勝利 → 繼續劇情
4. 失敗 → Game Over
5. 多結局

---

## 🔀 結局系統

至少包含：

* 好結局
* 壞結局
* 隱藏結局

---

## 🧠 限制

* ❌ 不可只輸出文字劇情

* ❌ 必須包含完整可執行程式

* ❌ 必須有戰鬥系統

* ✅ 必須可在 Console 執行

* ✅ 必須可互動（玩家輸入）

---

## 📤 輸出要求

1. 完整 C# 程式碼
2. 可直接執行
3. 不需額外套件

---

## 🚀 進階（可選）

* 等級系統
* 裝備系統
* 存檔系統
