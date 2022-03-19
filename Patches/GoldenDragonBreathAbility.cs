using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.ActivatableAbilities.Restrictions;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
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
    class GoldenDragonBreathAbility
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;

                PatchGoldenDragonBreathAbility();
                UpgradeGoldenDragonBreathDebuff();

                Main.Log("Patching Golden Dragon Breath Ability");
            }

            static void PatchGoldenDragonBreathAbility()
            {
                if (Main.settings.PatchGoldenDragonBreathAbility == false)
                {
                    return;
                }
                var goldenDragonBreathAbility = BlueprintTool.Get<BlueprintAbility>("22862fcda5a1d8a4f91e6154d0d8d721");

                foreach (var cfg in goldenDragonBreathAbility.GetComponents<ContextRankConfig>())
                {
                    if (cfg.m_BaseValueType == ContextRankBaseValueType.MythicLevel)
                    {
                        cfg.m_Progression = ContextRankProgression.AsIs;
                    }
                }

                var newGoldenDragonBreathDescription = "You gain the ability to breath holy energy, {g|Encyclopedia:Attack}attacking{/g} all enemies in a 50-foot cone, dealing " +
                                                "({g|Encyclopedia:Character_Level}character level{/g} plus mythic rank){g|Encyclopedia:Dice}d8{/g} {g|Encyclopedia:Energy_Damage}holy damage{/g}. " +
                                                "A successful {g|Encyclopedia:Saving_Throw}Reflex saving throw{/g} ({g|Encyclopedia:DC}DC{/g} = 10 + your character level + half your mythic rank) halves the " +
                                                "{g|Encyclopedia:Damage}damage{/g}.\nAny enemy hit by the breath attack also gains a -1 {g|Encyclopedia:Penalty}penalty{/g} on all d20 rolls, while all enemies " +
                                                "who failed the Reflex saving throw gain -2 penalty.\nYou can use your breath attack once every 1d4 {g|Encyclopedia:Combat_Round}rounds{/g}.\n" +
                                                "This cooldown is removed on a new round if you are transformed into your golden dragon form.";

                goldenDragonBreathAbility.m_Description = Helpers.CreateString(goldenDragonBreathAbility + ".Description", newGoldenDragonBreathDescription);
                goldenDragonBreathAbility.ActionType = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Free;

                var dragonFormBuffRef = BlueprintTool.Get<BlueprintBuff>("dbe1d6ac18ad4eafb4f6d24e48eb12dc").ToReference<BlueprintUnitFactReference>();

                var goldenDragonBreathCooldown = BlueprintTool.Get<BlueprintBuff>("4e9ddf0456c4d65498ad90fe6e621c3b");
                goldenDragonBreathCooldown.m_Flags = new BlueprintBuff.Flags();
                goldenDragonBreathCooldown.m_DisplayName = Helpers.CreateString(goldenDragonBreathCooldown + ".Name", "Gold Dragon Breath Cooldown");
                goldenDragonBreathCooldown.m_Description = Helpers.CreateString(goldenDragonBreathCooldown + ".Description", "Your Gold Dragon Breath is on cooldown.");

                var conditionalBuffEffects = new Conditional()
                {
                    ConditionsChecker = new ConditionsChecker()
                    {
                        Conditions = new Condition[] {
                            new ContextConditionHasFact() {
                                m_Fact = dragonFormBuffRef
                            }
                        }
                    },
                    IfTrue = new ActionList() { Actions = new GameAction[] { Helpers.Create<ContextActionRemoveSelf>() } },
                    IfFalse = new ActionList()
                };

                goldenDragonBreathCooldown.AddComponent<NewRoundTrigger>(c => {
                    c.NewRoundActions = new ActionList();
                    c.NewRoundActions.Actions = new GameAction[] { conditionalBuffEffects };
                });

                goldenDragonBreathCooldown.AddComponent<CombatStateTrigger>(c => {
                    c.CombatStartActions = new ActionList();
                    c.CombatStartActions.Actions = new GameAction[] { conditionalBuffEffects };
                    c.CombatEndActions = new ActionList();
                    c.CombatEndActions.Actions = new GameAction[] { conditionalBuffEffects };
                });

            }
            static void UpgradeGoldenDragonBreathDebuff()
            {
                if (Main.settings.UpgradeGoldenDragonBreathDebuff == false)
                {
                    return;
                }

                var goldenDragonBreathDebuff = BlueprintTool.Get<BlueprintBuff>("b29c5db31614ae444a9eda0132b71d5d");
                goldenDragonBreathDebuff.Stacking = StackingType.Stack;

                goldenDragonBreathDebuff.EditComponent<ModifyD20>(c =>
                {
                    c.BonusDescriptor = ModifierDescriptor.UntypedStackable;
                });

                var goldenDragonBreathDebuffHalf = BlueprintTool.Get<BlueprintBuff>("462ef3487d0065248ac3052a96892653");
                goldenDragonBreathDebuffHalf.Stacking = StackingType.Stack;

                goldenDragonBreathDebuffHalf.EditComponent<ModifyD20>(c =>
                {
                    c.BonusDescriptor = ModifierDescriptor.UntypedStackable;
                });


            }
        }
    }
}
