using System;
using System.Drawing;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;

namespace RS_Clinet
{
    public class Sender
    {        
        private IPEndPoint ipEndPoint;
        private UdpClient udpClient;
        private int width;
        private int height;

        public Sender()
        {
        }

        public void Run()
        {
            Load(); // Загружаем данные и получаем размер экрана

            udpClient = new UdpClient();
            Bitmap BackGround = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(BackGround);            

            while (true)
            {
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(width, height));   // Получаем снимок экрана
                byte [] bytes = ConvertToByte(BackGround);                      // Получаем изображение в виде массива байтов
                List<byte[]> lst = CutMsg(bytes);
                for (int i = 0; i < lst.Count; i++)
                {
                    udpClient.Send(lst[i], lst[i].Length, ipEndPoint);                // Отправляем картинку клиенту
                }
            }
        }
           
        // Загружаем данные
        private void Load()
        {
            // Загружаем IP адресс и порт
            using (StreamReader streamReader = new StreamReader("ip.txt"))
            {
                string ip = streamReader.ReadLine();
                int port = Convert.ToInt32(streamReader.ReadLine());
                    
                ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            }

            width = Screen.PrimaryScreen.Bounds.Width;
            height = Screen.PrimaryScreen.Bounds.Height;
        }

        // Конвертируем изображение в массив байтов со сжатием Jpeg
        private byte [] ConvertToByte(Bitmap bmp)
        {
            MemoryStream memoryStream = new MemoryStream();
            bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);            
            return memoryStream.ToArray();
        }

        private List<byte[]> CutMsg(byte[] bt)
        {
            int Lenght = bt.Length;
            byte[] temp;
            List<byte[]> msg = new List<byte[]>();

            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(BitConverter.GetBytes((short)((Lenght / 65500) + 1)), 0, 2);
            memoryStream.Write(bt, 0, bt.Length);
            memoryStream.Position = 0;
            while (Lenght > 0)
            {
                temp = new byte[65500];
                memoryStream.Read(temp, 0, 65500);
                msg.Add(temp);
                Lenght -= 65500;                
            }

            return msg;
        }
    }
}
