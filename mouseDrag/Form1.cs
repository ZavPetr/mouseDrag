using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace mouseDrag
{
    public partial class Form1 : Form
    {
        Random rand = new Random();
        Size velikostDilku;
        bool isMoveing = false;
        int dx = 0;
        int dy = 0;
        Bitmap source;
        Bitmap resizedSource;
        List<Dilek> dilky = new List<Dilek>();
        int pocetSloupcu = 4;
        int pocetRadku = 3;
        int pocetDilku;
        Point pozicePosunu;
        Dilek pohybujiciSeDilek;
        int r2 = 400;

        public Form1()
        {
            naplnBitmap();
            pocetDilku = pocetSloupcu * pocetRadku;
            vytvorDilky();
            InitializeComponent();
        }

        public void naplnBitmap()
        {
            int j = rand.Next(4);

            switch (j)
            {
                case 0:
                    source = new Bitmap(@"C:\Users\zaves\OneDrive\Plocha\racek.jpg");
                    break;
                case 1:
                    source = new Bitmap(@"C:\Users\zaves\OneDrive\Plocha\img.jfif");
                    break;
                case 2:
                    source = new Bitmap(@"C:\Users\zaves\OneDrive\Plocha\img2.jfif");
                    break;
                case 3:
                    source = new Bitmap(@"C:\Users\zaves\OneDrive\Plocha\img3.jpeg");
                    break;
            }

        }

        private void PictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            //e.Graphics.DrawImage(resizedSource, new Point(0, 0));


            //Rectangle r = new Rectangle(poziceCtverce.X, poziceCtverce.Y, size, size);
            //e.Graphics.FillRectangle(color, r);
            //Rectangle section = new Rectangle(30, 80, size, size);

            for (int i = 0; i < pocetRadku; i++)
            {
                for(int j = 0; j < pocetSloupcu; j++)
                {
                    Point p = new Point(pozicePosunu.X + (velikostDilku.Width*j), pozicePosunu.Y + (velikostDilku.Height * i));


                    e.Graphics.DrawRectangle(new Pen(Color.Red), new Rectangle(p, velikostDilku));
                }
            }

            for (int i = 0; i < pocetDilku; i++)
            {
                Dilek dilek = dilky[i];
                e.Graphics.DrawImage(resizedSource, dilek.poziceDilku.X, dilek.poziceDilku.Y, dilek.section, GraphicsUnit.Pixel);
                if(dilek.jeUmistenSpravne == false)
                {
                    e.Graphics.DrawRectangle(new Pen(Color.Blue), new Rectangle(dilek.poziceDilku.X, dilek.poziceDilku.Y, dilek.size.Width, dilek.size.Height));
                }
                

            }


        }

        private void PictureBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMoveing)
            {
                Point point = new Point(e.X - dx, e.Y - dy);
                pohybujiciSeDilek.poziceDilku = point;
                pictureBox1.Invalidate();
            }
        }

        private void PictureBox1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMoveing = false;

            for(int i = 0; i < pocetDilku; i++)
            {
                Dilek d = dilky[i];
                int deltaX = d.spravnaPoziceDilku.X - pohybujiciSeDilek.poziceDilku.X;
                int deltaY = d.spravnaPoziceDilku.Y - pohybujiciSeDilek.poziceDilku.Y;

                int q2 = deltaX * deltaX + deltaY * deltaY;

                if(q2 < r2)
                {
                    int p = 0;
                    pohybujiciSeDilek.poziceDilku = d.spravnaPoziceDilku;

                    if(pohybujiciSeDilek == d)
                    {
                        pohybujiciSeDilek.jeUmistenSpravne = true;

                        for (int j = 0; j < pocetDilku; j++)
                        {
                            if (dilky[j].jeUmistenSpravne == true)
                            {
                                p++;
                            }
                        }

                        if (p == pocetDilku)
                        {
                            novaHra();
                            return;
                        }
                    }

                    pictureBox1.Invalidate();

                    break;
                }
            }
        }

        private void PictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int z = -1;
            for (int i = 0; i < pocetDilku; i++)
            {

                if(dilky[i].jeUmistenSpravne == true)
                {
                    continue;
                }

                if (e.X > dilky[i].poziceDilku.X && e.X < dilky[i].poziceDilku.X + dilky[i].size.Width && e.Y > dilky[i].poziceDilku.Y && e.Y < dilky[i].poziceDilku.Y + dilky[i].size.Height)
                {
                    if(z < dilky[i].z)
                    {
                        z = dilky[i].z;
                        isMoveing = true;
                        dx = e.X - dilky[i].poziceDilku.X;
                        dy = e.Y - dilky[i].poziceDilku.Y;
                        pohybujiciSeDilek = dilky[i];
                    }

                }
            }

            if (z == -1)
            {
                return;
            }
            pohybujiciSeDilek.z = pocetDilku - 1;

            for(int i = z + 1;i < pocetDilku;i++)
            {
                dilky[i].z -= 1;

                dilky[i - 1] = dilky[i];
            }

            dilky[pocetDilku - 1] = pohybujiciSeDilek;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Width = Width;
            pictureBox1.Height = Height; 
            novaHra();
            pictureBox1.Invalidate();
        }

        public void novaHra()
        { 
            resizedSource = new Bitmap(source, new Size((int)((double)pictureBox1.Width * 0.7), (int)((double)pictureBox1.Height * 0.7)));
            int x = (pictureBox1.Width - resizedSource.Width) / 2;
            int y = (pictureBox1.Height - resizedSource.Height) / 2;
            pozicePosunu = new Point(x, y);

            velikostDilku = new Size(resizedSource.Size.Width / pocetSloupcu, resizedSource.Size.Height / pocetRadku);
            int min = Math.Min(velikostDilku.Width, velikostDilku.Height);
            r2 = min / 3;
            r2 *= r2;
            nastavDilky();
            pictureBox1.Invalidate();
        }

        public void vytvorDilky()
        {
            for (int i = 0; i < pocetDilku; i++)
            {
                dilky.Add(new Dilek());
            }
        }

        public void nastavDilky()
        {
            for (int i = 0; i < pocetRadku; i++)
            {
                int y = velikostDilku.Height * i;
                for (int j = 0; j < pocetSloupcu; j++)
                {
                    int x = (velikostDilku.Width * j);
                    Point spravnaPoziceDilku = new Point(x + pozicePosunu.X, y + pozicePosunu.Y);

                    Rectangle section = new Rectangle(x, y, velikostDilku.Width, velikostDilku.Height);
                    Dilek d = dilky[j + (i * pocetSloupcu)];
                    d.spravnaPoziceDilku = spravnaPoziceDilku;
                    d.section = section;
                    d.size = velikostDilku;
                    d.z = j + (i * pocetSloupcu);
                    d.jeUmistenSpravne = false;
                    
                }
            }

            Rectangle mrizka = new Rectangle(pozicePosunu, resizedSource.Size);
            mrizka.X-= velikostDilku.Width/2;
            mrizka.Y-= velikostDilku.Height/2;

            for(int i = 0; i < pocetDilku; i++)
            {
                int x = rand.Next(pictureBox1.Width - velikostDilku.Width);
                int y = rand.Next(pictureBox1.Height - velikostDilku.Height);

                Point sp = new Point(x, y);

                if(mrizka.Contains(sp))
                {
                    i--;
                    continue;
                }

                dilky[i].poziceDilku = sp;


            }
        }
    }
}
