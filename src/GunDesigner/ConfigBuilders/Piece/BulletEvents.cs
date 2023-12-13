using System;
using System.Collections.Generic;
using System.Text;
using GunDesigner.ConfigBuilders.Converters;
using static GunDesigner.ConfigBuilders.Piece.EventType;

namespace GunDesigner.ConfigBuilders.Piece
{
    [Serializable]
    public class BulletEvents
    {
        public List<DecodedBulletEvent> bulletEvents = new List<DecodedBulletEvent>();
    }

    [Serializable]
    public class DecodedBulletEvent
    {
        public EvStart evStart = new EvStart();
        public EvDuration evDuration = new EvDuration();
        public D2ArrayEncode<float> eventNumber = new D2ArrayEncode<float>();
        public EventType eventType = new EventType();
    }


    public class EventTypeConvert : ITwoWayConverter<EventType, int[]>
    {
        public int[] ConvertTo(EventType from)
        {
            int mainEvent = (int)from.mainEvent;
            if (from.mainEvent == MainEvent.Invalid)
                mainEvent = 99;
            switch (from.valueModifier)
            {
                case ValueModifier.None:
                    return new int[] { mainEvent };
                case ValueModifier.Negative:
                    return new int[] { mainEvent, 1 };
                case ValueModifier.Multiplicative:
                    return new int[] { mainEvent, 2 };
                default:
                    throw new ArgumentException("Bullet event type valueModifier is out of range");
            }

        }

        public EventType ReverseConvert(int[] to)
        {
            if (to.Length == 0)
                throw new ArgumentOutOfRangeException($"{nameof(EventTypeConvert)} {nameof(EventTypeConvert.ReverseConvert)} {typeof(int)} array does not contain any data");


            MainEvent mainEvent;
            if (to[0] == 99)
                mainEvent = MainEvent.Invalid;
            else
                mainEvent = (MainEvent)to[0];

            if (to.Length == 1)
            {
                return new EventType() { mainEvent = mainEvent, valueModifier = ValueModifier.None };
            }
            switch (to[1])
            {
                case 0:
                    return new EventType() { mainEvent = mainEvent, valueModifier = ValueModifier.None };
                case 1:
                    return new EventType() { mainEvent = mainEvent, valueModifier = ValueModifier.Negative };
                case 2:
                    return new EventType() { mainEvent = mainEvent, valueModifier = ValueModifier.Multiplicative };
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(EventTypeConvert)} {nameof(EventTypeConvert.ReverseConvert)} {typeof(int)} array[1] is out of range");

            }
        }
    }

    [Serializable]
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


    [Serializable]
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

    [Serializable]
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
                    return new int[][] { evStart.baseValue.ConvertSelf(), evStart.group.ConvertSelf(), evStart.way.ConvertSelf() };
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


    [Serializable]
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
