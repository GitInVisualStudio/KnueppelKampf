using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace KnueppelKampfBase.Render
{
    public class RenderUtils
    {

        public static Bitmap BlurImage(Bitmap original, int size, int quality = 1, byte alpha = 255)
        {
            Bitmap clone = new Bitmap(original.Width, original.Height, original.PixelFormat);
            BitmapData bData = clone.LockBits(new Rectangle(0, 0, clone.Width, clone.Height), ImageLockMode.ReadWrite, clone.PixelFormat);
            BitmapData bData1 = original.LockBits(new Rectangle(0, 0, original.Width, original.Height), ImageLockMode.ReadWrite, clone.PixelFormat);
            int var1 = (2 * size) * (2 * size);
            unsafe
            {
                int bitsPerPixel = Image.GetPixelFormatSize(clone.PixelFormat);
                byte* scan0 = (byte*)bData.Scan0.ToPointer();
                byte* scan1 = (byte*)bData1.Scan0.ToPointer();
                int r = 0;
                int g = 0;
                int b = 0;
                for (int i = 0; i < bData.Height; ++i)
                {
                    for (int j = 0; j < bData.Width; ++j)
                    {
                        r = g = b = 0;
                        byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                        int z = 0;
                        for (int k = -size; k <= size; k += quality)
                        {
                            for(int l = -size; l <= size; l += quality)
                            {
                                int y = i + k;
                                int x = j + l;
                                if (x < 0 || x >= bData.Width)
                                    continue;
                                if (y < 0 || y >= bData.Height)
                                    continue;
                                byte* _data = scan1 + y * bData.Stride + x * bitsPerPixel / 8;
                                r += _data[2];
                                g += _data[1];
                                b += _data[0];
                                z++;
                            }
                        }
                        data[2] = (byte)(int)(r / (float)z);
                        data[1] = (byte)(int)(g / (float)z);
                        data[0] = (byte)(int)(b / (float)z);
                    }
                }
                original.UnlockBits(bData1);
                clone.UnlockBits(bData);
                return clone;
            }
        }

    }
}
