using Bonsai.Framework;
using Bonsai.Framework.ContentLoading;
using Bonsai.MapEditor.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Svg;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Bonsai.MapEditor
{
    public class EditorGame : BonsaiGame
    {
        public EditorGame()
        {
            Content.RootDirectory = "Content";
        }

        SvgDocument mapSvg;
        List<Shape> mapShapes = new List<Shape>();


        protected override void Init()
        {
            base.SetWindow("Bonsai map editor", 800, 600, true);
        }

        protected override void Load(IContentLoader loader)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Open map file";
            var result = ofd.ShowDialog();
            if (result != DialogResult.OK)
                return;

            var mapPath = ofd.FileName;
            mapSvg = SvgDocument.Open(mapPath);
            var elements = mapSvg.Children.FindSvgElementsOf<SvgPath>();

            foreach (SvgPath path in elements)
            {
                var s = new Shape
                {
                    Indices = path.PathData.Select(p => new Vector2(p.Start.X, p.Start.Y)).Distinct().ToList()
                };

                mapShapes.Add(s);
            }

        }

        protected override void Unload()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }


    }
}
