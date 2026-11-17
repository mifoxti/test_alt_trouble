using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using TheLastTestament.Common.NPCs;

namespace TheLastTestament.Content.NPCs.CursedGrove;

public sealed class WitheredRootNPC : CursedGroveEnemyBase
{
    private const int Idle = 0;
    private const int Chase = 1;
    private const int DigDown = 2;
    private const int Burrow = 3;
    private const int EmergenceWarning = 4;
    private const int Emerge = 5;
    private const int BranchStrikeOne = 6;
    private const int StrikePause = 7;
    private const int BranchStrikeTwo = 8;
    private const int Vulnerable = 9;
    private const int Death = 10;

    private const int DeathDuration = 48;
    private bool deathStarted;

    public override string Texture => "TheLastTestament/Content/NPCs/CursedGrove/Sprites/WitheredRoot";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = 48;
        NPC.height = 38;
        NPC.damage = 18;
        NPC.defense = 8;
        NPC.lifeMax = 80;
        NPC.knockBackResist = 0.55f;
        NPC.aiStyle = -1;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath4;
        NPC.value = 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Wood, 1, 2, 4));
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        // Temporary first-playable placement until a dedicated Cursed Grove biome exists.
        return !Main.dayTime && spawnInfo.Player.ZoneForest
            ? SpawnCondition.OverworldNightMonster.Chance * 0.06f
            : 0f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (IsAuthority)
        {
            NPC.ai[2] = Main.rand.NextBool() ? 1f : -1f;
            NPC.netUpdate = true;
        }
    }

    public override void AI()
    {
        if (State == Death)
        {
            UpdateDeath();
            return;
        }

        if (!TryAcquireTarget(420f))
        {
            if (State is not (Idle or Vulnerable))
                ChangeState(Idle);
            NPC.damage = 18;
            NPC.dontTakeDamage = false;
            UpdateIdle();
            return;
        }

        FaceTarget();
        Player target = Main.player[NPC.target];

        switch (State)
        {
            case Idle:
                ChangeState(Chase);
                break;
            case Chase:
                UpdateChase(target);
                break;
            case DigDown:
                UpdateDigDown();
                break;
            case Burrow:
                UpdateBurrow(target);
                break;
            case EmergenceWarning:
                UpdateEmergenceWarning();
                break;
            case Emerge:
                UpdateEmerge();
                break;
            case BranchStrikeOne:
                UpdateStrike(BranchStrikeOne);
                break;
            case StrikePause:
                UpdateStrikePause();
                break;
            case BranchStrikeTwo:
                UpdateStrike(BranchStrikeTwo);
                break;
            case Vulnerable:
                UpdateVulnerable(target);
                break;
            default:
                ChangeState(Idle);
                break;
        }
    }

    private void UpdateIdle()
    {
        NPC.dontTakeDamage = false;
        NPC.alpha = 0;
        NPC.color = Color.White;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.damage = 18;
        int phase = StateTimer % 240;
        if (phase < 132)
        {
            NPC.velocity.X = NPC.ai[2] * 0.7f;
            if (NPC.collideX && IsAuthority)
                NPC.ai[2] *= -1f;
        }
        else if (phase < 180)
        {
            NPC.velocity.X *= 0.75f;
        }
        else
        {
            if (phase == 180)
                NPC.ai[2] *= -1f;
            NPC.velocity.X = NPC.ai[2] * 0.45f;
        }

        AdvanceStateTimer();
    }

    private void UpdateChase(Player target)
    {
        NPC.damage = 18;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.dontTakeDamage = false;
        NPC.velocity.X = NPC.direction * 1.8f;

        if (NPC.collideX)
            NPC.velocity.X = 0f;

        float distance = Vector2.Distance(NPC.Center, target.Center);
        if (distance <= 360f && Math.Abs(NPC.Center.Y - target.Center.Y) <= 100f)
        {
            NPC.ai[2] = target.Center.X >= NPC.Center.X ? 1f : -1f;
            ChangeState(DigDown);
            NPC.damage = 0;
            NPC.velocity.X = 0f;
        }
    }

    private void UpdateDigDown()
    {
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.velocity = new Vector2(0f, 1.1f);
        NPC.alpha = Math.Min(200, NPC.alpha + 12);
        SpawnGroundDust(0.25f);

        if (StateTimer >= 18)
            ChangeState(Burrow);
        else
            AdvanceStateTimer();
    }

    private void UpdateBurrow(Player target)
    {
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.alpha = 200;

        Vector2 destination = target.Center + new Vector2(NPC.ai[2] * -120f, 24f);
        Vector2 delta = destination - NPC.Center;
        NPC.velocity = delta.LengthSquared() > 1f ? delta.SafeNormalize(Vector2.UnitX) * 5f : Vector2.Zero;
        SpawnGroundDust(0.12f);

        if (delta.Length() < 24f || StateTimer >= 52)
        {
            NPC.velocity = Vector2.Zero;
            PlaceAtSurfaceNear(target);
            ChangeState(EmergenceWarning);
        }
        else
        {
            AdvanceStateTimer();
        }
    }

    private void UpdateEmergenceWarning()
    {
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.velocity = Vector2.Zero;
        NPC.alpha = 200;
        SpawnGroundDust(0.6f);

        if (StateTimer >= 30)
        {
            NPC.alpha = 0;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.velocity.Y = -5f;
            ChangeState(Emerge);
        }
        else
        {
            AdvanceStateTimer();
        }
    }

    private void UpdateEmerge()
    {
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.velocity.X = 0f;
        if (StateTimer >= 12)
        {
            NPC.velocity.Y = 0f;
            NPC.dontTakeDamage = false;
            ChangeState(BranchStrikeOne);
        }
        else
        {
            AdvanceStateTimer();
        }
    }

    private void UpdateStrike(int strikeState)
    {
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.velocity.X *= 0.7f;
        NPC.velocity.Y = 0f;

        if (StateTimer == 0 && IsAuthority)
            FireBranchSweep();

        int duration = strikeState == BranchStrikeOne ? 18 : 24;
        if (StateTimer >= duration)
        {
            if (strikeState == BranchStrikeOne)
                ChangeState(StrikePause);
            else
                ChangeState(Vulnerable);
        }
        else
        {
            AdvanceStateTimer();
        }
    }

    private void UpdateStrikePause()
    {
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.velocity.X = 0f;
        if (StateTimer >= 16)
            ChangeState(BranchStrikeTwo);
        else
            AdvanceStateTimer();
    }

    private void UpdateVulnerable(Player target)
    {
        NPC.damage = 0;
        NPC.dontTakeDamage = false;
        NPC.velocity.X = 0f;
        if (StateTimer >= 48)
        {
            if (Vector2.DistanceSquared(NPC.Center, target.Center) <= 420f * 420f)
                ChangeState(Chase);
            else
                ChangeState(Idle);
        }
        else
        {
            AdvanceStateTimer();
        }
    }

    private void PlaceAtSurfaceNear(Player target)
    {
        float side = NPC.ai[2];
        int tileX = Math.Clamp((int)((target.Center.X + side * 120f) / 16f), 10, Main.maxTilesX - 10);
        int targetTileY = Math.Clamp((int)(target.Bottom.Y / 16f), 10, Main.maxTilesY - 10);
        for (int y = Math.Max(10, targetTileY - 10); y < Math.Min(Main.maxTilesY - 10, targetTileY + 14); y++)
        {
            if (WorldGen.SolidTile(tileX, y) && !WorldGen.SolidTile(tileX, y - 1))
            {
                NPC.Center = new Vector2(tileX * 16f + 8f, y * 16f - NPC.height * 0.5f + 2f);
                NPC.direction = target.Center.X >= NPC.Center.X ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
                return;
            }
        }

        NPC.Center = new Vector2(target.Center.X + side * 120f, target.Bottom.Y - NPC.height * 0.5f);
    }

    private void FireBranchSweep()
    {
        Vector2 position = NPC.Center + new Vector2(NPC.direction * 22f, -4f);
        Vector2 velocity = new Vector2(NPC.direction * 5.5f, 0f);
        Projectile.NewProjectile(
            NPC.GetSource_FromAI(),
            position,
            velocity,
            ModContent.ProjectileType<RootBranchSweepProjectile>(),
            18,
            2.5f,
            Main.myPlayer);
    }

    private void SpawnGroundDust(float chance)
    {
        if (Main.rand.NextFloat() >= chance)
            return;

        int dust = Dust.NewDust(NPC.Bottom - new Vector2(10f, 6f), NPC.width + 20, 8, DustID.Dirt, Main.rand.NextFloat(-1.2f, 1.2f), -1.5f, 100, new Color(79, 57, 49), 1.1f);
        Main.dust[dust].noGravity = false;
    }

    private void UpdateDeath()
    {
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.noTileCollide = false;
        NPC.noGravity = false;
        NPC.velocity.X *= 0.6f;
        NPC.alpha = Math.Min(200, NPC.alpha + 3);
        NPC.color = Color.Lerp(Color.White, new Color(93, 67, 58), MathHelper.Clamp(StateTimer / (float)DeathDuration, 0f, 1f));

        if (Main.rand.NextBool(4))
        {
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Dirt, Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-2f, 0f), 100, new Color(93, 67, 58), 1.3f);
            Main.dust[dust].noGravity = true;
        }

        if (StateTimer >= DeathDuration)
        {
            if (IsAuthority)
                NPC.StrikeInstantKill();
            return;
        }

        AdvanceStateTimer();
    }

    public override bool CheckDead()
    {
        if (deathStarted)
            return true;

        deathStarted = true;
        NPC.life = 1;
        NPC.damage = 0;
        NPC.dontTakeDamage = true;
        NPC.velocity = Vector2.Zero;
        NPC.alpha = 0;
        NPC.netUpdate = true;
        ChangeState(Death);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (Main.dedServ)
            return;

        if (State is BranchStrikeOne or BranchStrikeTwo)
        {
            float duration = State == BranchStrikeOne ? 18f : 24f;
            float progress = MathHelper.Clamp(StateTimer / duration, 0f, 1f);
            float startAngle = State == BranchStrikeOne ? -0.8f : 0.8f;
            float angle = (startAngle + (State == BranchStrikeOne ? 1.6f : -1.6f) * progress) * NPC.direction;
            Vector2 start = NPC.Center - screenPos + new Vector2(NPC.direction * 10f, -7f);
            Vector2 elbow = start + new Vector2((float)System.Math.Cos(angle) * 27f, (float)System.Math.Sin(angle) * 27f);
            Vector2 tip = elbow + new Vector2((float)System.Math.Cos(angle + NPC.direction * 0.35f) * 25f,
                (float)System.Math.Sin(angle + NPC.direction * 0.35f) * 25f);
            DrawPixelLine(spriteBatch, start, elbow, new Color(31, 23, 26), 5f);
            DrawPixelLine(spriteBatch, elbow, tip, new Color(31, 23, 26), 4f);
            DrawPixelLine(spriteBatch, start, elbow, new Color(112, 74, 59), 3f);
            DrawPixelLine(spriteBatch, elbow, tip, new Color(146, 93, 67), 2f);
        }

        if (State == EmergenceWarning)
        {
            Vector2 ground = NPC.Bottom - screenPos + new Vector2(0f, -2f);
            Color crack = new(83, 58, 51, 230);
            DrawPixelLine(spriteBatch, ground + new Vector2(-37f, 0f), ground + new Vector2(-12f, -1f), crack, 1.5f);
            DrawPixelLine(spriteBatch, ground + new Vector2(-12f, -1f), ground + new Vector2(-5f, 5f), crack, 1.5f);
            DrawPixelLine(spriteBatch, ground + new Vector2(37f, 0f), ground + new Vector2(11f, -1f), crack, 1.5f);
            DrawPixelLine(spriteBatch, ground + new Vector2(11f, -1f), ground + new Vector2(5f, 5f), crack, 1.5f);
        }

        if (State == Death && StateTimer >= 10)
        {
            Vector2 center = NPC.Center - screenPos;
            Color crack = new(38, 29, 28, Math.Min(220, StateTimer * 3));
            DrawPixelLine(spriteBatch, center + new Vector2(-11f, -12f), center + new Vector2(-3f, -4f), crack, 1.5f);
            DrawPixelLine(spriteBatch, center + new Vector2(-3f, -4f), center + new Vector2(-10f, 5f), crack, 1.5f);
            DrawPixelLine(spriteBatch, center + new Vector2(7f, -8f), center + new Vector2(2f, 1f), crack, 1.5f);
            DrawPixelLine(spriteBatch, center + new Vector2(2f, 1f), center + new Vector2(12f, 10f), crack, 1.5f);
        }
    }

    private static void DrawPixelLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Vector2 delta = end - start;
        spriteBatch.Draw(pixel, start, null, color, delta.ToRotation(), Vector2.Zero,
            new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        for (int i = 0; i < 3; i++)
        {
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Dirt, hit.HitDirection * 1.2f, -1.2f, 120, new Color(93, 67, 58), 1f);
            Main.dust[dust].noGravity = true;
        }
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Y = 0;
    }
}

public sealed class RootBranchSweepProjectile : ModProjectile
{
    public override string Texture => "Terraria/Images/Projectile_0";

    public override void SetDefaults()
    {
        Projectile.width = 58;
        Projectile.height = 34;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 8;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hide = true;
    }

    public override void AI()
    {
        if (Main.rand.NextBool(2))
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, Projectile.velocity.X * 0.2f, -0.5f, 120, new Color(102, 69, 59), 0.8f);
            Main.dust[dust].noGravity = true;
        }
    }
}


