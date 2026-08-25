using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Achievements;
using Stellamod.Core;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH;

public class MagicWitchCauldron : VeilTownNPC
{
    private int _frame;
    private ref float Timer => ref NPC.ai[0];
    private ref float CraftTimer => ref NPC.ai[1];
    private ref float ItemType => ref NPC.ai[2];
    private ref float NeedsMixing => ref NPC.ai[3];
    private float DrinkEaseTime => 45;
    private float CraftEaseTime => 20;
    private Cauldron Cauldron => ModContent.GetInstance<Cauldron>();
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 60;
        NPCID.Sets.ActsLikeTownNPC[Type] = true;
        NPCID.Sets.SpawnsWithCustomName[Type] = true;
        NPCID.Sets.NoTownNPCHappiness[Type] = true;
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Velocity = 1f,
            Direction = 1
        };

        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 128;
        NPC.height = 80;
        NPC.friendly = true;
        NPC.lifeMax = 20;
        NPC.defense = 1;
        NPC.damage = 1;
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.dontCountMe = true;
        //NPC.townNPC = true;
    }
    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.5f;
        if (NPC.frameCounter >= 1f)
        {
            NPC.frameCounter = 0;
            _frame++;
            _frame %= Main.npcFrameCount[Type];
        }


        NPC.frame = CommonDrawing.GetFrame(width: 680, height: 588, _frame, 10, 6);// TextureAssets.Npc[Type].Value.GetFrame(_frame, 10, 6);
    }
    private void Spew()
    {
        for (float f = 0; f < 6; f++)
        {
            Vector2 upwardVelocity = -Vector2.UnitY * 5;
            upwardVelocity *= Main.rand.NextFloat(0.5f, 1.5f);
            upwardVelocity = upwardVelocity.RotatedByRandom(MathHelper.ToRadians(65));

            Vector2 pos = NPC.position;
            pos.X += Main.rand.Next(0, NPC.width);

            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.innerColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
            spawnParams.outerColor = Color.Red;
            spawnParams.scaleRange *= 0.5f;
            DustParticle.Spawn(pos, upwardVelocity, spawnParams);
        }
    }
    public override void AI()
    {
        base.AI();
        if (Main.rand.NextBool(24))
        {
            Vector2 pos = NPC.position;
            pos.X += Main.rand.Next(0, NPC.width);
            BubbleParticle bp = BubbleParticle.Spawn(pos, -Vector2.UnitY, Color.White, Main.rand.NextFloat(0.25f, 0.5f));
            bp.gravity = 0;
        }

        if (CraftTimer <= 0 && Timer <= 0)
        {
            Rectangle npcRect = NPC.getRect();
            foreach (Item item in Main.ActiveItems)
            {
                Rectangle itemRect = item.getRect();
                if (!npcRect.Intersects(itemRect))
                    continue;

                if (!Cauldron.IsMaterial(item.type) && !Cauldron.IsMold(item.type))
                    continue;

                if (item.stack <= 0)
                    continue;

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/CauldronCraft");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);

                Spew();
                Timer = DrinkEaseTime;
                if (MultiplayerHelper.IsHost)
                    Cauldron.AddToBrew(item.type, item.stack);
                item.active = false;
            }
        }

        if (NeedsMixing > 0)
        {
            NeedsMixing--;
        }

        if (NeedsMixing <= 0)
        {
            _brewingMaterials = Cauldron.InsideCauldron.ToList();
        }

        if (NPC.HasBuff(BuffID.OnFire) && Cauldron.CanMix() && MultiplayerHelper.IsHost)
        {
            NeedsMixing = 60;
            Cauldron.MixDaCauldron();
            NPC.DelBuff(NPC.FindBuffIndex(BuffID.OnFire));
            NPC.netUpdate = true;
        }

        if (Cauldron.Results.Count > 0 && CraftTimer <= 0)
        {
            Timer = 0;
            CraftTimer = CraftEaseTime;
            var result = Cauldron.Results.Dequeue();
            if (MultiplayerHelper.IsHost)
            {
                int itemIndex = Item.NewItem(NPC.GetSource_FromThis(), NPC.getRect(),
                    result.item, result.stack);
                float dir = Main.rand.NextBool(2) ? -1 : 1;
                Main.item[itemIndex].shimmered = true;
                Main.item[itemIndex].velocity = -Vector2.UnitY * 8 + new Vector2(-7 * dir, 0);
                Main.item[itemIndex].velocity = Main.item[itemIndex].velocity.RotatedByRandom(MathHelper.ToRadians(65));
                ItemType = result.item;
                NPC.netUpdate = true;
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
            }
            if(Main.netMode != NetmodeID.Server)
            {
                ModContent.GetInstance<WitchsBabySteps>().BrewCountCondition.Value++;
            }
            Spew();
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/CauldronCraft");
            soundStyle.PitchVariance = 0.15f;
            SoundEngine.PlaySound(soundStyle, NPC.position);
        }

        if (ItemType != -1)
        {
            Item item = new Item((int)ItemType);
            string get = item.Name;
            Color color = Color.White;
            if (item.IsAir || item.type == 0)
            {
                color = Color.DarkGray;
                get = "...........";
            }
            CombatText.NewText(NPC.getRect(), color, get, false);
            ItemType = -1;
        }
        if (CraftTimer > 0)
        {
            CraftTimer--;
        }

        if (Timer > 0)
        {
            Timer--;
        }
    }

    public override void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        //  base.DrawOutlines(spriteBatch, screenPos, lightColor);
        if (!_drawOutlines)
            return;
        _drawOutlines = false;
        float o = 2;

        //I should just make an outline RT
        //I'll do that later
        DrawCauldron(spriteBatch, -Vector2.UnitX * o);
        DrawCauldron(spriteBatch, Vector2.UnitX * o);
        DrawCauldron(spriteBatch, Vector2.UnitY * o);
        DrawCauldron(spriteBatch, -Vector2.UnitY * o);
    }

    private void DrawCauldron(SpriteBatch spriteBatch, Vector2 offset)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromNPC(NPC);
        sbDrawer.worldPosition.Y += ExtraMath.Osc(0f, 4f, speed: 1);
        sbDrawer.worldPosition.Y -= 40;

        float range = MathHelper.ToRadians(2);
        float radians = MathHelper.Lerp(-range, range, ExtraMath.Osc(0f, 1f, speed: 2));
        sbDrawer.rotation = radians;
        sbDrawer.scale = Vector2.One * 2;

        float ease = EasingFunction.QuadraticBump(Timer / DrinkEaseTime);
        float ease2 = EasingFunction.QuadraticBump(CraftTimer / CraftEaseTime);
        Vector2 drinkScale = Vector2.Lerp(Vector2.One, new Vector2(0.75f, 1.25f), MathHelper.Clamp(ease + ease2, 0f, 1f));
        sbDrawer.scale *= drinkScale;
        sbDrawer.worldPosition += offset;
        sbDrawer.BottomCenterOrigin();
        sbDrawer.drawOrigin.Y -= 32;
        spriteBatch.Draw(sbDrawer);

        //Flash
        if (Timer > 0 || CraftTimer > 0)
        {
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
            spriteBatch.Restart(effect: whiteShader.Effect);
            sbDrawer.color = Color.Lerp(Color.Transparent, Color.LightGreen * 0.75f, MathHelper.Clamp(ease + ease2, 0f, 1f));
            spriteBatch.Draw(sbDrawer);
            spriteBatch.RestartDefaults();

        }
    }

    //There's probably a way better way to code this
    //But this isn't really performance critical
    //so it's whatever
    private List<float> _oldAlpha;
    private List<Vector2> _oldPositions;
    private List<StoredBrewingMaterial> _brewingMaterials;
    private void InitializeOldArrs()
    {
        _oldAlpha ??= new List<float>();
        if (_oldAlpha.Count < 8)
        {
            for (int i = 0; i < 8; i++)
            {
                _oldAlpha.Add(0);
            }
        }

        _oldPositions ??= new List<Vector2>();
        if (_oldPositions.Count < 8)
        {
            for (int i = 0; i < 8; i++)
            {
                _oldPositions.Add(NPC.Center);
            }
        }

        int alphaIndex = 0;
        for (int i = 0; i < _oldAlpha.Count; i++)
        {
            if (_brewingMaterials.Count > i && NeedsMixing <= 0)
            {
                if (!Cauldron.IsMaterial(_brewingMaterials[i].item))
                {
                    continue;
                }
                float targetAlpha = 1f;
                float lerp = MathHelper.Lerp(_oldAlpha[alphaIndex], targetAlpha, 0.1f);
                _oldAlpha[alphaIndex] = lerp;
            }
            else
            {
                float targetAlpha = 0f;
                float lerp = MathHelper.Lerp(_oldAlpha[alphaIndex], targetAlpha, 0.1f);
                _oldAlpha[alphaIndex] = lerp;
            }

            alphaIndex++;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_brewingMaterials == null)
            return false;
        InitializeOldArrs();
        DrawCauldron(spriteBatch, Vector2.Zero);


        //Godrays here
        Asset<Texture2D> godrayTexture = AssetManager.GlowMask.SimpleGlowCircle;
        Vector2 origin = godrayTexture.Size() * 0.5f;
        Vector2 godrayScale = new Vector2(0.35f, 2f);

        float godrayOffset = 150;
        SpritebatchDrawer sbDrawer2 = SpritebatchDrawer.FromTextureAsset(godrayTexture, NPC.Center);
        sbDrawer2.color = Color.White * 0.2f * ExtraMath.Osc(0f, 1f, speed: 1) * LightingHelper.DayLightEase;
        sbDrawer2.color.A = 0;
        sbDrawer2.rotation -= MathHelper.ToRadians(25);
        sbDrawer2.scale *= godrayScale;
        sbDrawer2.worldPosition.Y -= godrayOffset;
        Main.spriteBatch.Draw(sbDrawer2);

        sbDrawer2.color = Color.White * 0.2f * ExtraMath.Osc(0f, 1f, speed: 1, offset: 1) * LightingHelper.DayLightEase;
        sbDrawer2.color.A = 0;
        sbDrawer2.worldPosition += Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * 1) * 64;
        sbDrawer2.worldPosition.Y -= godrayOffset;
        Main.spriteBatch.Draw(sbDrawer2);
        float scale = 1f;
        Vector2 offset = new Vector2();
        offset.Y -= 96;
        float moldIndex = 0;
        float index = 0;
        float count = 0;
        foreach (var sbm in _brewingMaterials)
        {
            if (Cauldron.IsMaterial(sbm.item))
                count++;
        }

        foreach (var sbm in _brewingMaterials)
        {
            Vector2 pos = NPC.Center;
            pos.Y += ExtraMath.Osc(0f, 4f, speed: 2, offset: index);
            ModItem modItem = ModContent.GetModItem(sbm.item);
            pos.Y -= 128;
            if (!Cauldron.IsMaterial(sbm.item))
            {
                pos.Y += 64;
                pos.X += ExtraMath.Osc(-16, 16, speed: 2, moldIndex);
                SpritebatchDrawer moldDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Item[sbm.item], pos);
                Main.spriteBatch.Draw(moldDrawer);
                moldIndex++;
                continue;
            }


            float circleRange = MathHelper.Lerp(25, 350, count / 5f);


            Vector2 left = pos + Vector2.UnitX * -circleRange;
            Vector2 right = pos + Vector2.UnitX * circleRange;

            float c = count - 1;
            if (c <= 0)
                c = 1;
            Vector2 interpPos = Vector2.Lerp(left, right, index / c);

            if (count == 1)
            {
                interpPos = Vector2.Lerp(left, right, 0.5f);
            }

            float alpha = _oldAlpha[(int)index];

            Vector2 targetCirclePos = interpPos;

            //For the brew animation
            if (NeedsMixing > 0)
                targetCirclePos = NPC.Center;

            Vector2 lerp = Vector2.Lerp(_oldPositions[(int)index], targetCirclePos, 0.1f);
            _oldPositions[(int)index] = lerp;
            Vector2 circleCenterPos = lerp;

            float glowProgress = sbm.stack / 10f;
            Color rarityColor = RarityLoader.GetRarity(modItem.Item.rare).RarityColor;
            Color glowingColor = Color.Lerp(Color.Lerp(Color.Black, rarityColor, 0.5f), rarityColor, glowProgress);

            SpritebatchDrawer darknessDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BasicGlow"), circleCenterPos);
            darknessDrawer.color = Color.Black * alpha;
            darknessDrawer.rotation = 0;
            darknessDrawer.scale *= 1.6f;
            Main.spriteBatch.Draw(darknessDrawer);


            SpritebatchDrawer magicCircleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.MagicCircle, circleCenterPos);
            magicCircleDrawer.color = glowingColor * 0.8f * alpha;
            magicCircleDrawer.color.A = 0;
            magicCircleDrawer.rotation = Main.GlobalTimeWrappedHourly * 0.5f;
            magicCircleDrawer.scale *= 0.5f;
            Main.spriteBatch.Draw(magicCircleDrawer);

            Vector2 drawPos = circleCenterPos;



            /*
            SpritebatchDrawer iconDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Item[sbm.item], drawPos);

            iconDrawer.color *= alpha;
            Main.spriteBatch.Draw(iconDrawer);
            */
            ItemSlot.DrawItemIcon(modItem.Item, 0, spriteBatch, drawPos - screenPos, 1f, 32, Color.White * alpha);


            SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, drawPos);
            glowDrawer.color = glowingColor * 0.4f * ExtraMath.Osc(0.5f, 1f, speed: 2, offset: index) * alpha;
            glowDrawer.color.A = 0;
            glowDrawer.scale *= 0.3f;
            Main.spriteBatch.Draw(glowDrawer);
            if (sbm.stack >= 10)
            {
                Color g = Color.White;
                g *= ExtraMath.Osc(0.5f, 1f, speed: 18) * alpha;
                g.A = 0;
                ItemSlot.DrawItemIcon(modItem.Item, 0, spriteBatch, drawPos - screenPos, 1f, 32, g);

            }



            /*
            for (float i = 0; i < sbm.stack; i++)
            {
                float ratio = i / 10f;
                float circleRadians = ratio * MathHelper.TwoPi;
                Vector2 circleOffset = circleRadians.ToRotationVector2();
                circleOffset *= 64;
                Vector2 drawPos = circleCenterPos + circleOffset;
                SpritebatchDrawer iconDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Item[sbm.item], drawPos);
                Main.spriteBatch.Draw(iconDrawer);
            }
            */

            string text = $"x{sbm.stack} {modItem.DisplayName.Value}";
            Vector2 size = FontAssets.ItemStack.Value.MeasureString(text);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, text,
                circleCenterPos - screenPos + new Vector2(0, 32), Color.White * alpha, 0f, size * 0.5f, new Vector2(scale), -1, scale);
            offset.Y -= 24;
            index++;
        }
        return false;
    }
    public override void Interact()
    {
        //        base.Interact();
        //Hacky way to do netcode lol
        NPC.AddBuff(BuffID.OnFire, 60);
    }
    public override void OnKill()
    {
        base.OnKill();
    }
}
