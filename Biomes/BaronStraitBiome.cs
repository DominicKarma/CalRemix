﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalRemix.Biomes
{
    // Shows setting up two basic biomes. For a more complicated example, please request.
    public class BaronStraitBiome : ModBiome
    {
        //public override bool IsPrimaryBiome =>false; // Allows this biome to impact NPC prices

        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

        // Populate the Bestiary Filter
        public override string BestiaryIcon => "CalRemix/Biomes/BaronIcon";
        public override string BackgroundPath => "Terraria/Images/MapBG42";
        public override Color? BackgroundColor => Color.PaleGoldenrod;

        // Use SetStaticDefaults to assign the display name
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baron Strait");
        }

        // Calculate when the biome is active.
        public override bool IsBiomeActive(Player player)
        {
            return CalRemixWorld.baronTiles >= 200;
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TheEndZone");
    }
}