using HarmonyLib;
using UnityEngine;
using Verse;

namespace yayoCombat;

[HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming")]
public static class patch_DrawEquipmentAiming
{
    [HarmonyPrefix]
    private static bool Prefix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle, Pawn ___pawn)
    {
        if (!yayoCombat.advAni)
        {
            return true;
        }

        var num = aimAngle - 90f;
        var notRanged = false;
        var mirrored = false;

        if (!(___pawn.CurJob != null && ___pawn.CurJob.def.neverShowWeapon) && ___pawn.stances.curStance is Stance_Busy
            {
                neverAimWeapon: false, focusTarg.IsValid: true
            } stance_Busy)
        {
            if (___pawn.Rotation == Rot4.West)
            {
                mirrored = true;
            }

            if (!___pawn.equipment.Primary.def.IsRangedWeapon || stance_Busy.verb.IsMeleeAttack)
            {
                notRanged = true;
            }
        }

        Mesh mesh;
        if (notRanged)
        {
            if (mirrored)
            {
                mesh = MeshPool.plane10Flip;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
        }
        else if (aimAngle is > 20f and < 160f)
        {
            mesh = MeshPool.plane10;
            num += eq.def.equippedAngleOffset;
        }
        else if (aimAngle is > 200f and < 340f || mirrored)
        {
            mesh = MeshPool.plane10Flip;
            num -= 180f;
            num -= eq.def.equippedAngleOffset;
        }
        else
        {
            mesh = MeshPool.plane10;
            num += eq.def.equippedAngleOffset;
        }

        if ((___pawn.Rotation == Rot4.West || ___pawn.Rotation == Rot4.East) && yayoCombat.using_dualWeld &&
            !notRanged &&
            ___pawn.equipment.TryGetOffHandEquipment(out var result) && eq == result)
        {
            Stance_Busy stance_Busy2 = null;
            if (___pawn.GetStancesOffHand() != null)
            {
                stance_Busy2 = ___pawn.GetStancesOffHand().curStance as Stance_Busy;
            }

            if (stance_Busy2 == null || stance_Busy2.neverAimWeapon || !stance_Busy2.focusTarg.IsValid)
            {
                mesh = !(mesh == MeshPool.plane10Flip) ? MeshPool.plane10Flip : MeshPool.plane10;
            }
        }

        num %= 360f;
        Graphics.DrawMesh(
            material: !(eq.Graphic is Graphic_StackCount graphic_StackCount)
                ? eq.Graphic.MatSingle
                : graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle,
            mesh: PawnRenderer_override.GetMesh(mesh, eq, aimAngle, ___pawn),
            position: PawnRenderer_override.GetDrawOffset(drawLoc, eq, ___pawn),
            rotation: Quaternion.AngleAxis(num, Vector3.up), layer: 0);
        return false;
    }
}