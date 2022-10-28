﻿using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items;
using Terraria.ModLoader;
using CalRemix.Projectiles;

namespace CalRemix.Items.Weapons
{
    public class AtlasMunitionsBeacon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atlas Munitions Beacon");
            Tooltip.SetDefault("Summons an Atlas Soldier to fight for you\n"+"Takes up 3 minion slots and only one can exist");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 280;
            Item.mana = 20;
            Item.width = 40;
            Item.height = 42;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item113;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AtlasSoldier>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.maxMinions >= 3;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<AstralBar>(), 8).
                AddIngredient(ModContent.ItemType<TitanHeart>(), 1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
