using Terraria;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using OldDukeNPC = CalamityMod.NPCs.OldDuke.OldDuke;

namespace CalRemix.NPCs.Bosses.OldDuke
{
    public class OldDukeModScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            int oldDukeIndex = NPC.FindFirstNPC(ModContent.NPCType<OldDukeNPC>());
            if (oldDukeIndex != -1 && Main.npc[oldDukeIndex].TryGetGlobalNPC(out CalRemixGlobalNPC remixGlobalNPC))
                return remixGlobalNPC.OldDukeReworker.UseBackgroundVisuals;

            return false;
        }

        public override void Load()
        {
            Filters.Scene["CalRemix:OldDuke"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);

            ScreenShaderData overlayShader = new SandstormShaderData("FilterSandstormBackground").UseColor(0.58f, 1f, 0.06f).UseSecondaryColor(0.7f, 0.5f, 0.3f).UseImage("Images/Misc/noise").UseIntensity(0.4f);
            Overlays.Scene["CalRemix:OldDuke"] = new SimpleOverlay("Images/Misc/noise", overlayShader, EffectPriority.High, RenderLayers.ForegroundWater);
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalRemix:OldDuke", isActive);
        }
    }
}
