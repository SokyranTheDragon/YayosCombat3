using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(Pawn), "Tick")]
internal class patch_Pawn_TickRare
{
    [HarmonyPostfix]
    [HarmonyPriority(0)]
    private static void Postfix(Pawn __instance)
    {
        if (!yayoCombat.ammo || !__instance.Drafted || Find.TickManager.TicksGame % 60 != 0 ||
            __instance.CurJobDef != JobDefOf.Wait_Combat && __instance.CurJobDef != JobDefOf.AttackStatic ||
            __instance.equipment == null)
        {
            return;
        }

        var allEquipmentListForReading = __instance.equipment.AllEquipmentListForReading;
        foreach (var item in allEquipmentListForReading)
        {
            var compReloadable = item.TryGetComp<CompReloadable>();
            if (compReloadable == null)
            {
                continue;
            }

            reloadUtility.tryAutoReload(compReloadable);
            break;
        }
    }
}