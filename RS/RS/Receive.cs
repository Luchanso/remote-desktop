using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Net;
using System.Net.Sockets;
//using System.Drawing;

namespace RS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Receive : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D BackGround;
        private UdpClient udpClient;

        private delegate void NetEvent(byte[] Date);
        private delegate void AsyncWork();
        private event NetEvent GetData;

        public Receive()
        {
            graphics = new GraphicsDeviceManager(this);
            udpClient = new UdpClient(34000);
            GetData += new NetEvent(Receive_GetData);
            new AsyncWork(AsyncReceiver).BeginInvoke(null, null);
            Content.RootDirectory = "Content";
        }        
        
        protected override void Initialize()
        {           
            base.Initialize();
        }

        protected override void LoadContent()
        {            
            spriteBatch = new SpriteBatch(GraphicsDevice);            
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (BackGround != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(BackGround,                    
                    new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                    new Rectangle(0, 0, BackGround.Width, BackGround.Height),
                    Color.White);
                spriteBatch.End();
                this.Window.Title = "Потеряно пакетов: " + countErorr.ToString();
            }

            base.Draw(gameTime);
        }

        int countErorr = 0;
        private void AsyncReceiver()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 0);

            while (true)
            {
                try
                {
                    MemoryStream memoryStream = new MemoryStream();
                    byte[] bytes = udpClient.Receive(ref ep);
                    memoryStream.Write(bytes, 2, bytes.Length - 2);

                    int countMsg = bytes[0] - 1;
                    if (countMsg > 10)
                        throw new Exception("Потеря первого пакета");
                    for (int i = 0; i < countMsg; i++)
                    {
                        byte[] bt = udpClient.Receive(ref ep);
                        memoryStream.Write(bt, 0, bt.Length);
                    }

                    GetData(memoryStream.ToArray());
                    memoryStream.Close();
                }
                catch(Exception ex)
                {
                    countErorr++;                    
                }
            }
        }

        private void Receive_GetData(byte[] Date)
        {
            BackGround = ConvertToTexture2D(Date);
        }

        private Texture2D ConvertToTexture2D(byte[] bytes)
        {
            MemoryStream memoryStream = new MemoryStream(bytes);
            try
            {
                System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(memoryStream);                        
                memoryStream = new MemoryStream();
                bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch(Exception ex)
            {
            }
            return Texture2D.FromStream(GraphicsDevice, memoryStream);
        }
    }
}
