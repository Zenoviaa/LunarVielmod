using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Godrays;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.TilesNew.RainforestTiles;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{
    public class AcaciaSapling : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<RainforestGrass>(), TileID.Gold];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.StyleMultiplier = 3;

            //TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            //TileObjectData.newSubTile.AnchorValidTiles = [ModContent.TileType<ExampleSand>()];
            //TileObjectData.addSubTile(1);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Sapling"));

            TileID.Sets.TreeSapling[Type] = true;
            TileID.Sets.CommonSapling[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do

            DustType = ModContent.DustType<Sparkle>();

            AdjTiles = [TileID.Saplings];
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            // A random chance to slow down growth
            if (!WorldGen.genRand.NextBool(20))
            {
                return;
            }

            Tile tile = Framing.GetTileSafely(i, j); // Safely get the tile at the given coordinates
            bool growSuccess; // A bool to see if the tree growing was successful.

            // Style 0 is for the ExampleTree sapling, and style 1 is for ExamplePalmTree, so here we check frameX to call the correct method.
            // Any pixels before 54 on the tilesheet are for ExampleTree while any pixels above it are for ExamplePalmTree
            if (tile.TileFrameX < 54)
            {
                growSuccess = WorldGen.GrowTree(i, j);
            }
            else
            {
                growSuccess = WorldGen.GrowPalmTree(i, j);
            }

            // A flag to check if a player is near the sapling
            bool isPlayerNear = WorldGen.PlayerLOS(i, j);

            // If growing the tree was a success and the player is near, show growing effects
            if (growSuccess && isPlayerNear)
            {
                WorldGen.TreeGrowFXCheck(i, j);
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
        {
            if (i % 2 == 0)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
        }
    }
    public class AcaciaTreeVineRenderer : ModSystem
    {
        private Asset<Texture2D> _vineTextureAsset;
        private List<Point> _invalidPoints;
        private Dictionary<Point, VerletChain> _vines;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _invalidPoints = new List<Point>(10);
            _vines = new Dictionary<Point, VerletChain>();
            _vineTextureAsset = ModContent.Request<Texture2D>(typeof(AcaciaTreeTop).DirectoryHere() + "/AcaciaTreeTop_Vine");
        }

        public void AddVine(Point point, int segments)
        {
            if (_vines.ContainsKey(point))
                return;

            Vector2 rootPosition = CalculateRootPosition(point);
            VerletChain chain = new VerletChain(segments, rootPosition, Vector2.UnitY * 24);
            chain.points[0].pinned = true;
            chain.points[0].position = rootPosition;
            chain.segmentLength = 24;
            chain.subdivisionCount = 1;
            chain.gravity = 0.25f;
            _vines.Add(point, chain);
        }

        public void KillVine(Point point)
        {
            _vines.Remove(point);
        }


        private Vector2 CalculateRootPosition(Point point)
        {
            Vector2 worldPosition = point.ToWorldCoordinates();
            Vector2 rootPosition = worldPosition;
            rootPosition.Y -= 32;
            rootPosition.X += ExtraMath.Osc(-36, 36, 0, point.X);
            return rootPosition;
        }
        public override void PostDrawTiles()
        {
            base.PostDrawTiles();
            _invalidPoints.Clear();
            int width = Main.screenWidth;
            int height = Main.screenHeight; 
            Rectangle screenRectangle = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, width, height);
            VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();
            foreach(var vine in _vines)
            {
                Vector2 worldPoint = vine.Key.ToWorldCoordinates();
                if (!screenRectangle.Contains(worldPoint.ToPoint()))
                {
                    _invalidPoints.Add(vine.Key);
                    continue;
                }

                VerletChain chain = vine.Value;
                chain.points[0].position = CalculateRootPosition(vine.Key);
                chain.externalForces = Vector2.UnitX * Main.windSpeedCurrent * ExtraMath.Osc(0, 1f, offset: worldPoint.X) * 0.1f;

                for(int i = 0; i < chain.points.Length; i++)
                {
                    Vector2 effector = chain.points[i].position;
                    chain.externalForces += velocityMap.GetVelocity(effector) * 0.2f;
                }
   
                /*
       
                float distanceToEndEffector = Vector2.Distance(Main.LocalPlayer.Center, endEffector);
                if(distanceToEndEffector <= 32)
                {
                    chain.g += Main.LocalPlayer.velocity * 0.02f;
                }*/
                chain.Update();
            }

            for(int i = 0; i < _invalidPoints.Count; i++)
            {
                _vines.Remove(_invalidPoints[i]);
            }
            PixelationManager.QueueSpritebatchDrawAction(RenderPixelatedVines, DrawLayer.OverNPCs);
        }

        private void RenderPixelatedVines(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            foreach(var vine in _vines)
            {
                VerletChain chain = vine.Value;
                Color lightColor = Lighting.GetColor(vine.Key.X, vine.Key.Y);
                int variantFrameIndex = (int)ExtraMath.Osc(0, 2, 0, offset: vine.Key.X);

                int frameHeight = 32;
                int frameWidth = 28;
                int variantOffset = variantFrameIndex * frameHeight * 3;
                Vector2 origin = new Vector2(frameWidth / 2f, 0);

                for (int i = 0; i < chain.points.Length; i++)
                {
                    VerletPoint point = chain.points[i];
                    Vector2 position = point.position;
                    Vector2 drawPosition = position - screenPos;
                    int localFrameIndex;
                    if(i == 0)
                    {
                        //Leaf frame
                        localFrameIndex = 0;
                    } else if (i == chain.points.Length - 1)
                    {
                        //edge frame
                        localFrameIndex = 2;
                    }
                    else
                    {
                        localFrameIndex = 1;
                    }

                    int localFrameY = localFrameIndex * frameHeight;
                    int frameY = localFrameY + variantOffset;
                    Rectangle frame = new Rectangle(0, frameY, frameWidth, frameHeight);
           
                    float rotation;
                    if(i < chain.points.Length - 1)
                    {
                        rotation = (chain.points[i + 1].position - point.position).ToRotation() - MathHelper.PiOver2;
                    }
                    else
                    {
                        rotation = (point.position - chain.points[i - 1].position).ToRotation() - MathHelper.PiOver2;
                    }

                    Vector2 scale = Vector2.One;
                    scale.X *= ExtraMath.Osc(0.3f, 1f, 0, offset: vine.Key.X);
                    spriteBatch.Draw(_vineTextureAsset.Value, drawPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0);
                }
            }
        }
    }

    public class AcaciaTreeTop : ModTile
    {
        private UnifiedRandom _random;
        private Asset<Texture2D> _topsTextureAsset;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            _topsTextureAsset = ModContent.Request<Texture2D>(Texture + "_Tops");

            _random = new UnifiedRandom(0);
            LocalizedText name = CreateMapEntryName();
            TileID.Sets.IsATreeTrunk[Type] = true;
            Main.tileAxe[Type] = true;
            AddMapEntry(new Color(169, 200, 93), name);
            RegisterItemDrop(ItemID.Wood);
        }

        private float GetLeafSway(float offset, float magnitude, float speed)
        {
            return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Main.instance.TilesRenderer.AddSpecialLegacyPoint(new Point(i, j));
        }

        private Rectangle GetTopFrame(int rand)
        {
            int frameWidth = _topsTextureAsset.Width() / 5;
            int frameHeight = _topsTextureAsset.Height();
            Rectangle frame = new Rectangle(frameWidth * rand, 0, frameWidth, frameHeight);
            return frame;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            _random.SetSeed(i + j);
            Vector2 pos = (new Vector2(i + 1, j) + VeilGen.TileAdj) * 16;

            Color color = Lighting.GetColor(i, j);
            Rectangle frame = GetTopFrame(_random.Next(0, 5));
            Vector2 offset = new Vector2(-13, 0);
            spriteBatch.Draw(_topsTextureAsset.Value, pos - Main.screenPosition + offset, frame, color, GetLeafSway(3, 0.05f, 0.008f),
                new Vector2(frame.Width / 2, frame.Height), 1, 0, 1);

            AcaciaTreeVineRenderer vineRenderer = ModContent.GetInstance<AcaciaTreeVineRenderer>();
            vineRenderer.AddVine(new Point(i, j), 6);
            if (Main.rand.NextBool(250))
            {
                GodrayRenderer godrayRenderer = ModContent.GetInstance<GodrayRenderer>();
                Vector2 centerPos = new Point(i, j).ToWorldCoordinates();
                centerPos.Y += 128;
                godrayRenderer.AddGodrayParticle(centerPos + Main.rand.NextVector2Circular(64, 64));
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
                return;

            Framing.GetTileSafely(i, j).HasTile = false;


            AcaciaTreeVineRenderer vineRenderer = ModContent.GetInstance<AcaciaTreeVineRenderer>();
            vineRenderer.KillVine(new Point(i, j));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            short x = 0;
            short y = 0;

            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<AcaciaTree>();

            if (down)
            {
                y = (short)(Main.rand.Next(3) * 18);
            }

            Tile tile = Framing.GetTileSafely(i, j);
            tile.TileFrameX = x;
            tile.TileFrameY = y;
            return false;
        }
    }
    public class AcaciaTree : ModTile
    {
        private UnifiedRandom _random;
        private Asset<Texture2D> _branchTextureAsset;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            _branchTextureAsset = ModContent.Request<Texture2D>(Texture + "_Branches");
            _random = new UnifiedRandom(0);
            LocalizedText name = CreateMapEntryName();
            TileID.Sets.IsATreeTrunk[Type] = true;
            Main.tileAxe[Type] = true;
            AddMapEntry(new Color(169, 200, 93), name);
            RegisterItemDrop(ItemID.Wood);
        }

        private float GetLeafSway(float offset, float magnitude, float speed)
        {
            return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
        }

        private Rectangle GetBranchFrame(int rand)
        {
            int frameWidth = _branchTextureAsset.Width();
            int frameHeight = _branchTextureAsset.Height() / 6;
            Rectangle frame = new Rectangle(0, frameHeight * rand, frameWidth, frameHeight);
            return frame;
        }

        private void DrawBranches(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 pos2 = (new Vector2(i + 1, j) + VeilGen.TileAdj) * 16;
            Color color2 = Lighting.GetColor(i, j);
            _random.SetSeed(i + j);
            SpriteEffects flip = 0;
            if (_random.NextBool(2))
            {
                flip = SpriteEffects.FlipHorizontally;
            }

            bool drawBranch = _random.NextBool(4);
            Vector2 branchoffset = new Vector2(-2, 0);
            if (drawBranch)
            {
                var tex2 = _branchTextureAsset.Value;
                Rectangle frame = GetBranchFrame(_random.Next(0, 6));
                Vector2 origin = new Vector2(0, frame.Height / 2f);
                if (flip == SpriteEffects.FlipHorizontally)
                    branchoffset.X -= frame.Width + 14;
                spriteBatch.Draw(tex2, pos2 + branchoffset - Main.screenPosition, frame, color2.MultiplyRGB(Color.White), GetLeafSway(0, 0.05f, 0.01f),
                   origin, 1, flip, 0);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            DrawBranches(i, j, spriteBatch);
            return base.PreDraw(i, j, spriteBatch);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
                return;

            Framing.GetTileSafely(i, j).HasTile = false;

            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<AcaciaTree>() || Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<AcaciaTreeTop>();
            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<AcaciaTree>();

            if (up)
                WorldGen.KillTile(i, j - 1);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            short x = 0;
            short y = 0;

            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<AcaciaTree>() || Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<AcaciaTreeTop>();
            bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<AcaciaTree>();

            if (up || down)
            {
                y = (short)(Main.rand.Next(3) * 18);
            }

            Tile tile = Framing.GetTileSafely(i, j);
            tile.TileFrameX = x;
            tile.TileFrameY = y;
            return false;
        }
    }
}
