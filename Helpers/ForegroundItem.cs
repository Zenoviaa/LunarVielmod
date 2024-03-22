using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Helpers
{
    public class ForegroundItem
    {
        /// <summary>
        /// The size for this sprite in the draw check.
        /// </summary>
        public virtual Point DrawSize => source.Size().ToPoint();
        public virtual bool SaveMe => false;
        public virtual bool SyncMe => SaveMe;

        public Asset<Texture2D> Texture { get; protected set; }

        public Vector2 Center
        {
            get => position + (source.Size() / 2f);
            set => position = value - (source.Size() / 2f);
        }

        public Vector2 position = new Vector2(0, 0);
        internal Vector2 drawPosition = new Vector2();
        public Vector2 velocity = new Vector2(0, 0);
        public float scale = 1f;
        public Rectangle source = new Rectangle();
        public Color drawColor = Color.White;
        public float rotation = 0f;

        internal bool drawLighted = true;

        public bool killMe = false; //love this

        public ForegroundItem(Vector2 pos, Vector2 vel, float sc, string path)
        {
            position = pos;
            velocity = vel;
            Texture = ModContent.Request<Texture2D>($"Stellamod/Gores/Foreground/{path}");
            scale = sc;
            source = new Rectangle(0, 0, Texture.Width(), Texture.Height());
        }

        public virtual void Update()
        {
            position += velocity;
        }

        public virtual void Draw()
        {
            Main.spriteBatch.Draw(Texture.Value, drawPosition - Main.screenPosition, source, drawColor, rotation, Texture.Size() / 2, scale, SpriteEffects.None, 0f);
        }

        /// <summary>Called when saving this ForegroundItem.</summary>
        /// <returns>Value(s) to be saved.</returns>
        public virtual void Save(TagCompound tag) { }

        /// <summary>Called on world loading.</summary>
        public virtual void Load(TagCompound tag) { }

        public override string ToString() => $"{GetType().Name} at {position.ToTileCoordinates()}\nSIZE: {scale}, SAVE: {SaveMe}, LIGHTED: {drawLighted}";

        public Vector2 DirectionTo(Vector2 target) => Vector2.Normalize(target - Center);
        public float DistanceSQ(Vector2 other) => Vector2.DistanceSquared(Center, other);
    }
}