using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities
{

    /// <summary>
    /// Wrapper class for a render target that automatically gets resized and disposed
    /// </summary>
    public class ManagedRenderTarget
    {
        private Point _oldScreenSize;
        private int _downSamples;
        private ResizeFunction _resizeFunction;
        private RenderTarget2D _renderTarget;
        private bool _mipMap;
        private SurfaceFormat _surfaceFormat;
        private DepthFormat _depthFormat;
        private ManagedRenderTarget(ResizeFunction resizeFunction, int downSamples = 1, bool mipMap = false, SurfaceFormat surfaceFormat = SurfaceFormat.Color, DepthFormat depthFormat = DepthFormat.None)
        {
 
            _resizeFunction = resizeFunction;
            _mipMap = mipMap;
            _surfaceFormat = surfaceFormat;
            _depthFormat = depthFormat;
            _downSamples = downSamples;
            if (resizeFunction == null)
                _resizeFunction = GetScreenTargetSize;
            active = true;
            //Setting to 1 here just incase we get a division by 0 somewhere for like a single frame
            Width = 1;
            Height = 1;
        }
        public bool NeedsResizing()
        {
            return _oldScreenSize != GetScreenTargetSize();
        }

        public bool active;
        public Vector2 Size() => new Vector2(Width, Height);
        public void Dispose()
        {
            _renderTarget?.Dispose();
            _resizeFunction = null;
            _renderTarget = null;
        }

        public static RenderTarget2D DummyTarget;
        public delegate Point ResizeFunction();

        public int Width { get; private set; }
        public int Height { get; private set; }

        public static Semaphore Semaphore = new(1, 1);


        private void Resize()
        {
            Semaphore.WaitOne();

            Point screenSize = _resizeFunction();
            Point newSize = new Point(screenSize.X / _downSamples, screenSize.Y / _downSamples);
            _renderTarget.Release();
            _renderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, newSize.X, newSize.Y, mipMap: _mipMap, _surfaceFormat, _depthFormat,0, RenderTargetUsage.PlatformContents );

            Width = newSize.X;
            Height = newSize.Y;

            Semaphore.Release();
        }

        public void QueueResize(Point screenSize)
        {
            if (_oldScreenSize == screenSize)
                return;

            _oldScreenSize = screenSize;
            Main.QueueMainThreadAction(Resize);
        }
        public void QueueDispose()
        {
            Main.QueueMainThreadAction(Dispose);
        }



        public Point GetScreenTargetSize()
        {
            return new Point(Main.screenTarget.Width, Main.screenTarget.Height);
        }

        public static void InitializeDummyTarget()
        {
            DummyTarget = new RenderTarget2D(Main.instance.GraphicsDevice, 1, 1);
        }

        public static ManagedRenderTarget New(ResizeFunction resizeFunction = null, int downSamples = 1, bool mipMap = true, SurfaceFormat surfaceFormat = SurfaceFormat.Color, DepthFormat depthFormat = DepthFormat.None)
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            ManagedRenderTarget managedRenderTarget = new ManagedRenderTarget(resizeFunction, downSamples, mipMap, surfaceFormat, depthFormat);

            ManagedRenderTargetManager managedRenderTargetManager = ModContent.GetInstance<ManagedRenderTargetManager>();
            managedRenderTargetManager.AddManagedRenderTarget(managedRenderTarget);
            return managedRenderTarget;
        }

        public static implicit operator RenderTarget2D(ManagedRenderTarget managedRenderTarget)
        {
            //This looks weird, it's just so something gets output while the render target is loading
            //Might cause flickers when you resize the screen depending on what you're using the render target for
            //But that's such a non issue
            if (managedRenderTarget._renderTarget == null ||managedRenderTarget._renderTarget.IsDisposed)
            {
                return DummyTarget;
            }

            return managedRenderTarget._renderTarget;
        }
    }

    /// <summary>
    /// Automatically handles creating and resizing render targets so we don't have to duplicate the code everywhere
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class ManagedRenderTargetManager : ModSystem
    {
        private int _resizeTimer;
        private Point _oldScreenSize;
        private List<ManagedRenderTarget> _managedRenderTargets;
        public override void Load()
        {
            base.Load();
            Main.QueueMainThreadAction(ManagedRenderTarget.InitializeDummyTarget);
        }
        public override void Unload()
        {
            base.Unload();
            ManagedRenderTarget.DummyTarget = null;
            ManagedRenderTarget.Semaphore = null;
            if (_managedRenderTargets == null)
                return;
            Main.QueueMainThreadAction(DisposeRenderTargets);

        }
        private void DisposeRenderTargets()
        {
            foreach (var target in _managedRenderTargets)
            {
                target.Dispose();
            }
            _managedRenderTargets?.Clear();
        }


        public override void OnModLoad()
        {
            base.OnModLoad();
      
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            ResizeRenderTargets();
        }

        public void AddManagedRenderTarget(ManagedRenderTarget managedRenderTarget)
        {
            //Lazy loading
            _managedRenderTargets ??= new List<ManagedRenderTarget>();
            _managedRenderTargets.Add(managedRenderTarget);
        }

        private void ResizeRenderTargets()
        {
            //Wait a little bit before resizing render targets after getting in game
            //This is just to avoid a few issues
            if (Main.gameMenu)
            {
                _resizeTimer = 0;
                return;
            }
            else
            {
                _resizeTimer++;
                if (_resizeTimer < 30)
                    return;
            }
                
            Point screenSize = Main.ScreenSize;
            for(int i = 0; i < _managedRenderTargets.Count; i++)
            {
                ManagedRenderTarget managedRenderTarget = _managedRenderTargets[i];
                //Some render targets don't need to be resized when the screen size changes.
                //So we should check this individually
                if (managedRenderTarget.NeedsResizing())
                {
                    managedRenderTarget.QueueResize(screenSize);
                }

                if (!managedRenderTarget.active)
                {

                    managedRenderTarget.QueueDispose();
                }
            }

            //Remove all render targets that are no longer active
            //They should've already been queued to be disposed
            _managedRenderTargets.RemoveAll(x => !x.active);
        }
    }
}
