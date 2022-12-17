using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
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
using WOTR_PATH_OF_BLING.Utilities;

namespace WOTR_PATH_OF_BLING.Patches
{
    class GoldenDragonSpellDamageFeature
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
                if (Main.settings.PatchGoldDragonSpellDamage == false)
                {
                    return;
                }
                var goldenDragonSpellDamageFeature = BlueprintTool.Get<BlueprintFeature>("a83dd922ad1441fd8224bd01a5948331");
                goldenDragonSpellDamageFeature.RemoveComponents<AddSpellDiceBonusTrigger>();
                goldenDragonSpellDamageFeature.AddComponent<GoldenDragonSpellDamage>();

                var x = goldenDragonSpellDamageFeature.ToReference<BlueprintFeatureBaseReference>();

                var goldenDragonProgression = BlueprintTool.Get<BlueprintProgression>("a6fbca43902c6194c947546e89af64bd");

                goldenDragonProgression.LevelEntries[1].m_Features = goldenDragonProgression.LevelEntries[1].m_Features.Where(c => c != x).ToList();
                goldenDragonProgression.LevelEntries[2].m_Features = goldenDragonProgression.LevelEntries[2].m_Features.Where(c => c != x).ToList();
            }
        }
    }
}