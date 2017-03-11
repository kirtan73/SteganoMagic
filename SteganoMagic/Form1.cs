using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;
using System.Threading;

namespace SteganoMagic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string loadedFilePath;
        long fileSize;
        int fileNameSize;
        FileInfo finfo;
        string binMsg = "";
        string msg = "";
        string i2ic, i2is;

        public bool browsefile(string type, TextBox t)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadedFilePath = openFileDialog1.FileName;
                fileNameSize = justFName(loadedFilePath).Length;
                type = type.ToLower();
                if (!(justFName(loadedFilePath).Substring(fileNameSize - (type.Length), type.Length).Equals(type)))
                {
                    MessageBox.Show("  Select only" + type + " files  ");
                    loadedFilePath = "";
                    fileNameSize = 0;
                    return false;
                }
                else
                {
                    t.Text = loadedFilePath;
                    finfo = new FileInfo(loadedFilePath);
                    fileSize = finfo.Length;
                    return true;
                }
            }
            return false;
        }


        private void button1_Click(object sender, EventArgs e)
        {

            if (browsefile(".txt", textBox1))
            {
                label7.Text = fileSize.ToString() + " Bytes";

                string text = File.ReadAllText(loadedFilePath, Encoding.UTF8);

                var foundIndexes = new List<int>();
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == ' ')
                        foundIndexes.Add(i);
                }
                label8.Text = ((foundIndexes.Count) / 8).ToString() + " Characters";
            }
            else
            {
                label7.Text = "";
                label8.Text = "";
                textBox1.Text = "";
            }
        }

        private string justFName(string path)
        {
            string output;
            int i;
            if (path.Length == 3)   // i.e: "C:\\"
                return path.Substring(0, 1);
            for (i = path.Length - 1; i > 0; i--)
                if (path[i] == '\\')
                    break;
            output = path.Substring(i + 1);
            return output;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string text = File.ReadAllText(loadedFilePath, Encoding.UTF8);

            while (text.Contains("  "))
                text = text.Replace("  ", " ");
            text = text.Trim();

            var foundIndexes = new List<int>();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                    foundIndexes.Add(i);
            }

            string msg = textBox2.Text;

            for (int i = 0; i < msg.Length; i++)
            {
                binMsg += Convert.ToString(msg[i], 2).PadLeft(8, '0');
            }
            binMsg += "11111111";

            int ji = 0;
            for (int i = 0; i < binMsg.Length; i++)
            {
                if (binMsg[i] == '0')
                {
                    Console.Write("0");

                }
                else
                {
                    Console.Write("1");
                    text = text.Insert(foundIndexes[ji], " ");
                    for (int k = ji; k < foundIndexes.Count; k++)
                    {
                        foundIndexes[k]++;
                    }
                }
                ji++;
            }
            label6.Text = text;
            string path = loadedFilePath.Remove(loadedFilePath.Length - fileNameSize) + @"\kk1.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                using (TextWriter tw = new StreamWriter(path))
                {
                    tw.Write("" + text + "");
                    tw.Close();
                }
            }
            else if (File.Exists(path))
            {
                using (TextWriter tw = new StreamWriter(path))
                {
                    MessageBox.Show("File already exist...!");
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            browsefile(".txt", textBox3);
        }

        private void button4_Click(object sender, EventArgs e)
        {

            string text = File.ReadAllText(loadedFilePath, Encoding.UTF8);
            string binMsg = "";


            for (int i = 1; i < text.Length; i++)
            {
                if (text[i - 1] == ' ' && text[i] != ' ')
                    binMsg += "0";
                else if (text[i - 1] == ' ' && text[i] == ' ')
                {
                    binMsg += "1";
                    i++;
                }
            }

            label10.Text = binMsg;
            for (int i = 0; i < binMsg.Length - 8; i += 8)
            {
                if (binMsg[i] == '1' && binMsg[i + 1] == '1' && binMsg[i + 2] == '1' && binMsg[i + 3] == '1' && binMsg[i + 4] == '1' && binMsg[i + 5] == '1' && binMsg[i + 6] == '1' && binMsg[i + 7] == '1')
                {
                    break;
                }
                msg += (char)(128 * int.Parse(binMsg[i].ToString()) + 64 * int.Parse(binMsg[i + 1].ToString()) + 32 * int.Parse(binMsg[i + 2].ToString()) + 16 * int.Parse(binMsg[i + 3].ToString()) + 8 * int.Parse(binMsg[i + 4].ToString()) + 4 * int.Parse(binMsg[i + 5].ToString()) + 2 * int.Parse(binMsg[i + 6].ToString()) + int.Parse(binMsg[i + 7].ToString()));

            }

            label10.Text = msg;
            msg = "";

        }

        private void button5_Click(object sender, EventArgs e)
        {
            browsefile(".jpg", textBox4);
            i2ic = loadedFilePath;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            browsefile(".jpg", textBox5);
            i2is = loadedFilePath;
        }

        //Embedding Image in Image
        private void button7_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            //Secret Image
            Image imgs = Image.FromFile(i2is);
            Bitmap bmps = new Bitmap(imgs);
            int widths = bmps.Width;
            int heights = bmps.Height;

            int i = 0;

            //Carrier Image
            Image imgc = Image.FromFile(i2ic);
            Bitmap bmpc = new Bitmap(imgc);
            int widthc = bmpc.Width;
            int heightc = bmpc.Height;

            String binMsg = "";

            for (int y = 0; y < heights; y++)
            {
                for (int x = 0; x < widths; x++)
                {
                    Color p = bmps.GetPixel(x, y);

                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    i++;
                    binMsg += Convert.ToString(a, 2).PadLeft(8, '0') + Convert.ToString(r, 2).PadLeft(8, '0') + Convert.ToString(g, 2).PadLeft(8, '0') + Convert.ToString(b, 2).PadLeft(8, '0');

                }
            }
            bmps.Dispose();
            
            string sizeS = Convert.ToString(heights, 2).PadLeft(16, '0') + Convert.ToString(widths, 2).PadLeft(16, '0');
            
            Bitmap eMap = new Bitmap(widthc,heightc);
            int h=0;
            int binIndex = 0,k=0;
            for (int y = 0; y < heightc; y++)
            {
                
                for (int x = 0; x < widthc; x++)
                {
                    
                    Color p = bmpc.GetPixel(x, y);

                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    if (x >= (widthc - 8) && y == (heightc - 1) )
                    {
                        if (int.Parse(sizeS[k].ToString()) != a % 2)
                        {
                            if (sizeS[k] == '0') a--;
                            else a++;
                        }
                        k++;
                        if (int.Parse(sizeS[k].ToString()) != r % 2)
                        {
                            if (sizeS[k] == '0') r--;
                            else r++;
                        }
                        k++;
                        if (int.Parse(sizeS[k].ToString()) != g % 2)
                        {
                            if (sizeS[k] == '0') g--;
                            else g++;
                        }
                        k++;
                        if (int.Parse(sizeS[k].ToString()) != b % 2)
                        {
                            if (sizeS[k] == '0') b--;
                            else b++;
                        }
                        k++;
                        eMap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                        h += a + r + g + b;
                    }
                    else
                    {
                        if (binIndex >= binMsg.Length)
                        { }
                        else
                        {
                            if (int.Parse(binMsg[binIndex].ToString()) != r % 2)
                            {
                                if (binMsg[binIndex] == '0') r--;
                                else r++;
                            }
                        }
                        binIndex++;
                        if (binIndex >= binMsg.Length)
                        { }
                        else
                        {
                            if (int.Parse(binMsg[binIndex].ToString()) != g % 2)
                            {
                                if (binMsg[binIndex] == '0') g--;
                                else g++;
                            }
                        }
                        binIndex++;
                        if (binIndex >= binMsg.Length)
                        { }
                        else
                        {
                            if (int.Parse(binMsg[binIndex].ToString()) != b % 2)
                            {
                                if (binMsg[binIndex] == '0') b--;
                                else b++;
                            }
                        }
                        binIndex++;
                        eMap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                    }
                }
            }
            eMap.SetPixel(widthc - 2, heightc - 1, Color.FromArgb(0, 100, 0, 100));
            eMap.Save(loadedFilePath.Remove(loadedFilePath.Length - fileNameSize) + @"\embedded.jpg");
            Cursor.Current = Cursors.Arrow;
            label20.Text = h.ToString();
            eMap.Dispose();
            bmpc.Dispose();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            browsefile(".jpg", textBox9);
        }
        private void button13_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Image imge = Image.FromFile(loadedFilePath);
            Bitmap bmpe = new Bitmap(imge);
            int i = 0;
            string binstream = "";
            int width = bmpe.Width;
            int height = bmpe.Height;
            //int h=0;
            for (int y = height-1; y <= height-1; y++)
            {
                for (int x = width-8; x < width; x++)
                {
                    Color c = bmpe.GetPixel(x, y);
                    if ((c.A % 2) == 0)
                        binstream += '0';
                    else
                        binstream += '1';
                    if ((c.R % 2) == 0)
                        binstream += '0';
                    else
                        binstream += '1';
                    if ((c.G % 2) == 0)
                        binstream += '0';
                    else
                        binstream += '1';
                    if ((c.B % 2) == 0)
                        binstream += '0';
                    else
                        binstream += '1';
                 //   h += c.A + c.R + c.G + c.B;
                }
            }
            //label19.Text = h.ToString();
            int h = bmpe.GetPixel(width - 2, height - 1).A;// Convert.ToInt32(binstream.Substring(0, 16), 2);
            int w = bmpe.GetPixel(width - 2, height - 1).G;//Convert.ToInt32(binstream.Substring(16, 16), 2);

            string path = textBox9.Text.ToString();
            //Bitmap bmps = new Bitmap(w,h);

           label19.Text = w.ToString() + " " +h.ToString();
           /* 
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = bmpe.GetPixel(x, y);

                    if (i>(100*100*32))
                        goto x;
                    i+=3;
                    if ((c.R % 2) == 0)
                        binstream += '0';
                    else
                        binstream += '1';
                    if ((c.G % 2) == 0)
                        binstream += '0';
                    else
                        binstream += '1';
                    if ((c.B % 2) == 0)
                        binstream += '0';
                    else
                        binstream += '1';

                }
            }
            
            x:
            label19.Text = binstream.Length.ToString();
            i = 0;
            
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int a,r, g, b;
                    a = Convert.ToInt32(binstream.Substring(i, 8), 2); i += 8;
                    r = Convert.ToInt32(binstream.Substring(i, 8), 2); i += 8;
                    g = Convert.ToInt32(binstream.Substring(i, 8), 2); i += 8;
                    b = Convert.ToInt32(binstream.Substring(i, 8), 2); i += 8;
            
                    bmps.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }
          
            bmps.Save(loadedFilePath.Remove(loadedFilePath.Length - fileNameSize) + @"\secret.jpg");
            */Cursor.Current = Cursors.Arrow;
        
            }

        private void button8_Click(object sender, EventArgs e)
        {
            browsefile(".jpg", textBox6);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //Text Processing
            string msg = textBox7.Text;
            for (int i = 0; i < msg.Length; i++)
            {
                binMsg += Convert.ToString(msg[i], 2).PadLeft(8, '0');
            }
            binMsg += "11111111";

            //Image Processing
            Image img = Image.FromFile(loadedFilePath);
            Bitmap bmp = new Bitmap(img);
            int width = bmp.Width;
            int height = bmp.Height;
            int binIndex = 0;
            int flag = 0;


            Bitmap eMap = new Bitmap(img);

            for (int y = 0; y < height && flag == 0; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color p = bmp.GetPixel(x, y);

                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    if (int.Parse(binMsg[binIndex].ToString()) != r % 2)
                    {
                        if (binMsg[binIndex] == '0') r--;
                        else r++;
                    }
                    eMap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                    binIndex++;
                    if (binIndex == binMsg.Length) { flag = 1; break; }
                    if (int.Parse(binMsg[binIndex].ToString()) != g % 2)
                    {
                        if (binMsg[binIndex] == '0') g--;
                        else g++;
                    }
                    eMap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                    binIndex++;
                    if (binIndex == binMsg.Length) { flag = 1; break; }
                    if (int.Parse(binMsg[binIndex].ToString()) != b % 2)
                    {
                        if (binMsg[binIndex] == '0') b--;
                        else b++;
                    }
                    eMap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                    binIndex++;
                    if (binIndex == binMsg.Length) { flag = 1; break; }
                }
            }
            eMap.Save(loadedFilePath.Remove(loadedFilePath.Length - fileNameSize) + @"\embedded.png");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            browsefile(".png", textBox8);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Image img = Image.FromFile(loadedFilePath);
            Bitmap bmp = new Bitmap(img);
            int width = bmp.Width;
            int height = bmp.Height;
            int flag = 0;
            label16.Text = "";
            msg = "";
            binMsg = "";
            Bitmap eMap = new Bitmap(img);

            for (int y = 0; y < height && flag == 0; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color p = bmp.GetPixel(x, y);

                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    binMsg += (r % 2).ToString();
                    if (binMsg.Length > 8)
                        if (binMsg.Substring(binMsg.Length - 8, 8).Equals("11111111") && binMsg.Length % 8 == 0)
                        { flag = 1; break; }

                    binMsg += (g % 2).ToString();
                    if (binMsg.Length > 8)
                        if (binMsg.Substring(binMsg.Length - 8, 8).Equals("11111111") && binMsg.Length % 8 == 0)
                        { flag = 1; break; }

                    binMsg += (b % 2).ToString();
                    if (binMsg.Length > 8)
                        if (binMsg.Substring(binMsg.Length - 8, 8).Equals("11111111") && binMsg.Length % 8 == 0)
                        { flag = 1; break; }
                }
            }
            binMsg = binMsg.Remove(binMsg.Length - 8);

            for (int i = 0; i < binMsg.Length; i += 8)
            {
                msg += (char)(128 * int.Parse(binMsg[i].ToString()) + 64 * int.Parse(binMsg[i + 1].ToString()) + 32 * int.Parse(binMsg[i + 2].ToString()) + 16 * int.Parse(binMsg[i + 3].ToString()) + 8 * int.Parse(binMsg[i + 4].ToString()) + 4 * int.Parse(binMsg[i + 5].ToString()) + 2 * int.Parse(binMsg[i + 6].ToString()) + int.Parse(binMsg[i + 7].ToString()));
            }
            label16.Text = msg;

        }






    }
}