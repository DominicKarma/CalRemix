using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace CalRemix.NPCs.Bosses.OldDuke
{
    public class OldDukeReworkProvider(NPC oldDuke)
    {
        /// <summary>
        /// The NPC instance that this rework should apply to.
        /// </summary>
        public NPC NPC
        {
            get;
            init;
        } = oldDuke;

        /// <summary>
        /// The target of the Old Duke instance.
        /// </summary>
        public Player Target => Main.player[NPC.target];

        /// <summary>
        /// The general purpose AI timer for the Old Duke instance.
        /// </summary>
        public int AITimer
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        /// <summary>
        /// The rendering frame for the Old Duke instance.
        /// </summary>
        public int Frame
        {
            get => (int)NPC.localAI[0];
            set => NPC.localAI[0] = value;
        }

        /// <summary>
        /// Executes a single frame of AI for an instance of Old Duke.
        /// </summary>
        public void AI()
        {
            NPC.TargetClosest();
            NPC.velocity = (MathHelper.TwoPi * AITimer / -10f).ToRotationVector2() * 120f;
            NPC.position += NPC.SafeDirectionTo(Main.MouseWorld) * 10f;
            NPC.rotation = NPC.velocity.ToRotation();

            Frame = 5;

            AITimer++;
        }

        /// <summary>
        /// Renders an instance of Old Duke.
        /// </summary>
        /// <param name="screenPos">The screen position. Used for offsetting positions to screen coordinates.</param>
        /// <param name="drawColor">The base draw position of the Old Duke.</param>
        public void Render(Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            Color color = NPC.GetAlpha(drawColor);
            Rectangle frame = texture.Frame(1, Main.npcFrameCount[NPC.type], 0, Frame);
            float rotation = NPC.rotation;
            if (NPC.spriteDirection == -1)
                rotation += MathHelper.Pi;

            SpriteEffects direction = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture, drawPosition, frame, color, rotation, frame.Size() * 0.5f, NPC.scale, direction, 0f);
        }
    }
}
