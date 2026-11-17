using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TheLastTestament.Common.NPCs;

namespace TheLastTestament.Content.NPCs.CursedGrove;

public sealed class HollowPilgrimNPC : CursedGroveEnemyBase
{
    private const int Roam = 0;
    private const int Pause = 1;
    private const int KeepDistance = 2;
    private const int RuneCharge = 3;
    private const int Beam = 4;
    private const int Recovery = 5;
    private const int Death = 6;

    private const int DetectRange = 620;
    private const int DesiredDistance = 320;
    private const int ChargeTicks = 90;
    private const int ChargeLockTicks = 78;
    private const int BeamTicks = 10;
    private const int RecoveryTicks = 150;
    private const int DeathTicks = 96;

    public override string Texture => "TheLastTestament/Content/NPCs/CursedGrove/Sprites/HollowPilgrim";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
        NPCID.Sets.DangerDetectRange[Type] = 480;
    }

    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 64;
        NPC.damage = 16;
        NPC.defense = 12;
        NPC.lifeMax = 110;
        NPC.knockBackResist = 0.35f;
        NPC.value = 0f;
        NPC.aiStyle = -1;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath6;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.StoneBlock, 1, 2, 4));
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        // Temporary first-playable spawn location until the Cursed Grove biome exists.
        return !Main.dayTime && spawnInfo.Player.ZoneForest && spawnInfo.Player.ZoneOverworldHeight ? 0.012f : 0f;
    }

    public override void AI()
    {
        if (State == Death)
        {
            RunDeathSequence();
            return;
        }

        NPC.damage = State is RuneCharge or Beam or Recovery ? 0 : 16;
        NPC.dontTakeDamage = false;

        switch (State)
        {
            case Roam:
                RunRoam();
                break;
            case Pause:
                RunPause();
                break;
            case KeepDistance:
                RunKeepDistance();
                break;
            case RuneCharge:
                RunRuneCharge();
                break;
            case Beam:
                RunBeam();
                break;
            case Recovery:
                RunRecovery();
                break;
            default:
                ChangeState(Roam);
                break;
        }

        if (State is not Roam and not Pause)
            FaceTarget();

        AdvanceStateTimer();
    }

    private void RunRoam()
    {
        if (TryAcquireTarget(DetectRange))
        {
            ChangeState(KeepDistance);
            return;
        }

        if (StateTimer >= 210)
        {
            NPC.velocity.X *= 0.25f;
            ChangeState(Pause);
            return;
        }

        if (StateTimer == 0 && IsAuthority)
        {
            NPC.direction = Main.rand.NextBool() ? 1 : -1;
            NPC.netUpdate = true;
        }

        NPC.spriteDirection = NPC.direction;
        float wantedSpeed = NPC.direction * 0.7f;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, wantedSpeed, 0.06f);
        if (NPC.collideX)
        {
            NPC.direction *= -1;
            NPC.netUpdate = true;
        }
    }

    private void RunPause()
    {
        if (TryAcquireTarget(DetectRange))
        {
            ChangeState(KeepDistance);
            return;
        }

        NPC.velocity.X *= 0.8f;
        if (StateTimer >= 48)
            ChangeState(Roam);
    }

    private void RunKeepDistance()
    {
        if (!TryAcquireTarget(DetectRange))
        {
            ChangeState(Roam);
            return;
        }

        FaceTarget();
        Player target = Main.player[NPC.target];
        float distance = Vector2.Distance(NPC.Center, target.Center);
        if (distance <= 500f && distance >= 210f && Collision.CanHitLine(NPC.Center, 1, 1, target.Center, 1, 1))
        {
            NPC.velocity.X *= 0.6f;
            ChangeState(RuneCharge);
            NPC.localAI[0] = (target.Center - RuneOrigin).ToRotation();
            NPC.netUpdate = true;
            return;
        }

        if (distance < 210f)
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, -NPC.direction * 1.1f, 0.08f);
        else if (distance > 365f)
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 0.8f, 0.05f);
        else
            NPC.velocity.X *= 0.75f;
    }

    private void RunRuneCharge()
    {
        NPC.velocity.X *= 0.82f;
        if (TryAcquireTarget(700f))
        {
            FaceTarget();
            if (StateTimer < ChargeLockTicks)
                NPC.localAI[0] = (Main.player[NPC.target].Center - RuneOrigin).ToRotation();
            else if (StateTimer == ChargeLockTicks)
            {
                NPC.ai[2] = NPC.localAI[0];
                NPC.netUpdate = true;
            }
        }

        if (StateTimer >= ChargeTicks)
        {
            if (IsAuthority)
            {
                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    RuneOrigin,
                    Vector2.Zero,
                    ModContent.ProjectileType<HollowPilgrimRuneBeamProjectile>(),
                    28,
                    0f,
                    Main.myPlayer,
                    NPC.ai[2],
                    480f);
            }

            ChangeState(Beam);
            NPC.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item122, NPC.Center);
        }
    }

    private void RunBeam()
    {
        NPC.velocity.X *= 0.7f;
        if (StateTimer >= BeamTicks)
            ChangeState(Recovery);
    }

    private void RunRecovery()
    {
        NPC.velocity.X *= 0.8f;
        if (StateTimer >= RecoveryTicks)
        {
            if (TryAcquireTarget(DetectRange))
                ChangeState(KeepDistance);
            else
                ChangeState(Roam);
        }
    }

    private void RunDeathSequence()
    {
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.noGravity = true;
        NPC.velocity.X *= 0.7f;
        NPC.velocity.Y = 0f;
        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0f, 0.12f);

        if (!Main.dedServ && StateTimer < 42 && StateTimer % 6 == 0)
        {
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone);
            Main.dust[dust].velocity *= 0.25f;
            Main.dust[dust].noGravity = true;
        }
        else if (!Main.dedServ && StateTimer >= 60 && StateTimer < DeathTicks && StateTimer % 3 == 0)
        {
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, Main.windSpeedCurrent * 2f,
                -0.4f - Main.rand.NextFloat(0.8f), 40, new Color(144, 143, 151), 1.05f);
            Main.dust[dust].velocity.X += Main.windSpeedCurrent * 2f;
            Main.dust[dust].noGravity = true;
        }

        if (StateTimer >= DeathTicks && IsAuthority)
        {
            NPC.life = 0;
            NPC.checkDead();
        }

        AdvanceStateTimer();
    }

    private Vector2 RuneOrigin => NPC.Center + new Vector2(NPC.direction * 18f, -8f);

    public override bool CheckDead()
    {
        if (State == Death)
            return true;

        NPC.ai[0] = Death;
        NPC.ai[1] = 0f;
        NPC.life = 1;
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.velocity = Vector2.Zero;
        NPC.netUpdate = true;
        if (!Main.dedServ)
            SoundEngine.PlaySound(SoundID.Shatter, NPC.Center);
        return false;
    }

    public override Color? GetAlpha(Color drawColor)
    {
        if (State != Death)
            return null;

        float stone = MathHelper.Clamp(StateTimer / 42f, 0f, 1f);
        return Color.Lerp(drawColor, new Color(151, 151, 158), stone);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (State == RuneCharge)
            DrawRuneCircle(spriteBatch, screenPos);

        return true;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (Main.dedServ || State != Death || StateTimer < 36)
            return;

        Vector2 center = NPC.Center - screenPos;
        float alpha = MathHelper.Clamp((StateTimer - 36f) / 24f, 0f, 1f);
        Color crack = new Color(38, 35, 43, 225) * alpha;
        DrawLine(spriteBatch, TextureAssets.MagicPixel.Value, center + new Vector2(-17f, -17f), center + new Vector2(-4f, -5f), crack, 1.5f);
        DrawLine(spriteBatch, TextureAssets.MagicPixel.Value, center + new Vector2(-4f, -5f), center + new Vector2(-12f, 7f), crack, 1.5f);
        DrawLine(spriteBatch, TextureAssets.MagicPixel.Value, center + new Vector2(11f, -14f), center + new Vector2(4f, -1f), crack, 1.5f);
        DrawLine(spriteBatch, TextureAssets.MagicPixel.Value, center + new Vector2(4f, -1f), center + new Vector2(16f, 12f), crack, 1.5f);
    }

    private void DrawRuneCircle(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        if (Main.dedServ)
            return;

        Vector2 center = RuneOrigin - screenPos;
        float radius = 23f + 2f * (float)System.Math.Sin(StateTimer * 0.13f);
        float rotation = StateTimer * 0.035f;
        float progress = MathHelper.Clamp(StateTimer / (float)ChargeTicks, 0f, 1f);
        Color color = Color.Lerp(new Color(97, 73, 157, 160), new Color(201, 179, 255, 245), progress);
        Texture2D pixel = TextureAssets.MagicPixel.Value;

        const int segments = 32;
        Vector2 previous = center + new Vector2(radius, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float angle = MathHelper.TwoPi * i / segments;
            Vector2 current = center + new Vector2((float)System.Math.Cos(angle) * radius, (float)System.Math.Sin(angle) * radius);
            DrawLine(spriteBatch, pixel, previous, current, color, 1.2f);
            previous = current;
        }

        int activeGlyphs = Math.Min(8, StateTimer * 8 / ChargeTicks);
        for (int i = 0; i < 8; i++)
        {
            float angle = rotation + MathHelper.TwoPi * i / 8f;
            Vector2 radial = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle));
            Vector2 mark = center + radial * (radius + 5f);
            DrawLine(spriteBatch, pixel, mark - radial * 3f, mark + radial * 3f, color, 1.5f);

            Vector2 glyphCenter = center + radial * (radius * 0.72f);
            Vector2 tangent = new Vector2(-radial.Y, radial.X);
            Color glyphColor = i < activeGlyphs ? color : new Color(72, 63, 93, 120);
            DrawLine(spriteBatch, pixel, glyphCenter - tangent * 3f, glyphCenter + tangent * 3f, glyphColor, 1f);
            DrawLine(spriteBatch, pixel, glyphCenter - radial * 3f, glyphCenter + radial * 3f, glyphColor, 1f);
        }

        Vector2 aim = new Vector2((float)System.Math.Cos(NPC.ai[2]), (float)System.Math.Sin(NPC.ai[2]));
        if (StateTimer < ChargeLockTicks && NPC.HasValidTarget)
            aim = new Vector2((float)System.Math.Cos(NPC.localAI[0]), (float)System.Math.Sin(NPC.localAI[0]));
        DrawLine(spriteBatch, pixel, center, center + aim * (radius + 9f), new Color(223, 203, 255, 230), 1f);
    }

    private static void DrawLine(SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color, float thickness)
    {
        Vector2 delta = end - start;
        spriteBatch.Draw(pixel, start, null, color, delta.ToRotation(), Vector2.Zero, new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);
    }
}

