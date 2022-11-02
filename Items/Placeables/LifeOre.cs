﻿using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CalRemix.Tiles;
using CalamityMod.Items;

namespace CalRemix.Items.Placeables
{
	public class LifeOre : ModItem
	{
		public override void SetStaticDefaults() 
		{
			SacrificeTotal = 100;
			DisplayName.SetDefault("Life Ore");
		}
		public override void SetDefaults() 
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 14;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<LifeOreTile>();
			Item.width = 12;
			Item.height = 12;
			Item.value = CalamityGlobalItem.Rarity8BuyPrice;
			Item.rare = ItemRarityID.Yellow;
		}
	}
}