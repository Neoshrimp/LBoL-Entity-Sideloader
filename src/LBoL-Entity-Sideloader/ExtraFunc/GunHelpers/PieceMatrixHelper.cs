using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBoLEntitySideloader.ExtraFunc.GunHelpers
{
    /// <summary>
    /// Helper class for creating array matrices used in bullet pattern calculations.
    /// These matrices follow the pattern: base + growth * groupID + growth2 * groupID² + wayGrowth * wayID
    /// Each row can have an optional random range: [value, randomRange]
    /// </summary>
    public static class PieceMatrixHelper
    {
        /// <summary>
        /// Creates a float matrix for properties like angle, speed, position, etc.
        /// </summary>
        /// <param name="baseValue">Starting value (Row 0)</param>
        /// <param name="baseRandom">Random range for base value</param>
        /// <param name="perGroup">Linear growth per group (Row 1)</param>
        /// <param name="perGroupRandom">Random range for linear growth</param>
        /// <param name="perGroupSquared">Quadratic growth per group (Row 2)</param>
        /// <param name="perGroupSquaredRandom">Random range for quadratic growth</param>
        /// <param name="perWay">Linear growth per way/bullet (Row 3)</param>
        /// <param name="perWayRandom">Random range for way growth</param>
        public static float[][] Matrix(
            float baseValue = 0f, float baseRandom = 0f,
            float perGroup = 0f, float perGroupRandom = 0f,
            float perGroupSquared = 0f, float perGroupSquaredRandom = 0f,
            float perWay = 0f, float perWayRandom = 0f)
        {
            var rows = new List<float[]>();

            /// Add all rows, even if they are zero.
            // Add Row 0: Base value
            AddRow(rows, baseValue, baseRandom);

            // Add Row 1: Linear group growth
            AddRow(rows, perGroup, perGroupRandom);

            // Add Row 2: Quadratic group growth
            AddRow(rows, perGroupSquared, perGroupSquaredRandom);

            // Add Row 3: Way growth
            AddRow(rows, perWay, perWayRandom);

            // Remove later rows if they are zero.
            while (rows.Count > 1 && IsZeroRow(rows[rows.Count - 1]))
            {
                rows.RemoveAt(rows.Count - 1);
            }

            // Return at least one row (zero if completely empty)
            return rows.Count > 0 ? rows.ToArray() : new float[][] { new[] { 0f } };
        }

        /// <summary>
        /// Creates an integer matrix for properties like Life, Color, Way counts, etc.
        /// </summary>
        public static int[][] MatrixInt(
            int baseValue = 0, int baseRandom = 0,
            int perGroup = 0, int perGroupRandom = 0,
            int perGroupSquared = 0, int perGroupSquaredRandom = 0,
            int perWay = 0, int perWayRandom = 0)
        {
            var rows = new List<int[]>();

            /// Add all rows, even if they are zero.
            // Add Row 0: Base value
            AddRow(rows, baseValue, baseRandom);

            // Add Row 1: Linear group growth
            AddRow(rows, perGroup, perGroupRandom);

            // Add Row 2: Quadratic group growth
            AddRow(rows, perGroupSquared, perGroupSquaredRandom);

            // Add Row 3: Way growth
            AddRow(rows, perWay, perWayRandom);

            // Remove trailing zero rows
            while (rows.Count > 1 && IsZeroRow(rows[rows.Count - 1]))
            {
                rows.RemoveAt(rows.Count - 1);
            }

            // Return at least one row (zero if completely empty)
            return rows.Count > 0 ? rows.ToArray() : new int[][] { new[] { 0 } };
        }

        /// <summary>
        /// Creates a Way matrix (2x2 max) for calculating bullet counts per group.
        /// Formula: (X1 + Random(-X2, X2)) + ((Y1 + Random(-Y2, Y2)) * groupID)
        /// </summary>
        public static int[][] Way(
            int baseCount = 1, int baseRandom = 0,
            int perGroup = 0, int perGroupRandom = 0)
        {
            var rows = new List<int[]>();

            // Row 0: Base count (always present)
            AddRow(rows, baseCount, baseRandom);

            // Row 1: Per group growth (optional)
            AddRow(rows, perGroup, perGroupRandom);

            // Remove trailing zero rows (but keep at least one)
            while (rows.Count > 1 && IsZeroRow(rows[rows.Count - 1]))
            {
                rows.RemoveAt(rows.Count - 1);
            }

            return rows.ToArray();
        }

        // ===== Helper methods =====

        /// <summary>
        /// Adds a value-random pair row to the matrix.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="value"></param>
        /// <param name="random"></param>
        private static void AddRow(List<float[]> rows, float value, float random)
        {
            if (random != 0f)
            {
                rows.Add(new[] { value, random });
            }
            else
            {
                rows.Add(new[] { value });
            }
        }

        private static void AddRow(List<int[]> rows, int value, int random)
        {
            if (random != 0)
            {
                rows.Add(new[] { value, random });
            }
            else
            {
                rows.Add(new[] { value });
            }
        }

        /// <summary>
        /// Is this array filled with zeros?
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static bool IsZeroRow(float[] row)
        {
            return row.All(v => v == 0f);
        }

        private static bool IsZeroRow(int[] row)
        {
            return row.All(v => v == 0);
        }

        // ===== Convenience methods for common cases =====

        /// <summary>
        /// Constant value (no growth or randomness)
        /// </summary>
        public static float[][] Constant(float value)
        {
            return new float[][] { new[] { value } };
        }

        /// <summary>
        /// Constant integer value
        /// </summary>
        public static int[][] ConstantInt(int value)
        {
            return new int[][] { new[] { value } };
        }

        /// <summary>
        /// Value with random range
        /// </summary>
        public static float[][] Random(float baseValue, float randomRange)
        {
            return new float[][] { new[] { baseValue, randomRange } };
        }

        /// <summary>
        /// Integer value with random range
        /// </summary>
        public static int[][] RandomInt(int baseValue, int randomRange)
        {
            return new int[][] { new[] { baseValue, randomRange } };
        }

        /// <summary>
        /// Linear spread across groups: value increases by perGroup each group
        /// Example: LinearSpread(0, 10) → Group 0: 0, Group 1: 10, Group 2: 20
        /// </summary>
        public static float[][] LinearSpread(float baseValue, float perGroup)
        {
            return new float[][]
            {
            new[] { baseValue },
            new[] { perGroup }
            };
        }

        /// <summary>
        /// Linear spread across ways: value increases by perWay each bullet
        /// Example: WaySpread(0, 5) → Bullet 0: 0, Bullet 1: 5, Bullet 2: 10
        /// </summary>
        public static float[][] WaySpread(float baseValue, float perWay)
        {
            return new float[][]
            {
            new[] { baseValue },
            new[] { 0f },
            new[] { 0f },
            new[] { perWay }
            };
        }

        /// <summary>
        /// Creates an empty/zero matrix
        /// </summary>
        public static float[][] Zero()
        {
            return new float[][] { new[] { 0f } };
        }

        /// <summary>
        /// Creates an empty/zero integer matrix
        /// </summary>
        public static int[][] ZeroInt()
        {
            return new int[][] { new[] { 0 } };
        }
    }
}
