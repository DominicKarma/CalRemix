using Terraria;
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

        }

        public override void SpecialVisuals(Player player, bool isActive)
        {

        }
    }
}
