using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LBoLEntitySideloader.Resource.AsyncTextureImport
{
    public enum FREE_IMAGE_FORMAT
    {
        /// <summary>
        /// Unknown format (returned value only, never use it as input value)
        /// </summary>
        FIF_UNKNOWN = -1,
        /// <summary>
        /// Windows or OS/2 Bitmap File (*.BMP)
        /// </summary>
        FIF_BMP = 0,
        /// <summary>
        /// Windows Icon (*.ICO)
        /// </summary>
        FIF_ICO = 1,
        /// <summary>
        /// Independent JPEG Group (*.JPG, *.JIF, *.JPEG, *.JPE)
        /// </summary>
        FIF_JPEG = 2,
        /// <summary>
        /// JPEG Network Graphics (*.JNG)
        /// </summary>
        FIF_JNG = 3,
        /// <summary>
        /// Commodore 64 Koala format (*.KOA)
        /// </summary>
        FIF_KOALA = 4,
        /// <summary>
        /// Amiga IFF (*.IFF, *.LBM)
        /// </summary>
        FIF_LBM = 5,
        /// <summary>
        /// Amiga IFF (*.IFF, *.LBM)
        /// </summary>
        FIF_IFF = 5,
        /// <summary>
        /// Multiple Network Graphics (*.MNG)
        /// </summary>
        FIF_MNG = 6,
        /// <summary>
        /// Portable Bitmap (ASCII) (*.PBM)
        /// </summary>
        FIF_PBM = 7,
        /// <summary>
        /// Portable Bitmap (BINARY) (*.PBM)
        /// </summary>
        FIF_PBMRAW = 8,
        /// <summary>
        /// Kodak PhotoCD (*.PCD)
        /// </summary>
        FIF_PCD = 9,
        /// <summary>
        /// Zsoft Paintbrush PCX bitmap format (*.PCX)
        /// </summary>
        FIF_PCX = 10,
        /// <summary>
        /// Portable Graymap (ASCII) (*.PGM)
        /// </summary>
        FIF_PGM = 11,
        /// <summary>
        /// Portable Graymap (BINARY) (*.PGM)
        /// </summary>
        FIF_PGMRAW = 12,
        /// <summary>
        /// Portable Network Graphics (*.PNG)
        /// </summary>
        FIF_PNG = 13,
        /// <summary>
        /// Portable Pixelmap (ASCII) (*.PPM)
        /// </summary>
        FIF_PPM = 14,
        /// <summary>
        /// Portable Pixelmap (BINARY) (*.PPM)
        /// </summary>
        FIF_PPMRAW = 15,
        /// <summary>
        /// Sun Rasterfile (*.RAS)
        /// </summary>
        FIF_RAS = 16,
        /// <summary>
        /// truevision Targa files (*.TGA, *.TARGA)
        /// </summary>
        FIF_TARGA = 17,
        /// <summary>
        /// Tagged Image File Format (*.TIF, *.TIFF)
        /// </summary>
        FIF_TIFF = 18,
        /// <summary>
        /// Wireless Bitmap (*.WBMP)
        /// </summary>
        FIF_WBMP = 19,
        /// <summary>
        /// Adobe Photoshop (*.PSD)
        /// </summary>
        FIF_PSD = 20,
        /// <summary>
        /// Dr. Halo (*.CUT)
        /// </summary>
        FIF_CUT = 21,
        /// <summary>
        /// X11 Bitmap Format (*.XBM)
        /// </summary>
        FIF_XBM = 22,
        /// <summary>
        /// X11 Pixmap Format (*.XPM)
        /// </summary>
        FIF_XPM = 23,
        /// <summary>
        /// DirectDraw Surface (*.DDS)
        /// </summary>
        FIF_DDS = 24,
        /// <summary>
        /// Graphics Interchange Format (*.GIF)
        /// </summary>
        FIF_GIF = 25,
        /// <summary>
        /// High Dynamic Range (*.HDR)
        /// </summary>
        FIF_HDR = 26,
        /// <summary>
        /// Raw Fax format CCITT G3 (*.G3)
        /// </summary>
        FIF_FAXG3 = 27,
        /// <summary>
        /// Silicon Graphics SGI image format (*.SGI)
        /// </summary>
        FIF_SGI = 28,
        /// <summary>
        /// OpenEXR format (*.EXR)
        /// </summary>
        FIF_EXR = 29,
        /// <summary>
        /// JPEG-2000 format (*.J2K, *.J2C)
        /// </summary>
        FIF_J2K = 30,
        /// <summary>
        /// JPEG-2000 format (*.JP2)
        /// </summary>
        FIF_JP2 = 31,
        /// <summary>
        /// Portable FloatMap (*.PFM)
        /// </summary>
        FIF_PFM = 32,
        /// <summary>
        /// Macintosh PICT (*.PICT)
        /// </summary>
        FIF_PICT = 33,
        /// <summary>
        /// RAW camera image (*.*)
        /// </summary>
        FIF_RAW = 34,
    }

    public enum FREE_IMAGE_FILTER
    {
        FILTER_BOX = 0,
        FILTER_BICUBIC = 1,
        FILTER_BILINEAR = 2,
        FILTER_BSPLINE = 3,
        FILTER_CATMULLROM = 4,
        FILTER_LANCZOS3 = 5
    }

    public class FreeImage
    {

        private const string FreeImageLibrary = "FreeImage.dll";

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Load")]
        public static extern IntPtr FreeImage_Load(FREE_IMAGE_FORMAT format, string filename, int flags);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_OpenMemory")]
        public static extern IntPtr FreeImage_OpenMemory(IntPtr data, uint size_in_bytes);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_CloseMemory")]
        public static extern IntPtr FreeImage_CloseMemory(IntPtr data);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_AcquireMemory")]
        public static extern bool FreeImage_AcquireMemory(IntPtr stream, ref IntPtr data, ref uint size_in_bytes);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_LoadFromMemory")]
        public static extern IntPtr FreeImage_LoadFromMemory(FREE_IMAGE_FORMAT format, IntPtr stream, int flags);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Unload")]
        public static extern void FreeImage_Unload(IntPtr dib);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Save")]
        public static extern bool FreeImage_Save(FREE_IMAGE_FORMAT format, IntPtr handle, string filename, int flags);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_SaveToMemory")]
        public static extern bool FreeImage_SaveToMemory(FREE_IMAGE_FORMAT format, IntPtr dib, IntPtr stream, int flags);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToRawBits")]
        public static extern void FreeImage_ConvertToRawBits(IntPtr bits, IntPtr dib, int pitch, uint bpp, uint red_mask, uint green_mask, uint blue_mask, bool topdown);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertToRawBits")]
        public static extern void FreeImage_ConvertToRawBits(byte[] bits, IntPtr dib, int pitch, uint bpp, uint red_mask, uint green_mask, uint blue_mask, bool topdown);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_ConvertTo32Bits")]
        public static extern IntPtr FreeImage_ConvertTo32Bits(IntPtr handle);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_Rescale")]
        public static extern IntPtr FreeImage_Rescale(IntPtr dib, int dst_width, int dst_height, FREE_IMAGE_FILTER filter);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetWidth")]
        public static extern uint FreeImage_GetWidth(IntPtr handle);

        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetHeight")]
        public static extern uint FreeImage_GetHeight(IntPtr handle);

        // doesn't work?
        [DllImport(FreeImageLibrary, EntryPoint = "FreeImage_GetFileTypeFromMemory")]
        public static extern FREE_IMAGE_FORMAT FreeImage_GetFileTypeFromMemory(IntPtr data, int size = 0);
    }

}
