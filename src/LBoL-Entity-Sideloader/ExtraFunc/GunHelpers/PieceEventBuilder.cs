using DG.Tweening.Core.Easing;
using LBoL.ConfigData;
using LBoL.Presentation.Bullet;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace LBoLEntitySideloader.ExtraFunc.GunHelpers
{

    /// <summary>
    /// Represents a single piece event
    /// </summary>
    public class PieceEvent
    {
        public PieceEventType Type { get; set; }
        public EventMode Mode { get; set; }
        public float[][] Number { get; set; }
        public int[][] Start { get; set; }
        public int[][] Duration { get; set; }

        public PieceEvent(
            PieceEventType type,
            float[][] number,
            int[][] start,
            int[][] duration,
            EventMode mode = EventMode.Add)
        {
            Type = type;
            Mode = mode;
            Number = number;
            Start = start;
            Duration = duration;
        }
    }


    /// <summary>
    /// Extension methods for directly configuring <see cref="PieceConfig"/> events.
    /// </summary>
    public static class PieceConfigExtensions
    {
        /// <summary>
        /// Configures piece events on this <see cref="PieceConfig"/> using the builder.
        /// </summary>
        /// <param name="config">The config to apply events to.</param>
        /// <param name="configure">An action containing event builder calls.</param>
        /// <returns>The original <see cref="PieceConfig"/> instance for method chaining.</returns>
        public static PieceConfig ConfigureEvents(this PieceConfig config, Action<PieceEventBuilder> configure)
        {
            var builder = new PieceEventBuilder();
            configure(builder);
            builder.ApplyTo(config);
            return config;
        }
    }

    /// <summary>
    /// Builder class for constructing and applying <see cref="PieceEvent"/>s to a <see cref="PieceConfig"/>.
    /// </summary>
    public class PieceEventBuilder
    {
        private readonly List<PieceEvent> _events = new List<PieceEvent>();

        /// <summary>
        /// Gets the current number of events queued in this builder.
        /// </summary>
        public int Count => _events.Count;


        /// <summary>
        /// Adds a preconstructed <see cref="PieceEvent"/>.
        /// </summary>
        public PieceEventBuilder Add(PieceEvent pieceEvent)
        {
            _events.Add(pieceEvent);
            return this;
        }

        /// <summary>
        /// Adds an event with constant values (no group/way variation)
        /// </summary>
        public PieceEventBuilder Add(
            PieceEventType type,
            float value = 1,
            int startTime = 0,
            int duration = 1,
            EventMode mode = EventMode.Add)
        {
            _events.Add(new PieceEvent(
                type,
                PieceMatrixHelper.Constant(value),
                PieceMatrixHelper.ConstantInt(startTime),
                PieceMatrixHelper.ConstantInt(duration),
                mode
            ));
            return this;
        }

        /// <summary>
        /// Adds an event with custom matrices for start, duration, and value for more complex patterns
        /// </summary>
        public PieceEventBuilder Add(
            PieceEventType type,
            float[][] number,
            int[][] start,
            int[][] duration,
            EventMode mode = EventMode.Add)
        {
            _events.Add(new PieceEvent(type, number, start, duration, mode));
            return this;
        }

        /// <summary>
        /// Adds an event with direct matrix values for more complex patterns
        /// </summary>
        public PieceEventBuilder AddComplex(
            PieceEventType type,
            float valueBase = 1, float valuePerGroup = 0f, float valuePerWay = 0f,
            int startBase = 0, int startPerGroup = 0, int startPerWay = 0,
            int durationBase = 60, int durationPerGroup = 0, int durationPerWay = 0,
            EventMode mode = EventMode.Add)
        {
            _events.Add(new PieceEvent(
                type,
                PieceMatrixHelper.Matrix(valueBase, valuePerGroup, valuePerWay),
                PieceMatrixHelper.MatrixInt(startBase, startPerGroup, startPerWay),
                PieceMatrixHelper.MatrixInt(durationBase, durationPerGroup, durationPerWay),
                mode
            ));
            return this;
        }

        /// <summary>
        /// Clears all queued events.
        /// </summary>
        public PieceEventBuilder Clear()
        {
            _events.Clear();
            return this;
        }

        /// <summary>
        /// Applies all configured events directly to the specified <see cref="PieceConfig"/>.
        /// </summary>
        public void ApplyTo(PieceConfig config)
        {
            if (_events.Count == 0)
            {
                config.EvStart = Array.Empty<int[][]>();
                config.EvDuration = Array.Empty<int[][]>();
                config.EvNumber = Array.Empty<float[][]>();
                config.EvType = Array.Empty<int[]>();
                return;
            }

            int count = _events.Count;

            config.EvStart = new int[count][][];
            config.EvDuration = new int[count][][];
            config.EvNumber = new float[count][][];
            config.EvType = new int[count][];

            for (int i = 0; i < count; i++)
            {
                PieceEvent evt = _events[i];

                config.EvStart[i] = evt.Start;
                config.EvDuration[i] = evt.Duration;
                config.EvNumber[i] = evt.Number;

                config.EvType[i] = (evt.Mode == EventMode.Add)
                    ? new[] { (int)evt.Type }
                    : new[] { (int)evt.Type, (int)evt.Mode };
            }
        }

        /// <summary>
        /// Apply events from a builder to a config.
        /// </summary>
        /// <seealso cref="ApplyTo(PieceConfig)"/>
        public static void ApplyEvents(PieceConfig config, PieceEventBuilder builder)
        {
            builder?.ApplyTo(config);
        }

        #region Common Events

        /// <summary>
        /// Adds a speed modification event.
        /// </summary>
        public PieceEventBuilder AddSpeed(float speed, int startTime = 0, int duration = 1, EventMode mode = EventMode.Add)
            => Add(PieceEventType.Speed, speed, startTime, duration, mode);

        /// <summary>
        /// Adds an angle/direction modification event.
        /// </summary>
        public PieceEventBuilder AddAngle(float angle, int startTime = 0, int duration = 1, EventMode mode = EventMode.Add)
            => Add(PieceEventType.Angle, angle, startTime, duration, mode);

        /// <summary>
        /// Uniformly scales the bullet's size.
        /// </summary>
        public PieceEventBuilder AddScale(float scale, int startTime = 0, int duration = 1, EventMode mode = EventMode.Add)
            => Add(PieceEventType.ScaleUniform, scale, startTime, duration, mode);

        /// <summary>
        /// Adds a homing event that turns or snaps toward the target.
        /// </summary>
        /// <param name="startTime">Frame when homing begins.</param>
        /// <param name="duration">Duration of the homing effect.</param>
        /// <param name="turnRate">Turn speed. Must be 0 if <paramref name="snapToTarget"/> is true.</param>
        /// <param name="snapToTarget">If true and <paramref name="turnRate"/> is 0, instantly snaps angle to target.</param>
        public PieceEventBuilder AddHoming(int startTime, int duration = 1, float turnRate = 0f, bool snapToTarget = false)
        {
            EventMode mode = (turnRate == 0f && snapToTarget) ? EventMode.Transition : EventMode.Add;
            return Add(PieceEventType.Homing, turnRate, startTime, duration, mode);
        }

        /// <summary>
        /// Adds a bounce event off screen boundaries.
        /// </summary>
        /// <param name="startTime">Frame when bounce behavior activates.</param>
        /// <param name="bounceCount">
        /// <para>BROKEN. How many times the bullets will bounce. Does not currently work properly due to a bug in the game.</para>
        /// Technically can work for cardinal if it's high enough but you'll have to test it since it can be inconsistent.
        /// Keep at 0 otherwise.
        /// </param>
        /// <param name="useCardinalDirections">If true, bounces to cardinal directions instead of reflection.</param>
        /// <param name="aimAtTargetOnBounce">If true, aims at the target on the bullet's last bounce.</param>
        public PieceEventBuilder AddBounce(
            int startTime,
            int bounceCount = 0,
            bool useCardinalDirections = false,
            bool aimAtTargetOnBounce = false)
        {
            PieceEventType type = useCardinalDirections
                ? PieceEventType.BounceCardinal
                : PieceEventType.BounceReflect;

            EventMode mode = aimAtTargetOnBounce ? EventMode.Transition : EventMode.Add;

            return Add(type, bounceCount, startTime, duration: 120, mode);
        }
        #endregion
    }
}
