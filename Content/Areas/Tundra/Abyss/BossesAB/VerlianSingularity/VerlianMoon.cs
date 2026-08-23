using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity
{
    public class VerlianMoon : ModNPC
    {
        private float _scale;
        private int ParentIndex
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private ref float Timer => ref NPC.ai[1];
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 100;
            NPC.defense = 11;
            NPC.lifeMax = 4500;
            NPC.scale = 1f;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 10f;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            _scale = MathHelper.Lerp(_scale, 1f, 0.1f);
            Timer++;
            NPC npc = GetParentNPC();
            if (!npc.active)
            {
                NPC.active = false;
            }

            float orbitDistance = 512;
            float radians = Timer * 0.0035f;
            Vector2 orbitingVector = Vector2.UnitY.RotatedBy(radians) * orbitDistance;
            NPC.Center = npc.Center + orbitingVector;

            float radiansStart = -MathHelper.ToRadians(5);
            float radiansEnd = radiansStart + MathHelper.ToRadians(2);
            float osc = ExtraMath.Osc(0f, 1f);
            NPC.rotation = MathHelper.ToRadians(-20) + MathHelper.Lerp(radiansStart, radiansEnd, osc);
        }

        private NPC GetParentNPC()
        {
            NPC npc = Main.npc[ParentIndex];
            return npc;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawScale = Vector2.One * _scale;
            Vector2 drawPosition = NPC.Center - screenPos;

            var lightTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 lightDrawOrigin = lightTexture.Size() / 2f;
            float sparkyRot = NPC.rotation;
            float scaleOsc2 = ExtraMath.Osc(0.4f, 0.5f, speed: 1);
            Color lightTextureDrawColor = Color.White;
            lightTextureDrawColor *= 0.5f;
            lightTextureDrawColor.A = 0;
            spriteBatch.Draw(lightTexture, drawPosition, null, lightTextureDrawColor, sparkyRot, lightDrawOrigin, drawScale * 3.5f * scaleOsc2, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawOrigin = texture.Size() / 2f;

            spriteBatch.Draw(texture, drawPosition, null, drawColor, NPC.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }
    }
}
