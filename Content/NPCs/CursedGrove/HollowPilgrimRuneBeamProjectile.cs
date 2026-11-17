using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheLastTestament.Content.NPCs.CursedGrove;

public sealed class HollowPilgrimRuneBeamProjectile : ModProjectile
{
    private const float MaxLength = 480f;
    private const float BeamWidth = 18f;

    public override string Texture => "Terraria/Images/Projectile_0";

    public override void SetDefaults()
    {
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 12;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hide = true;
        Projectile.netImportant = true;
    }

    public override void AI()
    {
        Projectile.velocity = Vector2.Zero;
        if (Projectile.localAI[0] > 0f)
            return;

        Vector2 direction = Direction;
        float clearLength = MaxLength;
        for (float distance = 16f; distance <= MaxLength; distance += 16f)
        {
            Vector2 point = Projectile.Center + direction * distance;
            if (!Collision.CanHitLine(Projectile.Center, 1, 1, point, 1, 1))
            {
                clearLength = distance - 16f;
                break;
            }
        }

        Projectile.localAI[0] = MathHelper.Max(16f, clearLength);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0f;
        Vector2 end = Projectile.Center + Direction * EffectiveLength;
        return Collision.CheckAABBvLineCollision(
            targetHitbox.TopLeft(),
            targetHitbox.Size(),
            Projectile.Center,
            end,
            BeamWidth,
            ref collisionPoint);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Main.dedServ)
            return false;

        float fade = MathHelper.Clamp(Projectile.timeLeft / 8f, 0f, 1f);
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Vector2 start = Projectile.Center - Main.screenPosition;
        Vector2 end = start + Direction * EffectiveLength;
        DrawBeam(pixel, start, end, new Color(157, 109, 255, 0) * (fade * 0.28f), 7f);
        DrawBeam(pixel, start, end, new Color(176, 139, 255, 0) * (fade * 0.60f), 3f);
        DrawBeam(pixel, start, end, new Color(236, 225, 255, 0) * fade, 1f);
        return false;
    }

    private Vector2 Direction => new((float)System.Math.Cos(Projectile.ai[0]), (float)System.Math.Sin(Projectile.ai[0]));
    private float EffectiveLength => Projectile.localAI[0] > 0f ? Projectile.localAI[0] : Projectile.ai[1];

    private void DrawBeam(Texture2D pixel, Vector2 start, Vector2 end, Color color, float thickness)
    {
        Vector2 delta = end - start;
        Main.spriteBatch.Draw(pixel, start, null, color, delta.ToRotation(), Vector2.Zero,
            new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);
    }
}
