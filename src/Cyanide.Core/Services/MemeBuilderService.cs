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

            Pen penColor = new Pen(Color.Empty);
            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Image image = Image.FromFile("Resources/Memes/honest.png");
            var imageGraphics = Graphics.FromImage(image);
            imageGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            
            using (Font font1 = new Font("Comic Sans MS", 14, FontStyle.Bold, GraphicsUnit.Point))
            {
                RectangleF rectF1 = new RectangleF(7, 78, 78, 126);
                imageGraphics.DrawString("YOU'RE NOT BEING HONEST, " + str.ToUpper() + ".", font1, Brushes.Black, rectF1, sf);
                imageGraphics.DrawRectangle(penColor, Rectangle.Round(rectF1));
            }

            imageGraphics.Flush();
            imageGraphics.Dispose();
            image.Save(ms, ImageFormat.Png);
            ms.Position = 0;

            return ms;
        }
    }
}