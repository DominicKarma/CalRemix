using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
        /// Whether background visuals should be created by the Old Duke or not.
        /// </summary>
        public bool UseBackgroundVisuals
        {
            get => NPC.localAI[1] == 1f;
            set => NPC.localAI[1] = value.ToInt();
        }

        /// <summary>
        /// The opacity of the Old Duke's afterimages.
        /// </summary>
        public ref float AfterimageOpacity => ref NPC.localAI[2];

        /// <summary>
        /// Executes a single frame of AI for an instance of Old Duke.
        /// </summary>
        public void AI()
        {
            PerformAutomaticRetargeting();

            switch (CurrentState)
            {
                case OldDukeAIState.SpawnAnimation:
                    DoBehavior_SpawnAnimation();
                    break;
            }

            AITimer++;
        }

        /// <summary>
        /// Performs per-frame update resets for the Old Duke, ensuring a default state for certain variables.
        /// </summary>
        public void PerformPreUpdateResets()
        {
            AfterimageOpacity = 0f;
            NPC.dontTakeDamage = true;
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

            int afterimageCount = (int)(Utils.GetLerpValue(20f, 100f, NPC.velocity.Length()) * 32f + 8);
            for (int i = afterimageCount - 1; i >= 1; i--)
            {
                float afterimageOpacity = 1f - i / (float)(afterimageCount - 1f);
                float afterimageIndexFloat = (1f - afterimageOpacity) * 8f;
                int currentAfterimageIndex = (int)afterimageIndexFloat;
                int previousAfterimageIndex = currentAfterimageIndex + 1;
                float afterimageInterpolant = 1f - afterimageIndexFloat % 1f;
                Color afterimageColor = color * MathF.Pow(afterimageOpacity, 2f) * AfterimageOpacity;
                Vector2 afterimageDrawPosition = Vector2.Lerp(NPC.oldPos[previousAfterimageIndex], NPC.oldPos[currentAfterimageIndex], afterimageInterpolant) + NPC.Size * 0.5f - screenPos;

                Main.spriteBatch.Draw(texture, afterimageDrawPosition, frame, afterimageColor, rotation, frame.Size() * 0.5f, NPC.scale, direction, 0f);
            }

            Main.spriteBatch.Draw(texture, drawPosition, frame, color, rotation, frame.Size() * 0.5f, NPC.scale, direction, 0f);
        }
    }
}
