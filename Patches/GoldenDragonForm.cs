using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.ActivatableAbilities.Restrictions;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
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
    class GoldenDragonForm
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;

                PatchGoldenDragonFormIcon();
                PatchGoldenDragonFormAbility();

                Main.Log("Patching Golden Dragon Polymorph");
            }

            static void PatchGoldenDragonFormIcon()
            {
                if (Main.settings.PatchGoldenDragonFormIcon == false)
                {
                    return;
                }
                var dragonFormAbillity = BlueprintTool.Get<BlueprintAbility>("a0273cfaafe84f0b89a70b3580568ebc");
                var dragonFormBuff = BlueprintTool.Get<BlueprintBuff>("dbe1d6ac18ad4eafb4f6d24e48eb12dc");

                dragonFormAbillity.m_Icon = AssetLoader.LoadInternal("Abilities", "GoldDragonForm.png");
                dragonFormBuff.m_Icon = dragonFormAbillity.m_Icon;

            }
            static void PatchGoldenDragonFormAbility()
            {
                if (Main.settings.PatchGoldenDragonFormAbility == false)
                {
                    return;
                }
                var dragonFormBuff = BlueprintTool.Get<BlueprintBuff>("dbe1d6ac18ad4eafb4f6d24e48eb12dc");
                var dragonFormAbillity = BlueprintTool.Get<BlueprintAbility>("a0273cfaafe84f0b89a70b3580568ebc");
                var dragonFormAbilityDescription = "You can assume the form of a Gold Dragon. You gain the following abilities: a +10 {g|Encyclopedia:Size}size{/g} " +
                                                   "{g|Encyclopedia:Bonus}bonus{/g} to {g|Encyclopedia:Strength}Strength{/g}, a +8 size bonus to {g|Encyclopedia:Constitution}Constitution{/g}, " +
                                                   "a +8 form natural armor bonus, Blindsense 60 feet. You also gain one bite ({g|Encyclopedia:Dice}2d8{/g}), two claws (2d6), two wing {g|Encyclopedia:Attack}attacks{/g} " +
                                                   "(1d8), and one tail slap attack (2d6).\n You also gain a +2 Mythic bonus to all mental stats.";
                dragonFormAbillity.m_Description = Helpers.CreateString(dragonFormAbillity + ".Description", dragonFormAbilityDescription);
                dragonFormBuff.m_Description = Helpers.CreateString(dragonFormBuff + ".Description", dragonFormAbilityDescription);

                dragonFormBuff.AddComponent<AddContextStatBonus>(c => {
                    c.Multiplier = 2;
                    c.Stat = Kingmaker.EntitySystem.Stats.StatType.Intelligence;
                    c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Mythic;
                    c.Value = 1;
                });
                dragonFormBuff.AddComponent<AddContextStatBonus>(c => {
                    c.Multiplier = 2;
                    c.Stat = Kingmaker.EntitySystem.Stats.StatType.Charisma;
                    c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Mythic;
                    c.Value = 1;
                });
                dragonFormBuff.AddComponent<AddContextStatBonus>(c => {
                    c.Multiplier = 2;
                    c.Stat = Kingmaker.EntitySystem.Stats.StatType.Wisdom;
                    c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Mythic;
                    c.Value = 1;
                });

            }
        }
    }
}