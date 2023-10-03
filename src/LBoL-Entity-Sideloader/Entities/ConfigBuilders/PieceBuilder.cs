using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.EntityLib.Cards.Character.Cirno;
using LBoL.EntityLib.EnemyUnits.Lore;
using LBoL.Presentation.Bullet;
using Mono.CSharp;
using Mono.CSharp.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using static LBoLEntitySideloader.Entities.ConfigBuilders.PieceBuilder;

namespace LBoLEntitySideloader.Entities.ConfigBuilders
{
    public class PieceBuilder : ConfigBuilder
    {
        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Id))]
        public class Id : IndentityConverter<int> { }


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Type))]
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


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.ShootType))]
        public class ShootTypeConverter : ITwoWayConverter<ShootType, int>
        {
            public int ConvertTo(ShootType from) => (int)from;

            public ShootType ReverseConvert(int to) => (ShootType)(to);
        }

        public enum ShootType
        {
            Zero,
            One,
            Two,
            Three
        }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.ParentPiece))]
        public class ParentPiece : IndentityConverter<int> { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.AddParentAngle))]
        public class AddParentAngle : IndentityConverter<bool> { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.LastWave))]
        public class LastWave : IndentityConverter<bool> { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.FollowPiece))]
        public class FollowPiece : IndentityConverter<int> { }

        // delay?
        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.ShootEnd))]
        public class ShootEnd : IndentityConverter<int> { }


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.HitAmount))]
        public class HitAmount : IndentityConverter<int> { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.ZeroHitNotDie))]
        public class ZeroHitNotDie : IndentityConverter<bool> { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Scale))]
        public class Scale : IndentityConverter<float> { }


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Color))]
        public class ColorConvert : ITwoWayConverter<Color, int[][]>
        {
            public int[][] ConvertTo(Color color)
            {
                if (color.hueList == null || color.hueList.Empty())
                    return new int[0][];
                switch (color.mode)
                {
                    case Color.Mode.Simple:
                        return new int[1][] { new int[] { (int)color.hueList.First() } };
                    case Color.Mode.Group:
                        return new int[2][] { new int[1] { 1 }, color.hueList.Select(h => (int)h).ToArray() };
                    case Color.Mode.Way:
                        return new int[2][] { new int[1] { 2 }, color.hueList.Select(h => (int)h).ToArray() };
                    case Color.Mode.Random:
                        return new int[2][] { new int[1] { 3 }, color.hueList.Select(h => (int)h).ToArray() };
                    default:
                        throw new ArgumentException("Color.mode is out of range");
                }
            }

            public Color ReverseConvert(int[][] to)
            {
                throw new NotImplementedException();
            }
        }


        public class Color
        {
            public Mode mode;

            public List<HueCases> hueList = new List<HueCases>();
            public enum Mode
            {
                Simple,
                Group,
                Way,
                Random
            }

            public enum HueCases
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


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.RootType))]
        public class RootTypeConvert : ITwoWayConverter<RootType, int>
        {
            public int ConvertTo(RootType from) => (int)from;

            public RootType ReverseConvert(int to) => (RootType)(to);
        }


        public enum RootType
        {
            Zero,
            One,
            Two
        }


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.X))]
        public class XConvert : D2ArrayConverter<float>
        { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Y))]
        public class YConvert : D2ArrayConverter<float>
        { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Radius))]
        public class RadiusConvert : D2ArrayConverter<float>
        { }


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.RadiusA))]
        public class RadiusAConvert : D2ArrayConverter<float>
        { }


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Aim))]
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

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.StartTime))]
        public class StartTime : IndentityConverter<int> { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.GInterval))]
        public class GInterval : IndentityConverter<int> { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Group))]
        public class Group : IndentityConverter<int> { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Way))]
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


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.GAngle))]
        public class GAngleConvert : D2ArrayConverter<float>
        { }


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Range))]
        public class RangeConvert : D2ArrayConverter<float>
        { }


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.Life))]
        public class Life : IndentityConverter<int> { }


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.LaserLastWave))]
        public class LaserLastWave : IndentityConverter<int> { }


        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.StartSpeed))]
        public class StartSpeedConvert : D2ArrayConverter<float>
        { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.StartAcc))]
        public class StartAccConvert : D2ArrayConverter<float>
        { }

        [FieldBind(typeof(PieceConfig), nameof(PieceConfig.StartSpeed))]
        public class StartAccAngleConvert : D2ArrayConverter<float>
        { }


        

    }

    public class BulletEvents
    {
        public List<DecodedBulletEvent> bulletEvents = new List<DecodedBulletEvent>();

        
    }


    public class DecodedBulletEvent
    {

    }






    public class EventTypeConvert : ITwoWayConverter<EventType, int[]>
    {
        public int[] ConvertTo(EventType from)
        {
            int mainEvent = (int)from.mainEvent;
            if (from.mainEvent == EventType.MainEvent.Invalid)
                mainEvent = 99;
            switch (from.valueModifier)
            {
                case EventType.ValueModifier.None:
                    return new int[] { mainEvent };
                case EventType.ValueModifier.Negative:
                    return new int[] { 1, mainEvent };
                case EventType.ValueModifier.Multiplicative:
                    return new int[] { 2, mainEvent };
                default:
                    throw new ArgumentException("Bullet event type valueModifier is out of range");
            }

        }

        public EventType ReverseConvert(int[] to)
        {
            throw new NotImplementedException();
        }
    }

    public class EventType
    {
        public MainEvent mainEvent;
        public ValueModifier valueModifier;

        /// <summary>
        /// has some duplicate unused events
        /// </summary>
        public enum MainEvent
        {
            NoDelta,
            Speed,
            Angle,
            Acc,
            AccAngle,
            EventNumber5,
            Reserved6,
            Reserved7,
            None,
            EventNumber9,
            EventNumber10,
            PositionX,
            PositionY,
            ScaleX,
            ScaleY,
            ScaleX15,
            ScaleX16,
            ScaleY17,
            ScaleX18,
            ScaleY19,
            NoModifier,
            Invalid
        }

        public enum ValueModifier
        {
            None,
            Negative,
            Multiplicative
        }
    }



    public class EventNumberConverter : D2ArrayConverter<float> { }

    public class EvDurationConvert : ITwoWayConverter<EvDuration, int[][]>
    {
        public int[][] ConvertTo(EvDuration evStart)
        {
            switch (evStart.mode)
            {
                case EvDuration.Mode.Zero:
                    return new int[0][];
                case EvDuration.Mode.Direct:
                    return new int[][] { evStart.baseValue.ConvertSelf() };
                case EvDuration.Mode.Group:
                    return new int[][] { evStart.baseValue.ConvertSelf(), evStart.group.ConvertSelf() };
                case EvDuration.Mode.WayAndGroup:
                    return new int[][] { evStart.baseValue.ConvertSelf(), evStart.group.ConvertSelf(), evStart.way.ConvertSelf() };
                default:
                    throw new ArgumentException("EvDuration mode is out of range");

            }
        }

        public EvDuration ReverseConvert(int[][] array)
        {
            var converter = new NumberOption<int>();
            switch (array.Length)
            {
                case 0:
                    return new EvDuration() { mode = EvDuration.Mode.Zero };
                case 1:
                    return new EvDuration() { mode = EvDuration.Mode.Direct, baseValue = converter.ReverseConvert(array[0]) };
                case 2:
                    return new EvDuration() { mode = EvDuration.Mode.Group, baseValue = converter.ReverseConvert(array[0]), group = converter.ReverseConvert(array[1]) };
                case 3:
                    return new EvDuration() { mode = EvDuration.Mode.Group, baseValue = converter.ReverseConvert(array[0]), group = converter.ReverseConvert(array[1]), way = converter.ReverseConvert(array[2]) };

                default:
                    throw new ArgumentOutOfRangeException($"{nameof(EvDurationConvert)} {nameof(EvDurationConvert.ReverseConvert)} {typeof(int)} array is out of range");
            }
        }
    }

    public class EvDuration
    {
        public Mode mode;
        public NumberOption<int> baseValue = new NumberOption<int>();
        public NumberOption<int> group = new NumberOption<int>();
        public NumberOption<int> way = new NumberOption<int>();

        public enum Mode
        {
            Zero,
            Direct,
            Group,
            WayAndGroup
        }
    }


    public class EvStartConvert : ITwoWayConverter<EvStart, int[][]>
    {
        public int[][] ConvertTo(EvStart evStart)
        {
            switch (evStart.mode)
            {
                case EvStart.Mode.Zero:
                    return new int[0][];
                case EvStart.Mode.Direct:
                    return new int[][] { evStart.baseValue.ConvertSelf() };
                case EvStart.Mode.Group:
                    return new int[][] { evStart.baseValue.ConvertSelf(), evStart.group.ConvertSelf() };
                case EvStart.Mode.WayAndGroup:
                    return new int[][] { evStart.baseValue.ConvertSelf(), evStart.group.ConvertSelf(), evStart.way.ConvertSelf()};
                default:
                    throw new ArgumentException("EvStart mode is out of range");

            }
        }

        public EvStart ReverseConvert(int[][] array)
        {
            var converter = new NumberOption<int>();
            switch (array.Length)
            {
                case 0:
                    return new EvStart() { mode = EvStart.Mode.Zero };
                case 1:
                    return new EvStart() { mode = EvStart.Mode.Direct, baseValue = converter.ReverseConvert(array[0]) };
                case 2:
                    return new EvStart() { mode = EvStart.Mode.Group, baseValue = converter.ReverseConvert(array[0]), group = converter.ReverseConvert(array[1]) };
                case 3:
                    return new EvStart() { mode = EvStart.Mode.Group, baseValue = converter.ReverseConvert(array[0]), group = converter.ReverseConvert(array[1]), way = converter.ReverseConvert(array[2]) };

                default:
                    throw new ArgumentOutOfRangeException($"{nameof(EvStartConvert)} {nameof(EvStartConvert.ReverseConvert)} {typeof(int)} array is out of range");
            }
        }
    }

    public class EvStart
    {
        public Mode mode;
        public NumberOption<int> baseValue = new NumberOption<int>();
        public NumberOption<int> group = new NumberOption<int>();
        public NumberOption<int> way = new NumberOption<int>();

        public enum Mode
        {
            Zero,
            Direct,
            Group,
            WayAndGroup
        }
    }



}
