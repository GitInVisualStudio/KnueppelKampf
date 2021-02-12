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

        public static Bitmap BlurImage(Bitmap original, int size)
        {
            Bitmap clone = (Bitmap)original.Clone();
            BitmapData bData = clone.LockBits(new Rectangle(0, 0, clone.Width, clone.Height), ImageLockMode.ReadWrite, clone.PixelFormat);

            unsafe
            {
                int bitsPerPixel = Image.GetPixelFormatSize(clone.PixelFormat) / 8;
                byte* scan0 = (byte*)bData.Scan0.ToPointer();

                for (int i = 0; i < bData.Height; ++i)
                {
                    for (int j = 0; j < bData.Width; ++j)
                    {
                        byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                        byte r = 0;
                        byte g = 0;
                        byte b = 0;
                        for(int k = -size; k <= size; k++)
                        {
                            for(int l = -size; l <= size; l++)
                            {
                                int y = i + k;
                                int x = j + l;
                                if (x < 0 || x >= bData.Width)
                                    continue;
                                if (y < 0 || y >= bData.Height)
                                    continue;
                                byte* _data = scan0 + y * bData.Stride + x * bitsPerPixel / 8;
                                //r += _data[2] / 
                            }
                        }
                        //data is a pointer to the first byte of the 3-byte color data
                        //data[0] = blueComponent;
                        //data[1] = greenComponent;
                        //data[2] = redComponent;
                    }
                }

                clone.UnlockBits(bData);
                return clone;
            }
        }

    }
}
