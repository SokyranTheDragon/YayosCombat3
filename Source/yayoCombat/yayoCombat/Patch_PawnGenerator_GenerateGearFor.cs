using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(PawnGenerator), "GenerateGearFor")]
internal class Patch_PawnGenerator_GenerateGearFor
{
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(Pawn pawn)
    {
        if (!yayoCombat.ammo)
        {
            return;
        }

        var allWeaponsComps = new List<CompReloadable>();
        // get all generated equipped weapons
        if (pawn?.equipment?.AllEquipmentListForReading != null)
        {
            foreach (var thing in pawn.equipment.AllEquipmentListForReading)
            {
                var comp = thing.GetComp<CompReloadable>();
                if (comp != null && thing.def.IsWeapon)
                {
                    allWeaponsComps.Add(comp);
                }
            }
        }

        // get all generated weapons in inventory
        if (pawn?.inventory?.innerContainer != null)
        {
            foreach (var thing in pawn.inventory.innerContainer)
            {
                var comp = thing.TryGetComp<CompReloadable>();
                if (comp != null && thing.def.IsWeapon)
                {
                    allWeaponsComps.Add(comp);
                }
            }
        }

        // add ammo to all weapons found
        foreach (var comp in allWeaponsComps)
        {
            int charges;
            if (yayoCombat.s_enemyAmmo <= 1f || pawn?.Faction != null && pawn.Faction.IsPlayer)
            {
                charges = Mathf.Min(comp.MaxCharges,
                    Mathf.RoundToInt(comp.MaxCharges * yayoCombat.s_enemyAmmo * Rand.Range(0.7f, 1.3f)));
            }
            else
            {
                charges = Mathf.RoundToInt(comp.MaxCharges * yayoCombat.s_enemyAmmo * Rand.Range(0.7f, 1.3f));
            }

            comp.remainingCharges = charges;
        }
    }
}