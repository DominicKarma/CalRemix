using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;

namespace CalRemix.Content.Projectiles.Hostile
{
    public class LifeThorn : ModProjectile
    {
        public static int TotalSegments = 10;
        public override string Texture => "CalamityMod/Projectiles/Melee/ThornBase";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI / 2f;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.alpha -= 33;
                if (Projectile.alpha <= 0)
                {
                    if (Projectile.ai[0] == 0f)
                    {
                        Projectile.ai[0] += 1f;
                        Projectile.position += Projectile.velocity;
                    }
                    if (Projectile.ai[0] < TotalSegments)
                    {
                        int number = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 1f);
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, number);
                    }
                    Projectile.alpha = 0;
                    Projectile.ai[1] = 1f;
                }
                return;
            }
            int num = 2;
            Projectile.alpha += num;
            if (Projectile.alpha == num * 22)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, Projectile.velocity.X * 0.025f, Projectile.velocity.Y * 0.025f, 170, default, 1.3f);
                    dust.noGravity = true;
                    dust.velocity *= 0.5f;
                }
            }
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value = ModContent.Request<Texture2D>(Texture).Value;
            if (Projectile.ai[0] == TotalSegments)
                value = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/ThornTip").Value;
            Main.spriteBatch.Draw(value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, value.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}