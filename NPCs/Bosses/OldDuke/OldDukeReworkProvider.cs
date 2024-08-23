using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace CalRemix.NPCs.Bosses.OldDuke
{
    public partial class OldDukeReworkProvider(NPC oldDuke)
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
        /// The target of the Old Duke.
        /// </summary>
        public Player Target => Main.player[NPC.target];

        /// <summary>
        /// The current state for the Old Duke.
        /// </summary>
        public OldDukeAIState CurrentState
        {
            get => (OldDukeAIState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        /// <summary>
        /// The general purpose AI timer for the Old Duke.
        /// </summary>
        public int AITimer
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        /// <summary>
        /// The rendering frame for the Old Duke.
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
            PerformAutomaticRetargeting();

            switch (CurrentState)
            {
                case OldDukeAIState.SpawnAnimation:
                    break;
            }

            AITimer++;
        }

        /// <summary>
        /// Changes the Old Duke's current state.
        /// </summary>
        /// <param name="newState">The new state that the Old Duke should execute.</param>
        public void ChangeState(OldDukeAIState newState)
        {
            AITimer = 0;
            CurrentState = newState;
            NPC.netUpdate = true;
        }

        /// <summary>
        /// Performs automatic retargetting, searching for new targets if the current one is really far away, or dead.
        /// </summary>
        public void PerformAutomaticRetargeting()
        {
            // Pick a target if the current one is invalid.
            bool invalidTargetIndex = NPC.target is < 0 or >= 255;
            if (invalidTargetIndex)
                NPC.TargetClosest();

            // Now that a valid target index has been found, verify that they're alive and sufficiently close.
            bool invalidTarget = Target.dead || !Target.active || !NPC.WithinRange(Target.Center, 4600f - Target.aggro);
            if (invalidTarget)
                NPC.TargetClosest();

            // If there's still no valid target, leave.
            if (Target.dead || !Target.active)
                NPC.active = false;
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
