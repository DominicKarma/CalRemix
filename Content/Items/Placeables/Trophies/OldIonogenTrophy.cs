﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalRemix.Content.Tiles.Trophies;
using CalRemix.Content.Items.Potions;
using static Terraria.ModLoader.ModContent;

namespace CalRemix.Content.Items.Placeables.Trophies
{
    public class OldIonogenTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<IonogenTrophy>();
            Tooltip.SetDefault("Help me");
        }
        public override void SetDefaults()
        {
            Item.useStyle = 1;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<OldIonogenTrophyPlaced>();
            Item.width = 12;
            Item.height = 12;
            Item.rare = 1;
        }
    }
}