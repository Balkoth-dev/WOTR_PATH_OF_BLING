using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
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
    class DragonLevel3Immunities
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;

                PatchDragonLevel3Immunities();

                Main.Log("Patching Golden Dragon Spell Damage Features");
            }

            static void PatchDragonLevel3Immunities()
            {
                var holyConversionIcon = AssetLoader.LoadInternal("Abilities", "HolyConversion.png");
                var newName = "Holy Conversion";
                var newDescription = "Toggling this ability will convert all outgoing damage to Holy damage.";
                var damageConvertBuffGuid = new BlueprintGuid(new Guid("f1a72955-1308-4fb0-b0b9-d7ec007d3811"));
                var damageConvertActivatableAbilityGuid = new BlueprintGuid(new Guid("5a4bbe62-a840-48a6-a436-8571a50562f9"));
                
                var damageConvertBuff = Helpers.Create<BlueprintBuff>(c => {
                    c.AssetGuid = damageConvertBuffGuid;
                    c.name = newName + c.AssetGuid;
                    c.m_DisplayName = Helpers.CreateString(c + ".Name", newName);
                    c.m_Description = Helpers.CreateString(c + ".Description", newDescription);
                    c.m_Icon = holyConversionIcon;
                    c.m_Flags = BlueprintBuff.Flags.HiddenInUi;
                });

                damageConvertBuff.AddComponent<ChangeOutgoingDamageType>(c => {
                    c.m_Flags = 0;
                    c.Type = new Kingmaker.RuleSystem.Rules.Damage.DamageTypeDescription()
                    {
                        Energy = Kingmaker.Enums.Damage.DamageEnergyType.Holy,
                        Type = Kingmaker.RuleSystem.Rules.Damage.DamageType.Energy,
                        Common = new Kingmaker.RuleSystem.Rules.Damage.DamageTypeDescription.CommomData() { Reality = 0, Alignment = 0, Precision = false },
                        Physical = new Kingmaker.RuleSystem.Rules.Damage.DamageTypeDescription.PhysicalData() { Material = 0, Form = Kingmaker.Enums.Damage.PhysicalDamageForm.Slashing, Enhancement = 0, EnhancementTotal = 0 }
                    };
                });

                Helpers.AddBlueprint(damageConvertBuff, damageConvertBuffGuid);


                var damageConvertActivatableAbility = Helpers.Create<BlueprintActivatableAbility>(c => {
                    c.AssetGuid = damageConvertActivatableAbilityGuid;
                    c.name = newName + c.AssetGuid;
                    c.m_DisplayName = Helpers.CreateString(c + ".Name", newName);
                    c.m_Description = Helpers.CreateString(c + ".Description", newDescription);
                    c.m_Icon = holyConversionIcon;
                    c.IsOnByDefault = true;
                    c.ActivationType = AbilityActivationType.Immediately;
                    c.m_ActivateWithUnitCommand = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Free;
                    c.m_ActivateOnUnitAction = AbilityActivateOnUnitActionType.Attack;
                    c.Group = ActivatableAbilityGroup.None;
                    c.WeightInGroup = 1;
                    c.DeactivateAfterFirstRound = false;
                    c.DeactivateIfCombatEnded = false;
                    c.DeactivateImmediately = true;
                    c.IsTargeted = false;
                    c.DeactivateIfOwnerDisabled = false;
                    c.OnlyInCombat = false;
                    c.m_AllowNonContextActions = false;
                    c.m_Buff = damageConvertBuff.ToReference<BlueprintBuffReference>();
                });

                Helpers.AddBlueprint(damageConvertActivatableAbility, damageConvertActivatableAbilityGuid);

                if (Main.settings.PatchDragonLevel3Immunities == false)
                {
                    return;
                }
                var dragonLevel3Immunities = BlueprintTool.Get<BlueprintFeature>("f1631a20b6f14e58924a32c81da95840");
                dragonLevel3Immunities.RemoveComponents<ChangeOutgoingDamageType>();

                var dragonLevel2Immunities = BlueprintTool.Get<BlueprintFeature>("d1f01519ffbe9144dac703b579a0c073");
                dragonLevel2Immunities.AddComponent<AddFacts>(c =>
                {
                    c.m_Facts = new BlueprintUnitFactReference[]{
                        damageConvertActivatableAbility.ToReference<BlueprintUnitFactReference>()
                    };
                });
            }
        }
    }
}