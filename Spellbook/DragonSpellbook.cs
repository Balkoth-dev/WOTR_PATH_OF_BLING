using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
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

namespace WOTR_PATH_OF_BLING.Spellbook
{
    class DragonSpellbook
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

                Main.Log("Adding Gold Dragon Spellbook");
            }

            static void PatchGoldenDragonSpellDamageFeature()
            {
                var goldenDragonClass = BlueprintTool.Get<BlueprintCharacterClass>("daf1235b6217787499c14e4e32142523");
                var oracleSpellbook = BlueprintTool.Get<BlueprintSpellbook>("6c03364712b415941a98f74522a81273");
                var angelSpellbook = BlueprintTool.Get<BlueprintSpellbook>("015658ac45811b843b036e4ccc96c772");
                var angelClericSpelllist = BlueprintTool.Get<BlueprintSpellList>("c074062863fbc1e4bab02f9e6e4eb78d");

                var goldenDragonSpelllistGuid = new BlueprintGuid(new Guid("b43a3ebc-27ba-4d93-beaf-f746eba6b2d0"));

                var goldenDragonSpelllist = Helpers.CreateCopy(angelClericSpelllist, bp =>
                {
                    bp.AssetGuid = goldenDragonSpelllistGuid;
                    bp.name = "Gold Dragon Spelllist" + bp.AssetGuid;
                });

                Helpers.AddBlueprint(goldenDragonSpelllist, goldenDragonSpelllistGuid);

                var goldenDragonSpellbookGuid = new BlueprintGuid(new Guid("9a9ced35-fa75-4287-bc87-ba97e29812c5"));

                var goldenDragonSpellbook = Helpers.CreateCopy(angelSpellbook, bp =>
                {
                    bp.AssetGuid = goldenDragonSpellbookGuid;
                    bp.Name = Helpers.CreateString(bp + ".Name", "Gold Dragon");
                    bp.name = "Gold Dragon Spellbook" + bp.AssetGuid;
                    bp.m_CharacterClass = goldenDragonClass.ToReference<BlueprintCharacterClassReference>();
                    bp.m_MythicSpellList = goldenDragonSpelllist.ToReference<BlueprintSpellListReference>();
                    bp.m_SpellList = goldenDragonSpelllist.ToReference<BlueprintSpellListReference>();
                    bp.Spontaneous = true;
                    bp.AllSpellsKnown = true;
                    bp.CastingAttribute = Kingmaker.EntitySystem.Stats.StatType.Charisma;
                });

                Helpers.AddBlueprint(goldenDragonSpellbook, goldenDragonSpellbookGuid);

                if (Main.settings.AddGoldDragonSpellbook == false)
                {
                    return;
                }
                goldenDragonClass.m_Spellbook = goldenDragonSpellbook.ToReference<BlueprintSpellbookReference>();

            }
        }
    }
}
