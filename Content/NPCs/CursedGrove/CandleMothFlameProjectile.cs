using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TheLastTestament.Content.NPCs.CursedGrove;

public sealed class CandleMothFlameProjectile : ModProjectile
{
    public override string Texture => "Terraria/Images/Projectile_85";

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 75;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Lighting.AddLight(Projectile.Center, 0.75f, 0.32f, 0.08f);
        if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(3))
        {
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 120, default, 0.8f);
            Main.dust[dustIndex].noGravity = true;
            Main.dust[dustIndex].velocity *= 0.15f;
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.OnFire, 120);
    }
}
