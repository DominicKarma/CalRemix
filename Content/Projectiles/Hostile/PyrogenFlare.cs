﻿using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

//shamelessly 'borrowed' from carcinogen since they behave almost the same... different visuals and statuses though!

namespace CalRemix.Content.Projectiles.Hostile
{
    public class PyrogenFlare : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pyro Flare");
            Main.projFrames[Projectile.type] = 5;
        }

        public override string Texture => "CalamityMod/Projectiles/Boss/FlareBomb";

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 320;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 1f, 1.6f, 0f);

            if (Projectile.ai[1] == 1 && Projectile.timeLeft > 180)
                Projectile.timeLeft = 180;

            float speed = Projectile.ai[1] == 1 ? 13 : 9;
            if (Projectile.ai[1] == 0 || (Projectile.ai[1] == 1 && Projectile.ai[2]++ > 30))
            {
                Player target = Main.player[(int)Projectile.ai[0]];
                Vector2 distance = target.Center - Projectile.Center;
                distance *= 6;
                Projectile.velocity = (Projectile.velocity * 24f + distance) / 25f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= speed;
            }
            if (!Main.dedServ)
            {
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f);
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 120);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f);
                d.velocity = new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5));
            }

        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            return Projectile.position.Y > Main.player[(int)Projectile.ai[0]].Bottom.Y;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPyrogenFlare(Projectile, lightColor);
            return false;
        }

        public static void DrawPyrogenFlare(Projectile p, Color lightColor)
        {
            Texture2D piss = TextureAssets.Projectile[p.type].Value;
            Main.EntitySpriteDraw(TextureAssets.Projectile[p.type].Value, p.Center - Main.screenPosition, TextureAssets.Projectile[p.type].Frame(1, 5, 0, p.frame), lightColor, p.rotation, new Vector2(piss.Width / 2, piss.Height / 10), p.scale, 0, 0);
        }
    }
}