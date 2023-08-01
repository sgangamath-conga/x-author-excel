/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using stdole;

namespace Apttus.XAuthor.AppRuntime.Helpers
{
    class OfficeIconHelper
    {
        public static Bitmap ConvertWithAlphaBlend(IPictureDisp ipd)
        {
            // get the info about the HBITMAP inside the IPictureDisp
            DIBSECTION dibsection = new DIBSECTION();
            GetObjectDIBSection((IntPtr)ipd.Handle, Marshal.SizeOf(dibsection), ref dibsection);
            int width = dibsection.dsBm.bmWidth;
            int height = dibsection.dsBm.bmHeight;

            // zero out the RGB values for all pixels with A == 0 
            // (AlphaBlend expects them to all be zero)
            unsafe
            {
                RGBQUAD* pBits = (RGBQUAD*)(void*)dibsection.dsBm.bmBits;

                for (int x = 0; x < dibsection.dsBmih.biWidth; x++)
                    for (int y = 0; y < dibsection.dsBmih.biHeight; y++)
                    {
                        int offset = y * dibsection.dsBmih.biWidth + x;
                        if (pBits[offset].rgbReserved == 0)
                        {
                            pBits[offset].rgbRed = 0;
                            pBits[offset].rgbGreen = 0;
                            pBits[offset].rgbBlue = 0;
                        }
                    }
            }

            // create the destination Bitmap object
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            // get the HDCs and select the HBITMAP
            Graphics graphics = Graphics.FromImage(bitmap);

            IntPtr hdcDest = graphics.GetHdc();
            IntPtr hdcSrc = CreateCompatibleDC(hdcDest);
            IntPtr hobjOriginal = SelectObject(hdcSrc, (IntPtr)ipd.Handle);

            // render the bitmap using AlphaBlend
            BLENDFUNCTION blendfunction = new BLENDFUNCTION(AC_SRC_OVER, 0, 0xFF, AC_SRC_ALPHA);
            AlphaBlend(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, width, height, blendfunction);

            // clean up
            SelectObject(hdcSrc, hobjOriginal);
            DeleteDC(hdcSrc);
            graphics.ReleaseHdc(hdcDest);
            graphics.Dispose();

            return bitmap;
        }

        public static Bitmap ConvertPixelByPixel(IPictureDisp ipd)
        {
            // get the info about the HBITMAP inside the IPictureDisp
            DIBSECTION dibsection = new DIBSECTION();
            GetObjectDIBSection((IntPtr)ipd.Handle, Marshal.SizeOf(dibsection), ref dibsection);
            int width = dibsection.dsBm.bmWidth;
            int height = dibsection.dsBm.bmHeight;

            // create the destination Bitmap object
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            unsafe
            {
                // get a pointer to the raw bits
                RGBQUAD* pBits = (RGBQUAD*)(void*)dibsection.dsBm.bmBits;

                // copy each pixel manually
                for (int x = 0; x < dibsection.dsBmih.biWidth; x++)
                    for (int y = 0; y < dibsection.dsBmih.biHeight; y++)
                    {
                        int offset = y * dibsection.dsBmih.biWidth + x;
                        if (pBits[offset].rgbReserved != 0)
                        {
                            bitmap.SetPixel(x, y, Color.FromArgb(pBits[offset].rgbReserved, pBits[offset].rgbRed, pBits[offset].rgbGreen, pBits[offset].rgbBlue));
                        }
                    }
            }

            return bitmap;
        }

        [DllImport("gdi32.dll", EntryPoint = "GdiAlphaBlend")]
        public static extern bool AlphaBlend(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
           int nWidthDest, int nHeightDest,
           IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
           BLENDFUNCTION blendFunction);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;

            public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
            {
                BlendOp = op;
                BlendFlags = flags;
                SourceConstantAlpha = alpha;
                AlphaFormat = format;
            }
        }

        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GdipCreateBitmapFromHBITMAP(IntPtr hbitmap,
            IntPtr hpalette, out IntPtr bitmap);

        [StructLayout(LayoutKind.Sequential)]
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public Int32 bmType;
            public Int32 bmWidth;
            public Int32 bmHeight;
            public Int32 bmWidthBytes;
            public Int16 bmPlanes;
            public Int16 bmBitsPixel;
            public IntPtr bmBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public Int16 biPlanes;
            public Int16 biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int bitClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DIBSECTION
        {
            public BITMAP dsBm;
            public BITMAPINFOHEADER dsBmih;
            public int dsBitField1;
            public int dsBitField2;
            public int dsBitField3;
            public IntPtr dshSection;
            public int dsOffset;
        }

        [DllImport("gdi32.dll", EntryPoint = "GetObject")]
        public static extern int GetObjectDIBSection(IntPtr hObject, int nCount, ref DIBSECTION lpObject);

    }
}
