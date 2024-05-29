﻿using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalRemix.Projectiles.Hostile
{
    public class HydrogenShell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Missile");
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2 - 0.2f;
            Player p = Main.player[(int)Projectile.ai[0]];
            if (p == null || !p.active || p.dead)
                return;
            Projectile.ai[1]++;
            int timeBeforeHome = 30;
            if (Projectile.ai[1] > timeBeforeHome)
            {
                if (Projectile.ai[1] > 120)
                Projectile.tileCollide = true;
                float scaleFactor = Projectile.velocity.Length();
                float inertia = 10f;
                Vector2 speed = Projectile.DirectionTo(p.Center).SafeNormalize(Vector2.UnitY) * scaleFactor;
                Projectile.velocity = (Projectile.velocity * inertia + speed) / (inertia + 1f);
                Projectile.velocity.Normalize();
                Projectile.velocity *= scaleFactor;
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            Projectile.ExpandHitboxBy(128);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.Damage();

            for (int i = 0; i < 12; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 15; j++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }
            HydrogenExplosion(Projectile);
        }
        public static void HydrogenExplosion(Projectile proj)
        {
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    Tile t = CalamityUtils.ParanoidTileRetrieval((int)(proj.Center.X / 16) + i, (int)(proj.Center.Y / 16) + j);

                    if (t.HasTile)
                    {
                        if (CalRemixWorld.SunkenSeaTiles.Contains(t.TileType))
                        {
                            WorldGen.KillTile((int)(proj.Center.X / 16) + i, (int)(proj.Center.Y / 16) + j, noItem: true);
                            if (!t.HasTile && Main.netMode != 0)
                                NetMessage.SendData(17, -1, -1, null, 0, i, j);
                        }
                    }
                    if (t != null && (t.WallType == ModContent.WallType<NavystoneWall>() || t.WallType == ModContent.WallType<EutrophicSandWall>()))
                    {
                        WorldGen.KillWall((int)(proj.Center.X / 16) + i, (int)(proj.Center.Y / 16) + j);
                        if (t.WallType == 0 && Main.netMode != 0)
                            NetMessage.SendData(17, -1, -1, null, 2, i, j);
                    }
                }
            }
        }
    }
}