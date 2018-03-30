using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace Cyanide
{
    public class MemeBuilderService
    {
        public Stream BuildMemeHonest(string str)
        {
            MemoryStream ms = new MemoryStream();

            Image image = Image.FromFile("Resources/Memes/honest.png");
            var imageGraphics = Graphics.FromImage(image);
            imageGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            
            using (Font font1 = new Font("Comic Sans MS", 14, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                RectangleF rectF1 = new RectangleF(7, 78, 78, 126);
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                imageGraphics.DrawString("YOU'RE NOT BEING HONEST, " + str.ToUpper() + ".", font1, Brushes.Black, rectF1, sf);
            }

            imageGraphics.Flush();
            imageGraphics.Dispose();
            image.Save(ms, ImageFormat.Png);
            ms.Position = 0;

            return ms;
        }

        public Stream BuildMemeAsLongAs(string str)
        {
            MemoryStream ms = new MemoryStream();
            
            Image image = Image.FromFile("Resources/Memes/aslongas.png");
            var imageGraphics = Graphics.FromImage(image);
            imageGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            using (Font font = new Font("Comic Sans MS", 17, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                RectangleF rect = new RectangleF(280, 20, 120, 182);
                StringFormat strFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                };

                imageGraphics.DrawString("NO MATTER IF YOU'RE A BOY OR A GIRL, IT'S ALL OKAY AS LONG AS " + str.ToUpper() + "!", font, Brushes.Black, rect, strFormat);
            }
            
            imageGraphics.Flush();
            imageGraphics.Dispose();
            image.Save(ms, ImageFormat.Png);
            ms.Position = 0;

            return ms;
        }
    }
}