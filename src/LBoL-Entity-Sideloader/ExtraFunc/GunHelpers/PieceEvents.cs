using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.ExtraFunc.GunHelpers
{
    /// <summary>
    /// Bullet event types that modify bullet behavior over time
    /// </summary>
    public enum PieceEventType
    {
        /// <summary>
        /// Modifies Bullet Speed
        /// </summary>
        Speed = 1,
        /// <summary>
        /// Modifies bullet angle/direction
        /// </summary>
        Angle = 2,
        /// <summary>
        /// Modifies acceleration
        /// </summary>
        Acceleration = 3,
        /// <summary>
        /// Modifies acceleration angle
        /// </summary>
        AccelerationAngle = 4,
        /// <summary>
        /// Aims toward target.
        /// If its event mode is 1 (transition), then it will instantly snap to the target.
        /// Use the special homing method for it.
        /// </summary>
        Homing = 5,
        /// <summary>
        /// Bounces off screen boundaries (mirror reflection). Does not bounce off the wall on the opposite side of the shooter.
        /// If its event mode is 1, it will aim towards the target at reflection instead.
        /// Use special bouncing method for it.
        /// </summary>
        BounceReflect = 9,
        /// <summary>
        /// Bounces to cardinal directions at boundaries (hitting the bottom changes its angle straight up). Does not bounce off the wall on the opposite side of the shooter.
        /// If its event mode is 1, it will aim towards the target at reflection.
        /// Use special bouncing method for it.
        /// </summary>
        BounceCardinal = 10,
        /// <summary>
        /// Directly changes X position
        /// </summary>
        PositionX = 11,
        /// <summary>
        /// Directly changes Y position
        /// </summary>
        PositionY = 12,
        /// <summary>
        /// Changes both X and Y scale
        /// </summary>
        ScaleUniform = 13,
        /// <summary>
        /// Changes Y scale only
        /// </summary>
        ScaleY = 14,
        /// <summary>
        /// Changes X scale only
        /// </summary>
        ScaleX = 15,
        /// <summary>
        /// Moves forward in bullet's direction
        /// </summary>
        MoveForward = 16,
        /// <summary>
        /// Moves perpendicular to bullet's direction
        /// </summary>
        MovePerpendicular = 17,
        /// <summary>
        /// Moves forward in AccAngle direction.
        /// Used for bullets that want to have a different direction to its sprite compared to its actual movement direction.
        /// For example, spinning objects.
        /// </summary>
        MoveAccAngleForward = 18,
        /// <summary>
        /// Moves perpendicular to AccAngle direction
        /// </summary>
        MoveAccAnglePerpendicular = 19,
        /// <summary>
        /// Custom sine-based movement. Ignores duration.
        /// </summary>
        Huali = 99
    }

    /// <summary>
    /// Event calculation mode (affects how EventNumber is applied)
    /// </summary>
    public enum EventMode
    {
        /// <summary>
        /// Add EventNumber directly over duration
        /// </summary>
        Add = 0,
        /// <summary>
        /// Transition from current value to EventNumber
        /// </summary>
        Transition = 1,
        /// <summary>
        /// Multiply current value by EventNumber over duration
        /// </summary>
        Multiply = 2
    }
}
