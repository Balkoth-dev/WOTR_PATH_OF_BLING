using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.Mechanics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOTR_PATH_OF_BLING;
using WOTR_PATH_OF_BLING.MechanicsChanges;

namespace WOTR_PATH_OF_RAGE.Patches
{
    [HarmonyPatch(typeof(BlueprintsCache), "Init")]
    static class BlueprintsCache_Init_Patch
    {
        static bool Initialized;

        static void Postfix()
        {
            if (Initialized) return;
            Initialized = true;

            PatchGoldenDragonSpellDamageFeature();

            Main.Log("Patching Golden Dragon Spell Damage Features");
        }

        static void PatchGoldenDragonSpellDamageFeature()
        {
            var goldenDragonSpellDamageFeature = BlueprintTool.Get<BlueprintFeature>("a83dd922ad1441fd8224bd01a5948331");
            goldenDragonSpellDamageFeature.RemoveComponents<AddSpellDiceBonusTrigger>();
            goldenDragonSpellDamageFeature.AddComponent<GoldenDragonSpellDamage>();
        }
    }
}