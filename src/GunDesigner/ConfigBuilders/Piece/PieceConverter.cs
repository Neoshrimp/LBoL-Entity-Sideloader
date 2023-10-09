using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.EnemyUnits.Lore;
using LBoL.Presentation.Bullet;
using GunDesigner.ConfigBuilders.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using static GunDesigner.ConfigBuilders.Piece.PieceConverter.ColorConvert;

namespace GunDesigner.ConfigBuilders.Piece
{
    public class PieceConverter : ConverterContainer
    {
        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Id))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.Id))]
        public class Id : IndentityConverter<int> { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Type))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.projectileType))]
        public class TypeConvert : ITwoWayConverter<ProjectileType, bool>
        {
            public bool ConvertTo(ProjectileType from)
            {
                switch (from)
                {
                    case ProjectileType.Bullet:
                        return false;
                    case ProjectileType.Laser:
                        return true;
                    default:
                        throw new ArgumentException("Projectile type out of range");
                }
            }

            public ProjectileType ReverseConvert(bool to)
            {
                if (!to)
                    return ProjectileType.Bullet;
                if (to)
                    return ProjectileType.Laser;
                throw new ArgumentException("wtf");
            }
        }

        public enum ProjectileType
        {
            Bullet,
            Laser
        }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.ShootType))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.shootType))]
        public class ShootTypeConverter : ITwoWayConverter<ShootType, int>
        {
            public int ConvertTo(ShootType from) => (int)from;

            public ShootType ReverseConvert(int to) => (ShootType)to;
        }

        public enum ShootType
        {
            Zero,
            One,
            Two,
            Three
        }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.ParentPiece))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.parentPiece))]
        public class ParentPiece : IndentityConverter<int> { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.AddParentAngle))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.addParentAngle))]
        public class AddParentAngle : IndentityConverter<bool> { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.LastWave))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.lastWave))]
        public class LastWave : IndentityConverter<bool> { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.FollowPiece))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.followPiece))]
        public class FollowPiece : IndentityConverter<int> { }

        // delay?
        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.ShootEnd))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.shootEnd))]
        public class ShootEnd : IndentityConverter<int> { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.HitInterval))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.laserHitInterval))]
        public class HitInterval : IndentityConverter<int> { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.HitAmount))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.hitAmount))]
        public class HitAmount : IndentityConverter<int> { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.ZeroHitNotDie))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.zeroHitNotDie))]
        public class ZeroHitNotDie : IndentityConverter<bool> { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Scale))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.scale))]
        public class Scale : IndentityConverter<float> { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Color))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.color))]
        public class ColorConvert : ITwoWayConverter<PieceColor, int[][]>
        {
            public int[][] ConvertTo(PieceColor color)
            {
                if (color.hueList == null || color.hueList.Empty())
                    return new int[0][];
                switch (color.mode)
                {
                    case PieceColor.Mode.Simple:
                        return new int[1][] { new int[] { (int)color.hueList.First() } };
                    case PieceColor.Mode.Group:
                        return new int[2][] { new int[1] { 1 }, color.hueList.Select(h => (int)h).ToArray() };
                    case PieceColor.Mode.Way:
                        return new int[2][] { new int[1] { 2 }, color.hueList.Select(h => (int)h).ToArray() };
                    case PieceColor.Mode.Random:
                        return new int[2][] { new int[1] { 3 }, color.hueList.Select(h => (int)h).ToArray() };
                    default:
                        throw new ArgumentException("Color.mode is out of range");
                }
            }

            public PieceColor ReverseConvert(int[][] to)
            {
                switch (to.Length)
                {
                    case 0:
                        return new PieceColor() { mode = PieceColor.Mode.Simple };
                    case 1:
                        return new PieceColor() { mode = PieceColor.Mode.Simple, hueList = new List<PieceColor.HueCase>() { (PieceColor.HueCase)to[0][0] } };
                    case 2:
                        switch (to[0][0])
                        {
                            case 1:
                                return new PieceColor() { mode = PieceColor.Mode.Group, hueList = to[1].Select(i => (PieceColor.HueCase)i).ToList() };
                            case 2:
                                return new PieceColor() { mode = PieceColor.Mode.Way, hueList = to[1].Select(i => (PieceColor.HueCase)i).ToList() };
                            case 3:
                                return new PieceColor() { mode = PieceColor.Mode.Random, hueList = to[1].Select(i => (PieceColor.HueCase)i).ToList() };
                            default:
                                throw new ArgumentOutOfRangeException($"{nameof(ColorConvert)} {nameof(ColorConvert.ReverseConvert)} {typeof(int)} value of array[0][0] is out of range");

                        }
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(ColorConvert)} {nameof(ColorConvert.ReverseConvert)} {typeof(int)} array[0]] is out of range");
                }
            }
        }

        [Serializable]
        public class PieceColor
        {
            public Mode mode = Mode.Simple;

            public List<HueCase> hueList = new List<HueCase>();
            public enum Mode
            {
                Simple,
                Group,
                Way,
                Random
            }

            public enum HueCase
            {
                Zero,
                num0f,
                num0dot083333336f,
                num0dot16666667f,
                num0dot25f,
                num0dot33333334f,
                num0dot41666666f,
                num0dot5f,
                num0dot5833333f,
                num0dot6666667f,
                num0dot75f,
                num0dot8333333f,
                num0dot9166667f,
                deColor,
                Random_Range0_360,
                Random_Range0_12
            }
        }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.RootType))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.rootType))]
        public class RootTypeConvert : ITwoWayConverter<RootType, int>
        {
            public int ConvertTo(RootType from) => (int)from;

            public RootType ReverseConvert(int to) => (RootType)to;
        }


        public enum RootType
        {
            Zero,
            One,
            Two
        }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.X))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.X))]
        public class XConvert : D2ArrayConverter<float>
        { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Y))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.Y))]
        public class YConvert : D2ArrayConverter<float>
        { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Radius))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.radius))]
        public class RadiusConvert : D2ArrayConverter<float>
        { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.RadiusA))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.radiusA))]
        public class RadiusAConvert : D2ArrayConverter<float>
        { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Aim))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.aim))]
        public class AimConvert : ITwoWayConverter<Aim, int>
        {
            public int ConvertTo(Aim from)
            {
                switch (from)
                {
                    case Aim.No:
                        return 1;
                    case Aim.FirstGroupOnly:
                        return 5;
                    case Aim.All:
                        return 3;
                    default:
                        throw new ArgumentException("Aim mode is out of range");

                }
            }

            public Aim ReverseConvert(int to)
            {
                switch (to)
                {
                    case 1:
                        return Aim.No;
                    case 0:
                    case 3:
                        return Aim.All;
                    case 2:
                    case 5:
                        return Aim.FirstGroupOnly;
                }
                throw new ArgumentException($"Aim ReverseConvert: {to} is out of range");
            }
        }


        public enum Aim
        {
            No,
            FirstGroupOnly,
            All
        }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.StartTime))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.startTime))]
        public class StartTime : IndentityConverter<int> { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.GInterval))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.gInterval))]
        public class GInterval : IndentityConverter<int> { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Group))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.group))]
        public class Group : IndentityConverter<int> { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Way))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.way))]
        public class WayConvert : ITwoWayConverter<Way, int[][]>
        {
            public int[][] ConvertTo(Way way)
            {
                switch (way.mode)
                {
                    case Way.Mode.One:
                        return new int[0][];
                    case Way.Mode.Direct:
                        return new int[][] { way.value.ConvertSelf() };
                    case Way.Mode.IncreaseByGroup:
                        return new int[][] { way.value.ConvertSelf(), way.variance.ConvertSelf() };
                    default:
                        throw new ArgumentException("Way mode is out of range");

                }
            }

            public Way ReverseConvert(int[][] array)
            {
                var converter = new NumberOption<int>();
                switch (array.Length)
                {
                    case 0:
                        return new Way() { mode = Way.Mode.One };
                    case 1:
                        return new Way() { mode = Way.Mode.Direct, value = converter.ReverseConvert(array[0]) };
                    case 2:
                        return new Way() { mode = Way.Mode.IncreaseByGroup, value = converter.ReverseConvert(array[0]), variance = converter.ReverseConvert(array[1]) };
                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(WayConvert)} {nameof(WayConvert.ReverseConvert)} {typeof(int)} array is out of range");
                }
            }
        }


        [Serializable]
        public class Way
        {
            public Mode mode;
            public NumberOption<int> value = new NumberOption<int>();
            public NumberOption<int> variance = new NumberOption<int>();

            public enum Mode
            {
                One,
                Direct,
                IncreaseByGroup,
            }
        }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.GAngle))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.gAngle))]
        public class GAngleConvert : D2ArrayConverter<float>
        { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Range))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.range))]
        public class RangeConvert : D2ArrayConverter<float>
        { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.Life))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.life))]
        public class Life : IndentityConverter<int> { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.LaserLastWave))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.laserLastWave))]
        public class LaserLastWave : IndentityConverter<int> { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.StartSpeed))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.startSpeed))]
        public class StartSpeedConvert : D2ArrayConverter<float>
        { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.StartAcc))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.startAcc))]
        public class StartAccConvert : D2ArrayConverter<float>
        { }

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.StartAccAngle))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.startAccAngle))]
        public class StartAccAngleConvert : D2ArrayConverter<float>
        { }


        // bullet events..

        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.VanishV3))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.vanishV3))]
        public class VanishV3 : IndentityConverter<Vector3> { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.LaunchSfx))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.launchSfx))]
        public class LaunchSfx : IndentityConverter<string> { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.HitBodySfx))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.hitBodySfx))]
        public class HitBodySfx : IndentityConverter<string> { }


        [ConfigFieldBind(typeof(PieceConfig), nameof(PieceConfig.HitAnimationSpeed))]
        [ReadableFieldBind(typeof(PieceReadableConfig), nameof(PieceReadableConfig.hitAnimationSpeed))]
        public class HitAnimationSpeed : IndentityConverter<float> { }

    }





}
