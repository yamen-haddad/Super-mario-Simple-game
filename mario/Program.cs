using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace mario
{
    public class FirstLab : GameWindow
    {

        static string TITLE = "first";
        static int WIDTH = 800;
        static int HEIGHT = 600;

        static int animIndex = 0;
        static int animX = 4, animY = 1;
        static float imagestep = (float)1.0 / (float)7.0;
        Vector2[] topleft = new Vector2[]
    {
                new Vector2(0*imagestep,0.0f),
                new Vector2(1*imagestep,0.0f),
                new Vector2(2*imagestep,0.0f),
                new Vector2(3*imagestep,0.0f),
                new Vector2(4*imagestep,0.0f),
                new Vector2(5*imagestep,0.0f),
                new Vector2(6*imagestep,0.0f)
    };
        static int imageId;
        static int pipeId, star1Id, star2Id, flagId, turtleId;

        static int freqdivisor = 1, freqLimit=3;

        double turtlex = 0.35, turtley = 0.1, turtlew = 0.1, turtleh = 0.1;
        double flagx = 0.8, flagy = 0.4, flagw = 0.15, flagh = 0.4;
        double star2x = 0.35, star2y = 0.55, star2w = 0.1, star2h = 0.1;
        double star1x = -0.5, star1y = 0.55, star1w = 0.1, star1h = 0.1;
        double mariox = -1.0, marioy = 0.1, mariow = 0.1, marioh = 0.1;
        double pipex = -0.1, pipey = 0.2, pipew = 0.1, pipeh = 0.2;
        double groundx = -1, groundy = 0, groundw = 2, groundh = 1;

        static double curx, cury, prevx, prevy;
        static double transx, transy, scalex, scaley;
        static double prevtransx, prevtransy;
        static double step = 0.01;
        static double jumpHeight = 0.5;
        static double curJumpHeight = 0.1, landHeight=0.1;
        static bool jumping = false, landing=false;
        static bool star1exists = true, star2exists = true;
        public FirstLab() : base(WIDTH, HEIGHT, GraphicsMode.Default, TITLE) { }

        bool pipeerror(double curxt, double curyt)
        {
            if (twocubeinterscet(curxt, curyt, mariow, marioh, pipex, pipey, pipew, pipeh))
            {
                return true;
            }
            return false;
        }
        bool grounderror(double curxt, double curyt)
        {
            if (twocubeinterscet(curxt, curyt, mariow, marioh, groundx, groundy-step, groundw, groundh))
            {
                return true;
            }
            return false;
        }
        bool flagtouch(double curxt, double curyt)
        {
            if (twocubeinterscet(curxt, curyt, mariow, marioh, flagx, flagy, flagw, flagh))
            {
                return true;
            }
            return false;
        }
        bool turtletouch(double curxt, double curyt)
        {
            if (twocubeinterscet(curxt, curyt, mariow, marioh, turtlex, turtley, turtlew, turtleh))
            {
                return true;
            }
            return false;
        }
        bool star1touch(double curxt, double curyt)
        {
            if (twocubeinterscet(curxt, curyt, mariow, marioh, star1x, star1y, star1w, star1h))
            {
                return true;
            }
            return false;
        }
        bool star2touch(double curxt, double curyt)
        {
            if (twocubeinterscet(curxt, curyt, mariow, marioh, star2x, star2y, star2w, star2h))
            {
                return true;
            }
            return false;
        }
        bool twocubeinterscet(double firx, double firy, double firw, double firh, double secx, double secy, double secw, double sech)
        {
            if (verticalalintersect(firy, firh, secy, sech) && horizontalintersect(firx, firw, secx, secw))
            {
                return true;
            }
            return false;
        }
        bool horizontalintersect(double firx, double firw, double secx, double secw)
        {
            if ((firx >= secx && firx <= secx + secw) || (firx + firw >= secx && firx + firw <= secx + secw))
            {
                return true;
            }
            return false;
        }
        bool verticalalintersect(double firy, double firh, double secy, double sech)
        {
            if ((firy <= secy && firy >= secy - sech) || (firy - firh <= secy && firy - firh >= secy - sech))
            {
                return true;
            }
            return false;
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(Color.White);
            curx = -1; prevx = curx;
            cury = marioy; prevy = cury;
            transx = 0;transy = 0;
            scaley = 1;scalex = 1;
            prevtransx = 0; prevtransy = 0;
            curJumpHeight = 0;
            jumping = false; landing = false;
            star1exists = true; star2exists = true;
            GL.Enable(EnableCap.Texture2D);
            imageId = Utilities.LoadTexture(@"F:\study\4th year\hurry\3rd semester\graphics\practical\marioGame\mario\mario\image\mariomove.jpg");
            pipeId = Utilities.LoadTexture(@"F:\study\4th year\hurry\3rd semester\graphics\practical\marioGame\mario\mario\image\pipe.png");
            star1Id = Utilities.LoadTexture(@"F:\study\4th year\hurry\3rd semester\graphics\practical\marioGame\mario\mario\image\star.png");
            flagId = Utilities.LoadTexture(@"F:\study\4th year\hurry\3rd semester\graphics\practical\marioGame\mario\mario\image\flag.png");
            turtleId = Utilities.LoadTexture(@"F:\study\4th year\hurry\3rd semester\graphics\practical\marioGame\mario\mario\image\turtle.png");
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            freqdivisor = 1;
        }
        void jump()
        {
            jumping = true;
            curJumpHeight = 0.1;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);


            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {


            bool pressed = false;
            prevx = curx;
            prevy = cury;
            prevtransx = transx;
            prevtransy = transy;
            base.OnUpdateFrame(e);
            if (jumping)
            {
                if (cury < jumpHeight)
                {
                    curJumpHeight += step;
                    cury += step;
                    transy += step;
                    landing = false;
                }
                else
                {
                    jumping = false;
                    landing = true;
                }
            }
            if (landing)
            {
                if (cury > landHeight+step)
                {
                    curJumpHeight -= step;
                    cury -= step;
                    transy -= step;

                }
                else
                {
                    jumping = false;
                    landing = false;
                }
            }
            if (Keyboard[OpenTK.Input.Key.Escape])
                Exit();
            if (Keyboard[OpenTK.Input.Key.Space])
            {
                jump();
            }
            if (Keyboard[OpenTK.Input.Key.Right])
            {
                pressed = true;
                curx+=step;
                transx += step;
            }
            if (Keyboard[OpenTK.Input.Key.Left])
            {
                pressed = true;
                curx -= step;
                transx -= step;
            }
            //if (Keyboard[OpenTK.Input.Key.Up])
            //{
            //    pressed = true;
            //    cury += step;
            //    transy += step;
            //}
            if (Keyboard[OpenTK.Input.Key.Down])
            {
                pressed = true;
                cury -= step;
                transy -= step;
            }
            if (pressed)
            {
                if (freqdivisor == freqLimit)
                {
                    freqdivisor = 1;
                    animIndex = (animIndex + 1) % 7;
                }
                else
                {
                    freqdivisor++;
                }
            }
            if (pressed || jumping || landing)
            {

                
                Console.WriteLine("({0},{1})", curx, cury);
                if (pipeerror(curx, cury) || grounderror(curx,cury))
                {
                    curx = prevx;
                    cury = prevy;
                    transx = prevtransx;
                    transy = prevtransy;
                    
                    Console.WriteLine("hitting pipe");
                }
                if (star1exists && star1touch(curx, cury) )
                {
                    star1exists = false;
                    Console.WriteLine("hitting star1");
                    scaley *= 2;
                }
                if(star2exists && star2touch(curx, cury))
                {
                    star2exists = false;
                    Console.WriteLine("hitting  star2");
                    scaley *= 2;
                }
                if (turtletouch(curx,cury))
                {
                    GL.ClearColor(Color.White);
                    curx = -1; prevx = curx;
                    cury = marioy; prevy = cury;
                    transx = 0; transy = 0;
                    scaley = 1; scalex = 1;
                    prevtransx = 0; prevtransy = 0;
                    curJumpHeight = 0;
                    jumping = false; landing = false;
                    star1exists = true; star2exists = true;
                }
                if (flagtouch(curx, cury))
                {
                    Exit();
                }
                }
        }




        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            //ground
            GL.LoadIdentity();
            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.Red);
            GL.Vertex3(groundx, groundy, 0);
            GL.Color3(Color.Red);
            GL.Vertex3(groundx+groundw, groundy, 0);
            GL.Color3(Color.Red);
            GL.Vertex3(groundx+groundw, groundy-groundh, 0);
            GL.Color3(Color.Red);
            GL.Vertex3(groundx, groundy-groundh, 0);
            GL.End();

            GL.Color3(Color.White);

            GL.Translate(transx, transy, 0) ;


            GL.Scale(1, scaley, 1);
            //mario
            GL.BindTexture(TextureTarget.Texture2D, imageId);
            GL.Begin(BeginMode.Quads);
           // GL.Color3(Color.Red);
            GL.TexCoord2(topleft[animIndex]);
            GL.Vertex3( mariox, marioy, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(topleft[animIndex] + new Vector2(imagestep, 0));
            GL.Vertex3( (mariox + mariow), marioy, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(topleft[animIndex] + new Vector2(imagestep, 1));
            GL.Vertex3( (mariox + mariow), marioy - marioh, 0);
           // GL.Color3(Color.Red);
            GL.TexCoord2(topleft[animIndex] + new Vector2(0.0f, 1));
            GL.Vertex3( mariox, marioy - marioh, 0);
            GL.End();


            //pipe
            GL.BindTexture(TextureTarget.Texture2D, pipeId);
            GL.LoadIdentity();
            GL.Begin(BeginMode.Quads);
            //GL.Color3(Color.Red);
            GL.TexCoord2(0, 0);
            GL.Vertex3(pipex, pipey, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(1, 0);
            GL.Vertex3(pipex + pipew, pipey, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(1, 1);
            GL.Vertex3(pipex + pipew, pipey - pipeh, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(0, 1);
            GL.Vertex3(pipex, pipey - pipeh, 0);
            GL.End();

            //first star
            if (star1exists)
            {
                GL.BindTexture(TextureTarget.Texture2D, star1Id);
                GL.LoadIdentity();
                GL.Begin(BeginMode.Quads);
                //GL.Color3(Color.Red);
                GL.TexCoord2(0, 0);
                GL.Vertex3(star1x, star1y, 0);
                //GL.Color3(Color.Red);
                GL.TexCoord2(1, 0);
                GL.Vertex3(star1x + star1w, star1y, 0);
                //GL.Color3(Color.Red);
                GL.TexCoord2(1, 1);
                GL.Vertex3(star1x + star1w, star1y - star1h, 0);
                //GL.Color3(Color.Red);
                GL.TexCoord2(0, 1);
                GL.Vertex3(star1x, star1y - star1h, 0);
                GL.End();

            }

            //second star
            if (star2exists)
            {
                star2Id = star1Id;
                GL.BindTexture(TextureTarget.Texture2D, star2Id);
                GL.LoadIdentity();
                GL.Begin(BeginMode.Quads);
                //GL.Color3(Color.Red);
                GL.TexCoord2(0, 0);
                GL.Vertex3(star2x, star2y, 0);
                //GL.Color3(Color.Red);
                GL.TexCoord2(1, 0);
                GL.Vertex3(star2x + star2w, star2y, 0);
                //GL.Color3(Color.Red);
                GL.TexCoord2(1, 1);
                GL.Vertex3(star2x + star2w, star2y - star2h, 0);
                //GL.Color3(Color.Red);
                GL.TexCoord2(0, 1);
                GL.Vertex3(star2x, star2y - star2h, 0);
                GL.End();
            }

            //flag
            GL.BindTexture(TextureTarget.Texture2D, flagId);
            GL.LoadIdentity();
            GL.Begin(BeginMode.Quads);
            //GL.Color3(Color.Red);
            GL.TexCoord2(0, 0);
            GL.Vertex3(flagx, flagy, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(1, 0);
            GL.Vertex3(flagx + flagw, flagy, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(1, 1);
            GL.Vertex3(flagx + flagw, flagy - flagh, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(0, 1);
            GL.Vertex3(flagx, flagy - flagh, 0);
            GL.End();


            //turtle
            GL.BindTexture(TextureTarget.Texture2D, turtleId);
            GL.LoadIdentity();
            GL.Begin(BeginMode.Quads);
            //GL.Color3(Color.Red);
            GL.TexCoord2(0, 0);
            GL.Vertex3(turtlex, turtley, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(1, 0);
            GL.Vertex3(turtlex + turtlew, turtley, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(1, 1);
            GL.Vertex3(turtlex + turtlew, turtley - turtleh, 0);
            //GL.Color3(Color.Red);
            GL.TexCoord2(0, 1);
            GL.Vertex3(turtlex, turtley - turtleh, 0);
            GL.End();
            SwapBuffers();
        }


        static void Main(string[] args)
        {
            FirstLab tutorial = new FirstLab();


                tutorial.Run();

        }
    }
}
