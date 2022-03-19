using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using Kingmaker.Utility.UnitDescription;
using Owlcat.Runtime.Core.Utils;
using System;
using System.Collections.Generic;

namespace WOTR_PATH_OF_BLING.MechanicsChanges
{
    [TypeId("a584568f53d2454a97a48123d6f7d78f")]
    public class GoldenDragonSpellDamage : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateDamage>, IRulebookHandler<RuleCalculateDamage>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public void OnEventAboutToTrigger(RuleCalculateDamage evt)
        {
            MechanicsContext context = evt.Reason.Context;
            if((context != null ? context.SourceAbility : null) == null)
            {
                return;
            }
            if(!context.SourceAbility.IsSpell)
            {
                return;
            }
            foreach (BaseDamage baseDamage in evt.DamageBundle)
            {
                DiceType newDice = new DiceType();
                if (base.Owner.Progression.MythicLevel == 8)
                {
                    newDice = DiceType.D6;
                }
                else if(base.Owner.Progression.MythicLevel == 9)
                {
                    newDice = DiceType.D8;
                }
                else if (base.Owner.Progression.MythicLevel == 10)
                {
                    newDice = DiceType.D10;
                }
                if (baseDamage.Dice.m_Dice < newDice)
                {
                    baseDamage.ReplaceDice(new DiceFormula(baseDamage.Dice.m_Rolls, newDice));
                }
                else
                {
                    int bonus = (1 * baseDamage.Dice.Rolls);
                    baseDamage.AddModifier(bonus, base.Fact);
                }
                if (CheckVulnerability(evt.Target))
                {
                    baseDamage.m_Vulnerability = 1.5f;
                }
            }
        }

        static bool CheckVulnerability(UnitEntityData targetUnit)
        {
            foreach (EntityFact entityFact in targetUnit.Facts.List)
            {
                AddEnergyVulnerability component = entityFact.Blueprint.GetComponent<AddEnergyVulnerability>();
                if (component != null)
                {
                    List<ContextActionDealDamage> actionDealDamageList = TempList.Get<ContextActionDealDamage>();
                    foreach (ContextActionDealDamage actionDealDamage6 in actionDealDamageList)
                    {
                        if (actionDealDamage6.DamageType.Type == DamageType.Energy && actionDealDamage6.DamageType.Energy == component.Type)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public void OnEventDidTrigger(RuleCalculateDamage evt)
        {

        }

    }
}