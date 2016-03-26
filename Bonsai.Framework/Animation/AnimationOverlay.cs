//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Bonsai.Framework.Utility;

//namespace Bonsai.Framework.Animation
//{
//    public enum SpriteAnimationType { SingleFrame, LoopingAnimation, NonLooping_ResetBackToStartingFrame, NonLooping_PauseOnEndFrame }
//    public enum SpriteOriginType { TopLeft, Center }

//    public class AnimationOverlay
//    {
//        public AnimationOverlay(string name, SpriteAnimationType animType, SpriteOriginType origin, int width, int height, int frames, int framerateMillisec, int yOffset)
//        { 
//            drawingRect = new Rectangle(0,yOffset,width,height);

//            Name = name;
//            AnimationType = animType;
//            FrameCount = frames;
//            FrameRateTimer = new MillisecCounter(framerateMillisec);

//            //assign origin
//            if (origin == SpriteOriginType.Center)
//                Origin = new Vector2(drawingRect.Width * 0.5f, drawingRect.Height * 0.5f);
//            else
//                Origin = Vector2.Zero;
//        }

//        public string Name { get; set; }
//        Rectangle drawingRect;
//        public int FrameCellIndex 
//        {
//            get 
//            {
//                return (DrawingRectangle.X == 0 ? 0 : DrawingRectangle.X / DrawingRectangle.Width);
//            }
//            set
//            {
//                if (FrameCellIndex < 0 || FrameCellIndex >= FrameCount)
//                    throw new Exception("New FrameCellIndex out of range");

//                drawingRect.X = (DrawingRectangle.Width * value);
//            }
//        }
//        public int YOffset
//        {
//            get 
//            {
//                return DrawingRectangle.Y;
//            }
//            set
//            {
//                drawingRect.Y = value;
//            }
//        }
//        public int FrameCount { get; set; }
//        public Vector2 Origin { get; set; }
//        public int Width { get { return drawingRect.Width; } }
//        public int Height { get { return drawingRect.Height; } }
//        public Rectangle DrawingRectangle 
//        { 
//            get { return drawingRect; } 
//            private set { drawingRect = value; } 
//        }
//        public SpriteAnimationType AnimationType { get; set; }
//        public MillisecCounter FrameRateTimer { get; set; }
//        public bool Completed { get; private set; }

//        public void Reset()
//        {
//            FrameCellIndex = 0;
//            Completed = false;
//            FrameRateTimer.Reset();
//        }

//        public void Update(int millisecs)
//        {
//            if (AnimationType == SpriteAnimationType.SingleFrame)
//            {
//                if (Completed == false)
//                    Completed = true;
//                return;
//            }

//            //update frame rate timer
//            FrameRateTimer.Update(millisecs);

//            if (FrameRateTimer.Completed)
//            {
//                //are we at end of animation strip
//                if (FrameCellIndex == (FrameCount - 1))
//                {
//                    //act according to the animation type
//                    if (AnimationType == SpriteAnimationType.NonLooping_ResetBackToStartingFrame)
//                    {
//                        //no reset of timer as this is where animation stops
//                        FrameCellIndex = 0;
//                        Completed = true;
//                    }
//                    else if (AnimationType == SpriteAnimationType.NonLooping_PauseOnEndFrame)
//                    {
//                        Completed = true;
//                    }
//                    else if (AnimationType == SpriteAnimationType.LoopingAnimation)
//                    {
//                        //reset back to cell zero and continue
//                        FrameCellIndex = 0;
//                        FrameRateTimer.Reset();
//                    }
//                }
//                else
//                {
//                    //keep going
//                    FrameCellIndex += 1;
//                    FrameRateTimer.Reset();
//                }

//            }

//        }
//    }


//}
