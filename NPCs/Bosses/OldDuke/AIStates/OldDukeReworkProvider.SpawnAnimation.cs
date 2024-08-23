using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalRemix.NPCs.Bosses.OldDuke
{
    public partial class OldDukeReworkProvider
    {
        /// <summary>
        /// Performs the Old Duke's Spawn Animation state.
        /// </summary>
        public void DoBehavior_SpawnAnimation()
        {
            if (AITimer <= 6)
                NPC.velocity = new Vector2(NPC.SafeDirectionTo(Target.Center).X * 32f, -70f);

            NPC.velocity.X *= 0.93f;
            NPC.velocity.Y *= 0.85f;
            if (NPC.velocity.Length() > 0.01f)
                NPC.rotation = NPC.velocity.ToRotation();

            if (NPC.velocity.Length() >= 19f)
                Frame = 2;
            else if (AITimer % 7 == 6)
                Frame = (Frame + 1) % 6;

            AfterimageOpacity = 0.5f;

            if (MathF.Abs(NPC.velocity.X) > 0.01f)
                NPC.spriteDirection = MathF.Sign(NPC.velocity.X);
        }
    }
}
