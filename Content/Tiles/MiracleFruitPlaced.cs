﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using CalamityMod.Items.PermanentBoosters;
using Terraria.GameContent;

namespace CalRemix.Content.Tiles
{
    public class MiracleFruitPlaced : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Terraria.ID.TileID.Sets.DisableSmartCursor[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Miracle Froota");
            AddMapEntry(new Color(66, 245, 158), name);
            RegisterItemDrop(ModContent.ItemType<MiracleFruit>());
            FlexibleTileWand.CreateRubblePlacerMedium().AddVariation(ModContent.ItemType<MiracleFruit>(), Type, 0);
        }
    }
}