using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TheLastTestament.Common.NPCs;

namespace TheLastTestament.Content.NPCs.CursedGrove;

public sealed class CandleMothNPC : CursedGroveEnemyBase
{
    private const int Patrol = 0;
    private const int Approach = 1;
    private const int Windup = 2;
    private const int Cooldown = 3;
    private const int Death = 4;
    private const int WindupTicks = 42;
    private const int CooldownTicks = 150;
    private const int DeathTicks = 90;
    private const int IgnitionTick = 30;
    private const int AshTick = 66;

    public override string Texture => "TheLastTestament/Content/NPCs/CursedGrove/Sprites/CandleMoth";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = 40;
        NPC.height = 32;
        NPC.damage = 10;
        NPC.defense = 3;
        NPC.lifeMax = 48;
        NPC.knockBackResist = 0.4f;
        NPC.value = 0f;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.AshBlock, 1, 1, 2));
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        // Temporary first-playable placement until the Cursed Grove biome exists.
        return !Main.dayTime && spawnInfo.Player.ZoneForest && spawnInfo.Player.ZoneOverworldHeight ? 0.035f : 0f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        NPC.ai[3] = NPC.Center.Y;
        if (IsAuthority)
        {
            NPC.ai[2] = Main.rand.Next(90, 181);
            NPC.direction = Main.rand.NextBool() ? 1 : -1;
            NPC.netUpdate = true;
        }
        NPC.spriteDirection = NPC.direction;
    }

    public override void AI()
    {
        if (State == Death)
        {
            UpdateDeath();
            return;
        }

        bool hasTarget = TryAcquireTarget(520f);
        if (!hasTarget)
        {
            if (State != Patrol)
                ChangeState(Patrol);
            Wander();
            return;
        }

        FaceTarget();
        Player target = Main.player[NPC.target];
        switch (State)
        {
            case Patrol:
                ChangeState(Approach);
                MoveToCombatRange(target);
                break;
            case Approach:
                MoveToCombatRange(target);
                float distance = Vector2.Distance(NPC.Center, target.Center);
                if (distance >= 235f && distance <= 340f)
                {
                    NPC.velocity *= 0.82f;
                    NPC.ai[3] = target.Center.Y - 82f;
                    ChangeState(Windup);
                }
                break;
            case Windup:
                HoverForAttack(target);
                if (StateTimer == WindupTicks && IsAuthority)
                    FireFan(target);
                AdvanceStateTimer();
                if (StateTimer >= WindupTicks + 1)
                    ChangeState(Cooldown);
                break;
            case Cooldown:
                MoveToCombatRange(target);
                AdvanceStateTimer();
                if (StateTimer >= CooldownTicks)
                    ChangeState(Approach);
                break;
            default:
                ChangeState(Patrol);
                break;
        }

        NPC.rotation = MathHelper.Clamp(NPC.velocity.X * 0.025f, -0.16f, 0.16f);
        NPC.spriteDirection = NPC.direction;
        AddAmbientLight();
    }

    private void Wander()
    {
        if (NPC.ai[3] == 0f)
            NPC.ai[3] = NPC.Center.Y;

        NPC.ai[2]--;
        if (NPC.ai[2] <= 0f && IsAuthority)
        {
            NPC.direction = Main.rand.NextBool() ? 1 : -1;
            NPC.ai[2] = Main.rand.Next(90, 181);
            NPC.netUpdate = true;
        }

        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 1.15f, 0.035f);
        HoverAround(NPC.ai[3] + (float)Math.Sin(Main.GameUpdateCount * 0.045 + NPC.whoAmI) * 5f);
        NPC.spriteDirection = NPC.direction;
        AddAmbientLight();
    }

    private void MoveToCombatRange(Player target)
    {
        Vector2 delta = target.Center - NPC.Center;
        float distance = delta.Length();
        float horizontalVelocity = 0f;
        if (distance > 340f)
            horizontalVelocity = Math.Sign(delta.X) * 1.8f;
        else if (distance < 235f)
            horizontalVelocity = -Math.Sign(delta.X) * 1.8f;

        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, horizontalVelocity, 0.055f);
        NPC.ai[3] = MathHelper.Lerp(NPC.ai[3], target.Center.Y - 82f, 0.025f);
        HoverAround(NPC.ai[3] + (float)Math.Sin(Main.GameUpdateCount * 0.05 + NPC.whoAmI) * 4f);
    }

    private void HoverForAttack(Player target)
    {
        Vector2 midpoint = new(target.Center.X + NPC.direction * 280f, target.Center.Y - 76f);
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, MathHelper.Clamp((midpoint.X - NPC.Center.X) * 0.035f, -1.2f, 1.2f), 0.08f);
        NPC.ai[3] = MathHelper.Lerp(NPC.ai[3], target.Center.Y - 76f, 0.02f);
        HoverAround(NPC.ai[3] + (float)Math.Sin(Main.GameUpdateCount * 0.05 + NPC.whoAmI) * 2f);
    }

    private void HoverAround(float desiredY)
    {
        NPC.velocity.Y = MathHelper.Clamp((desiredY - NPC.Center.Y) * 0.07f, -1.4f, 1.4f);
    }

    private void FireFan(Player target)
    {
        Vector2 aim = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
        float baseAngle = aim.ToRotation();
        float[] angles = { -0.34f, 0f, 0.34f };
        foreach (float angle in angles)
        {
            Vector2 velocity = (baseAngle + angle).ToRotationVector2() * 8f;
            Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                velocity,
                ModContent.ProjectileType<CandleMothFlameProjectile>(),
                12,
                1f,
                Main.myPlayer);
        }
        SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
    }

    private void AddAmbientLight()
    {
        float intensity = State == Windup ? 0.85f : 0.34f;
        Lighting.AddLight(NPC.Center, intensity, intensity * 0.62f, intensity * 0.24f);
        if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(18))
        {
            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, -0.3f, 100, default, 0.55f);
            Main.dust[dustIndex].noGravity = true;
            Main.dust[dustIndex].velocity *= 0.25f;
        }
    }

    private void UpdateDeath()
    {
        AdvanceStateTimer();
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.noTileCollide = false;
        bool landed = NPC.collideY || StateTimer >= 48;
        NPC.noGravity = landed;
        if (landed)
            NPC.velocity.Y = 0f;
        NPC.rotation = MathHelper.Clamp(NPC.velocity.X * 0.035f, -0.55f, 0.55f);
        NPC.alpha = Math.Clamp((StateTimer - IgnitionTick) * 4, 0, 230);

        if (Main.netMode != NetmodeID.Server && StateTimer >= IgnitionTick)
        {
            bool burning = StateTimer < AshTick;
            Lighting.AddLight(NPC.Center, burning ? 0.65f : 0.18f, burning ? 0.25f : 0.12f, 0.06f);
            if (StateTimer % 4 == 0)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, burning ? DustID.Torch : DustID.Ash,
                    Main.windSpeedCurrent * 1.2f, burning ? -0.6f : -0.2f, 90, default, 0.75f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity.X += Main.windSpeedCurrent;
            }
        }

        if (StateTimer >= DeathTicks && IsAuthority)
        {
            NPC.life = 0;
            NPC.checkDead();
        }
    }

    public override bool CheckDead()
    {
        if (State == Death)
            return StateTimer >= DeathTicks;

        NPC.life = 1;
        NPC.dontTakeDamage = true;
        NPC.damage = 0;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.velocity.X *= 0.35f;
        NPC.velocity.Y = Math.Min(NPC.velocity.Y, -0.5f);
        ChangeState(Death);
        NPC.netUpdate = true;
        SoundEngine.PlaySound(SoundID.Item27, NPC.Center);
        return false;
    }
}

