﻿using CalamityMod.Dusts;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using CalamityMod;
using CalamityMod.BiomeManagers;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using System;
using System.Collections.Generic;
using CalamityMod.Events;
using CalamityMod.UI;
using System.Linq;
using Terraria.ModLoader.Core;
using CalRemix.NPCs.TownNPCs;
using Terraria.ModLoader.IO;
using System.IO;
using CalRemix.Projectiles.Hostile;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Terraria.WorldBuilding;

namespace CalRemix.NPCs.BioWar
{
    public class BioWar : ModSystem
    {
        /// <summary>
        /// Whether or not the event is active
        /// </summary>
        public static bool IsActive;

        /// <summary>
        /// Enemies considered defenders
        /// </summary>
        public static List<int> DefenderNPCs = new List<int>() { ModContent.NPCType<Eosinine>() };

        /// <summary>
        /// Enemies considered invaders
        /// </summary>
        public static List<int> InvaderNPCs = new List<int>() { ModContent.NPCType<Malignant>() };

        /// <summary>
        /// Projectiles considered defenders
        /// </summary>
        public static List<int> DefenderProjectiles = new List<int>() { ModContent.ProjectileType<EosinineProj>() };

        /// <summary>
        /// Projectiles considered invaders
        /// </summary>
        public static List<int> InvaderProjectiles = new List<int>() { };

        /// <summary>
        /// Defender NPC kill count
        /// </summary>
        public static float DefendersKilled = 0;

        /// <summary>
        /// Invader NPC kill count
        /// </summary>
        public static float InvadersKilled = 0;

        /// <summary>
        /// Total kills, duh
        /// </summary>
        public static float TotalKills => DefendersKilled + InvadersKilled;

        /// <summary>
        /// Defender kills required in order to end the event as an invader
        /// </summary>
        public static float MaxRequired => MinToSummonPathogen + 200;

        /// <summary>
        /// How much higher a faction's kill count has to be to side with them
        /// </summary>
        public const float KillBuffer = 30;

        /// <summary>
        /// The amount of kills required to summon Pathogen
        /// </summary>
        public const float MinToSummonPathogen = 300;

        /// <summary>
        /// If the player is on the defenders' side
        /// </summary>
        public static bool DefendersWinning => InvadersKilled > DefendersKilled + KillBuffer;

        /// <summary>
        /// If the player is on the invaders' side
        /// </summary>
        public static bool InvadersWinning => DefendersKilled > InvadersKilled + KillBuffer;

        /// <summary>
        /// If Pathogen has been summoned
        /// </summary>
        public static bool SummonedPathogen = false;

        public override void PreUpdateWorld()
        {
            IsActive = true;
            if (IsActive)
            {
                if (TotalKills > 300 && !SummonedPathogen)
                {
                    NPC.SpawnOnPlayer(Main.LocalPlayer.whoAmI, NPCID.Frankenstein);
                    SummonedPathogen = true;
                }
            }
            if (DefendersKilled > MaxRequired)
            {
                EndEvent();
            }
        }

        public static void EndEvent()
        {
            DefendersKilled = 0;
            InvadersKilled = 0;
            SummonedPathogen = false;
            IsActive = false;
            CalRemixWorld.UpdateWorldBool();
        }

        public override void OnWorldLoad()
        {
            DefendersKilled = 0;
            InvadersKilled = 0;
            SummonedPathogen = false;
            IsActive = false;
        }

        public override void OnWorldUnload()
        {
            DefendersKilled = 0;
            InvadersKilled = 0;
            SummonedPathogen = false;
            IsActive = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["BioDefenders"] = DefendersKilled;
            tag["BioInvaders"] = InvadersKilled;
            tag["PathoSummon"] = SummonedPathogen;
            tag["BioActive"] = IsActive;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IsActive = tag.Get<bool>("BioActive");
            SummonedPathogen = tag.Get<bool>("PathoSummon");
            DefendersKilled = tag.Get<float>("BioDefenders");
            InvadersKilled = tag.Get<float>("BioInvaders");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(DefendersKilled);
            writer.Write(InvadersKilled);
            writer.Write(SummonedPathogen);
            writer.Write(IsActive);
        }

        public override void NetReceive(BinaryReader reader)
        {
            DefendersKilled = reader.ReadSingle();
            InvadersKilled = reader.ReadSingle();
            SummonedPathogen = reader.ReadBoolean();
            IsActive = reader.ReadBoolean();
        }

        public static Entity BioGetTarget(bool defender, NPC npc)
        {
            float currentDist = 0;
            Entity targ = null;
            List<int> enemies = defender ? InvaderNPCs : DefenderNPCs;
            if (defender ? DefendersWinning : InvadersWinning)
            {
                foreach (NPC n in Main.npc)
                {
                    if (n == null)
                        continue;
                    if (!n.active)
                        continue;
                    if (n.life <= 0)
                        continue;
                    if (!enemies.Contains(n.type))
                        continue;
                    if (n.Distance(npc.Center) < currentDist)
                        continue;
                    currentDist = n.Distance(npc.Center);
                    targ = n;
                }
            }
            else
            {
                foreach (NPC n in Main.npc)
                {
                    if (n == null)
                        continue;
                    if (!n.active)
                        continue;
                    if (n.life <= 0)
                        continue;
                    if (!enemies.Contains(n.type))
                        continue;
                    if (n.Distance(npc.Center) < currentDist)
                        continue;
                    currentDist = n.Distance(npc.Center);
                    targ = n;
                }
                foreach (Player n in Main.player)
                {
                    if (n == null)
                        continue;
                    if (!n.active)
                        continue;
                    if (n.dead)
                        continue;
                    if (n.Distance(npc.Center) < currentDist)
                        continue;
                    currentDist = n.Distance(npc.Center);
                    targ = n;
                }
            }
            return targ;
        }
    }

    public class BioWarNPC : GlobalNPC
    {
        public float hitCooldown = 0;
        public override bool InstancePerEntity => true;

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            hitCooldown = binaryReader.ReadSingle();
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(hitCooldown);
        }

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return BioWar.DefenderNPCs.Contains(entity.type) || BioWar.InvaderNPCs.Contains(entity.type);
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.life <= 0)
                return true;
            if (hitCooldown > 0)
            {
                hitCooldown--;
                return true;
            }
            if (BioWar.DefenderNPCs.Contains(npc.type))
            {
                foreach (NPC n in Main.npc)
                {
                    if (n == null)
                        continue;
                    if (!n.active)
                        continue;
                    if (n.life <= 0)
                        continue;
                    if (!BioWar.InvaderNPCs.Contains(n.type))
                        continue;
                    if (!n.getRect().Intersects(npc.getRect()))
                        continue;
                    npc.SimpleStrikeNPC(n.damage, n.direction, false);
                    hitCooldown = 20;
                    if (npc.life <= 0)
                    {
                        NPC.NewNPC(npc.GetSource_Death(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<Malignant>());
                    }
                    break;
                }
                if (hitCooldown > 0)
                {
                    return true;
                }
                foreach (Projectile n in Main.projectile)
                {
                    if (n == null)
                        continue;
                    if (!n.active)
                        continue;
                    if (!BioWar.InvaderProjectiles.Contains(n.type))
                        continue;
                    if (!n.getRect().Intersects(npc.getRect()))
                        continue;
                    npc.SimpleStrikeNPC(n.damage, n.direction, false);
                    n.penetrate--;
                    hitCooldown = 20;
                    break;
                }
            }
            if (BioWar.InvaderNPCs.Contains(npc.type))
            {
                foreach (NPC n in Main.npc)
                {
                    if (n == null)
                        continue;
                    if (!n.active)
                        continue;
                    if (n.life <= 0)
                        continue;
                    if (!BioWar.DefenderNPCs.Contains(n.type))
                        continue;
                    if (!n.getRect().Intersects(npc.getRect()))
                        continue;
                    npc.SimpleStrikeNPC(n.damage, n.direction, false);
                    hitCooldown = 20;
                    break;
                }
                if (hitCooldown > 0)
                {
                    return true;
                }
                foreach (Projectile n in Main.projectile)
                {
                    if (n == null)
                        continue;
                    if (!n.active)
                        continue;
                    if (!BioWar.DefenderProjectiles.Contains(n.type))
                        continue;
                    if (!n.getRect().Intersects(npc.getRect()))
                        continue;
                    npc.SimpleStrikeNPC(n.damage, n.direction, false);
                    n.penetrate--;
                    hitCooldown = 20;
                    break;
                }
            }
            return true;
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (npc.life <= 0)
            {
                if (BioWar.InvaderNPCs.Contains(npc.type))
                    BioWar.InvadersKilled++;
                if (BioWar.DefenderNPCs.Contains(npc.type))
                    BioWar.DefendersKilled++;
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.friendly)
            {
                if (npc.life <= 0)
                {
                    if (BioWar.InvaderNPCs.Contains(npc.type))
                        BioWar.InvadersKilled++;
                    if (BioWar.DefenderNPCs.Contains(npc.type))
                        BioWar.DefendersKilled++;
                }
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (BioWar.InvaderNPCs.Contains(npc.type) && BioWar.InvadersWinning)
                return false;
            if (BioWar.DefenderNPCs.Contains(npc.type) && BioWar.DefendersWinning)
                return false;
            return true;
        }
    }

    public class BioWarProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return BioWar.DefenderProjectiles.Contains(entity.type) || BioWar.InvaderProjectiles.Contains(entity.type);
        }

        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if (BioWar.InvaderProjectiles.Contains(projectile.type) && BioWar.InvadersWinning)
                return false;
            if (BioWar.DefenderProjectiles.Contains(projectile.type) && BioWar.DefendersWinning)
                return false;
            return true;
        }
    }
}