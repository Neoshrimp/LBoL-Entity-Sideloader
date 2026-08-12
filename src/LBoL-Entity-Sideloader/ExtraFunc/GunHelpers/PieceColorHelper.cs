using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoLEntitySideloader.ExtraFunc.GunHelpers
{
    /// <summary>
    /// Color IDs for piece colors. Colors 1-12 represent hues around the color wheel.
    /// </summary>
    public enum PieceColor
    {
        /// <summary>No color modification.</summary>
        Default = 0,
        /// <summary>0° hue</summary>
        Red = 1,
        /// <summary>30° hue</summary>
        RedOrange = 2,
        /// <summary>60° hue</summary>
        Orange = 3,
        /// <summary>90° hue</summary>
        Yellow = 4,
        /// <summary>120° hue</summary>
        YellowGreen = 5,
        /// <summary>150° hue</summary>
        Lime = 6,
        /// <summary>180° hue</summary>
        Cyan = 7,
        /// <summary>210° hue</summary>
        SkyBlue = 8,
        /// <summary>240° hue</summary>
        Blue = 9,
        /// <summary>270° hue</summary>
        Purple = 10,
        /// <summary>300° hue</summary>
        Magenta = 11,
        /// <summary>330° hue</summary>
        Pink = 12,
        /// <summary>Desaturated/grayscale</summary>
        Grayscale = 13,
        /// <summary>Random hue (0-360°)</summary>
        RandomHue = 14,
        /// <summary>Random from 12 colors (0°, 30°, 60°, etc.)</summary>
        RandomPalette = 15
    }

    /// <summary>
    /// Color distribution modes for multi-bullet patterns
    /// Use as the first element of the first row.
    /// </summary>
    public enum ColorMode
    {
        /// <summary>
        /// Colors cycle based on group index
        /// </summary>
        CycleByGroup = 1,
        /// <summary>
        /// Colors cycle based on way/bullet index
        /// </summary>
        CycleByWay = 2,
        /// <summary>
        /// Each bullet gets a random color from the palette
        /// </summary>
        RandomPerBullet = 3
    }

    /// <summary>
    /// Helper class for creating color matrices used for the color variable in the piece configs.
    /// Piece Color is a 2D matrix, whose function depends on how many rows (subarrays) it has.
    /// If there is only one row, it will take the first element of that row and color all bullets that way.
    /// If there are 2, the first row's first element determines the mode, and the second row determines the colors.
    /// </summary>
    public static class PieceColorHelper
    {
        /// <summary>
        /// Single constant color for all bullets
        /// </summary>
        /// <param name="color">The color ID to use</param>
        public static int[][] Constant(PieceColor color)
        {
            return new int[][] { new[] { (int)color } };
        }

        /// <summary>
        /// Colors cycle through a palette based on group index
        /// Example: CycleByGroup(Red, Orange, Yellow) 
        /// → Group 0: Red, Group 1: Orange, Group 2: Yellow, Group 3: Red, etc.
        /// </summary>
        public static int[][] CycleByGroup(params PieceColor[] colors)
        {
            return CreateCyclingPattern(ColorMode.CycleByGroup, colors);
        }

        /// <summary>
        /// Colors cycle through a palette based on way/bullet index within each group
        /// Example: CycleByWay(Red, Blue, Green)
        /// → Bullet 0: Red, Bullet 1: Blue, Bullet 2: Green, Bullet 3: Red, etc.
        /// </summary>
        public static int[][] CycleByWay(params PieceColor[] colors)
        {
            return CreateCyclingPattern(ColorMode.CycleByWay, colors);
        }

        /// <summary>
        /// Each bullet randomly picks a color from the provided colors
        /// </summary>
        public static int[][] RandomPerBullet(params PieceColor[] colors)
        {
            return CreateCyclingPattern(ColorMode.RandomPerBullet, colors);
        }

        /// <summary>
        /// Creates a rainbow gradient cycling by group
        /// Uses all 12 colors from the color wheel
        /// </summary>
        public static int[][] RainbowByGroup()
        {
            return CycleByGroup(
                PieceColor.Red,
                PieceColor.RedOrange,
                PieceColor.Orange,
                PieceColor.Yellow,
                PieceColor.YellowGreen,
                PieceColor.Lime,
                PieceColor.Cyan,
                PieceColor.SkyBlue,
                PieceColor.Blue,
                PieceColor.Purple,
                PieceColor.Magenta,
                PieceColor.Pink
            );
        }

        /// <summary>
        /// Creates a rainbow gradient cycling by way/bullet index
        /// </summary>
        public static int[][] RainbowByWay()
        {
            return CycleByWay(
                PieceColor.Red,
                PieceColor.RedOrange,
                PieceColor.Orange,
                PieceColor.Yellow,
                PieceColor.YellowGreen,
                PieceColor.Lime,
                PieceColor.Cyan,
                PieceColor.SkyBlue,
                PieceColor.Blue,
                PieceColor.Purple,
                PieceColor.Magenta,
                PieceColor.Pink
            );
        }

        /// <summary>
        /// All bullets use random hues (special color ID 14)
        /// </summary>
        public static int[][] CompletelyRandom()
        {
            return Constant(PieceColor.RandomHue);
        }

        /// <summary>
        /// All bullets use random curated palette colors (special color ID 15)
        /// </summary>
        public static int[][] RandomFromPalette()
        {
            return Constant(PieceColor.RandomPalette);
        }

        /// <summary>
        /// Private helper to create the cycling pattern structure
        /// </summary>
        private static int[][] CreateCyclingPattern(ColorMode mode, PieceColor[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("Must provide at least one color");
            }

            // Convert enum array to int array
            int[] colorIds = colors.Select(c => (int)c).ToArray();

            return new int[][]
            {
            new[] { (int)mode },  // Row 0: Mode selector
            colorIds              // Row 1: Color palette
            };
        }
    }
}
