using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheLastTestament.Common.NPCs;

/// <summary>Small shared helpers for the bespoke Cursed Grove NPC state machines.</summary>
public abstract class CursedGroveEnemyBase : ModNPC
{
    protected bool IsAuthority => Main.netMode != NetmodeID.MultiplayerClient;
    protected int State => (int)NPC.ai[0];
    protected int StateTimer => (int)NPC.ai[1];

    protected bool TryAcquireTarget(float range)
    {
        NPC.TargetClosest(faceTarget: false);
        if (!NPC.HasValidTarget)
            return false;

        Player target = Main.player[NPC.target];
        if (!target.active || target.dead || Vector2.DistanceSquared(NPC.Center, target.Center) > range * range)
        {
            if (IsAuthority)
                NPC.target = 255;
            return false;
        }

        return true;
    }

    protected void ChangeState(int nextState)
    {
        if (State == nextState)
            return;

        NPC.ai[0] = nextState;
        NPC.ai[1] = 0f;
        NPC.netUpdate = true;
    }

    protected void AdvanceStateTimer() => NPC.ai[1] += 1f;

    protected void FaceTarget()
    {
        if (NPC.HasValidTarget)
            NPC.direction = Main.player[NPC.target].Center.X >= NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
    }
}
