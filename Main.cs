using HarmonyLib;
using System;
using System.IO;
using UnityModManagerNet;
using WOTR_PATH_OF_BLING.Utilities;
using ModKit;

namespace WOTR_PATH_OF_BLING
{
    public class Main
    {
        public class Settings : UnityModManager.ModSettings
        {
            public bool PatchGoldDragonSpellDamage = true;
            public bool PatchGoldenDragonBreathAbility = true;
            public bool UpgradeGoldenDragonBreathDebuff = true;
            public bool PatchGoldenDragonFormIcon = true;
            public bool PatchGoldenDragonFormAbility = true;
            public bool AddGoldDragonSpellbook = true;
            public bool PatchDragonLevel3Immunities = true;

            public override void Save(UnityModManager.ModEntry modEntry)
            {
                Save(this, modEntry);
            }
        }
        public static UnityModManager.ModEntry modInfo = null;
        public static Settings settings; 
        private static bool enabled;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            AssetLoader.ModEntry = modEntry;
            modInfo = modEntry;
            settings = Settings.Load<Settings>(modEntry);
            var settingsFile = Path.Combine(modEntry.Path, "Settings.bak");
            var copyFile = Path.Combine(modEntry.Path, "Settings.xml");
            if (File.Exists(settingsFile) && !File.Exists(copyFile))
            {
                File.Copy(settingsFile, copyFile, false);
            }
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            harmony.PatchAll();
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry obj)
        {
            UI.AutoWidth(); UI.Div(0, 15);
            using (UI.VerticalScope())
            {
                UI.Label("SETTINGS WILL NOT BE UPDATED UNTIL YOU RESTART YOUR GAME.".yellow().bold().size(20));
                UI.Toggle("Gold Dragon Spell Damage Fix".bold(), ref settings.PatchGoldDragonSpellDamage);
                if(settings.PatchGoldDragonSpellDamage)
                {
                    UI.Label("Spell Damage Dice Progression is changed to work as written. In addition, if an enemy has any energy vulnerability they'll be vulnerable to the attack at mythic rank 10.".green().size(10));
                }
                else
                {
                    UI.Label("Spell Damage Dice Progression is unchanged.".red().size(10));
                }
                UI.Toggle("Gold Dragon Breath Upgrade".bold(), ref settings.PatchGoldenDragonBreathAbility);
                if (settings.PatchGoldenDragonBreathAbility)
                {
                    UI.Label("The golden dragon breath is now a move action instead of a standard and will scale damage off of your full mythic rank. Cooldown is negated when transformed into your gold dragon form.".green().size(10));
                }
                else
                {
                    UI.Label("Gold Dragon Breath is unchanged.".red().size(10));
                }
                UI.Toggle("Upgrade Gold Dragon Breath Debuff".bold(), ref settings.UpgradeGoldenDragonBreathDebuff);
                if (settings.UpgradeGoldenDragonBreathDebuff)
                {
                    UI.Label("The golden dragon breath's debuff will now stack.".green().size(10));
                }
                else
                {
                    UI.Label("Gold Dragon Breath's debuff is unchanged.".red().size(10));
                }
                UI.Toggle("Gold Dragon Form Icon Update".bold(), ref settings.PatchGoldenDragonFormIcon);
                if (settings.PatchGoldenDragonFormIcon)
                {
                    UI.Label("Gives a unique icon to the Gold Dragon Form.".green().size(10));
                }
                else
                {
                    UI.Label("Gold Dragon Form Icon is unchanged.".red().size(10));
                }
                UI.Toggle("Gold Dragon Form Ability Update".bold(), ref settings.PatchGoldenDragonFormAbility);
                if (settings.PatchGoldenDragonFormAbility)
                {
                    UI.Label("Gives the Gold Dragon Form a mythic bonus of plus two to all mental stats.".green().size(10));
                }
                else
                {
                    UI.Label("Gold Dragon Form Icon is unchanged.".red().size(10));
                }
                UI.Toggle("Gold Dragon Spellbook Added".bold(), ref settings.AddGoldDragonSpellbook);
                if (settings.AddGoldDragonSpellbook)
                {
                    UI.Label("Gives a Gold Dragon a divine spontaneous spellbook.".green().size(10));
                }
                else
                {
                    UI.Label("Spellbook is not added.".red().size(10));
                }
                UI.Toggle("Holy Damage Conversion Toggle".bold(), ref settings.AddGoldDragonSpellbook);
                if (settings.PatchDragonLevel3Immunities)
                {
                    UI.Label("Adds Holy Conversion toggle for Gold Dragon at mythic rank 9".green().size(10));
                }
                else
                {
                    UI.Label("Holy Conversion toggle for Gold Dragon not added.".red().size(10));
                }
            }
        }

        private static bool Unload(UnityModManager.ModEntry arg)
        {
            throw new NotImplementedException();
        }
        public static void Log(string msg)
        {
#if DEBUG
            modInfo.Logger.Log(msg);
#endif
        }

        public bool GetSettingValue(string b)
        {
            return true;
        }
        public static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

    }
}
