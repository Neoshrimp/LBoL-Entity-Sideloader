using LBoL.ConfigData;
using System;
using System.IO;
using UnityEngine;
using LBoLEntitySideloader.ExtraFunc.GunHelpers;
using LBoLEntitySideloader.Entities;

// This namespace is wrong but can't do anything without breaking a bunch of mods
namespace LBoLEntitySideloader.Resource
{
    public abstract class PieceTemplate : EntityDefinition,
        IConfigProvider<PieceConfig>
    {
        public override Type ConfigType() => typeof(PieceConfig);
        public override Type EntityType() => throw new InvalidDataException();
        public override Type TemplateType() => typeof(PieceTemplate);

        /// <summary>
        /// Generates an id for the piece through giving it the equivalent gun's id and the index you want for the piece.
        /// </summary>
        /// <param name="gunId">Id of the gun you want to connect this to</param>
        /// <param name="pieceNumber">Must be less than 100</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        static public int ConvertGunId(int gunId, int pieceNumber = 0)
        {
            if(pieceNumber < 0 || pieceNumber > 99)
            {
                throw new ArgumentException($"Exception while registering piece for GunId {gunId}: {pieceNumber} is out of range 0-99");
            }
            return gunId * 100 + pieceNumber;
        }

        /// <summary>
        /// <para>
        /// 2d arrays have a maximum 4x2 most of the time. (Up to 4 subarrays, each up to 2 values). See <see cref="PieceMatrixHelper"/>.
        /// </para>
        /// <para>
        /// <b>Identity</b><br/>
        /// <c>Id</c> — unique id; must equal gun ID * 100, with the last two digits
        /// being its index within the gun.<br/>
        /// <c>Type</c> — false for a normal bullet, true for a laser.<br/>
        /// <c>Projectile</c> — name of the projectile; see the readable bullet or
        /// laser configs.
        /// </para>
        /// <para>
        /// <b>Placement</b><br/>
        /// <c>ShootType</c> — 0/1/2/3; determines how the bullet spawns relative to
        /// its parent piece.<br/>
        /// <c>ParentPiece</c> — index of the parent piece; only used with ShootType 2 and 3.<br/>
        /// <c>AddParentAngle</c> — adds the parent's current angle when spawning.<br/>
        /// <c>FollowPiece</c> — id of an earlier piece; copies position and angle of
        /// bullets with matching group and way indices.<br/>
        /// <c>RootType</c> — 0/1/2; spawn relative to shooter, target, or world
        /// (0,0 is center).<br/>
        /// <c>X</c>, <c>Y</c> — offset from the spawn point.<br/>
        /// <c>Radius</c> — spawn distance from the spawn point.<br/>
        /// <c>RadiusA</c> — changes bullet angle after Radius.<br/>
        /// <c>Aim</c> — 0 for aimed bullets, 1 for unaimed.
        /// </para>
        /// <para>
        /// <b>Timing and grouping</b><br/>
        /// <c>Group</c> — number of times bullets are spawned.<br/>
        /// <c>Way</c> — bullets spawned per group.<br/>
        /// <c>GAngle</c> — angle of the group.<br/>
        /// <c>StartTime</c> — frames until the piece starts.<br/>
        /// <c>GInterval</c> — interval between groups.<br/>
        /// <c>Range</c> — spread angle of bullets within each group.<br/>
        /// <c>Life</c> — frame lifetime of bullets.<br/>
        /// <c>ShootEnd</c> — frames until the player's shoot animation stops.
        /// </para>
        /// <para>
        /// <b>Motion</b><br/>
        /// <c>StartSpeed</c> — bullet speed.<br/>
        /// <c>StartAcc</c> — bullet acceleration (change in speed).<br/>
        /// <c>StartAccAngle</c> — angle acceleration of bullets.
        /// </para>
        /// <para>
        /// <b>Collision</b><br/>
        /// <c>LastWave</c> — damage update only triggers when this piece hits an enemy.<br/>
        /// <c>HitAmount</c> — must >= 1; how many times a bullet can hit
        /// before dying.<br/>
        /// <c>HitInterval</c> — lasers only; time between each hit.<br/>
        /// <c>ZeroHitNotDie</c> — bullet does not die when HitAmount reaches 0.<br/>
        /// <c>LaserLastWave</c> — timer before the laser registers its first hit.
        /// </para>
        /// <para>
        /// <b>Events</b><br/>
        /// <c>EvStart</c> — start of events.<br/>
        /// <c>EvDuration</c> — duration of events.<br/>
        /// <c>EvNumber</c> — value for the event.<br/>
        /// <c>EvType</c> — type of event.<br/>
        /// <c>VanishV3</c> — 0.08.
        /// </para>
        /// <para>
        /// <b>Visuals and audio</b><br/>
        /// <c>Scale</c> — size of bullets.<br/>
        /// <c>Color</c> — color of bullets; see <see cref="PieceColorHelper"/>.<br/>
        /// <c>LaunchSfx</c> — SFX when launching a bullet.<br/>
        /// <c>HitBodySfx</c> — SFX when hitting an enemy.<br/>
        /// <c>HitAnimationSpeed</c> — speed of the enemy's hit animation.
        /// </para>
        /// </summary>
        /// <seealso href="https://docs.google.com/document/d/1GqY8VSSLTyk6j2RIo6P19wc49Jo4wRsbS6zacgGEkDI/edit?tab=t.0"/>
        /// <returns></returns>
        public PieceConfig DefaultConfig()
        {
            var config = new PieceConfig(
                    Id : 0,
                    Type : false,
                    Projectile : "",
                    ShootType : 1,
                    ParentPiece : 0,
                    AddParentAngle : false,
                    LastWave : true,
                    FollowPiece : 0,
                    ShootEnd : 0,
                    HitAmount : 1,
                    HitInterval : 6,
                    ZeroHitNotDie : false,
                    Scale : new float[0][],
                    Color : new int[0][],
                    RootType : 0,
                    X : new float[0][],
                    Y : new float[0][],
                    Radius : new float[0][],
                    RadiusA : new float[0][],
                    Aim : 0,
                    StartTime : 0,
                    GInterval : 0,
                    Group : 1,
                    Way : new int[0][],
                    GAngle : new float[0][],
                    Range : new float[0][],
                    Life : new int[0][],
                    LaserLastWave : 0,
                    StartSpeed : new float[][] { new float[] { 10f } },
                    StartAcc : new float[0][],
                    StartAccAngle : new float[0][],
                    EvStart : new int[0][][],
                    EvDuration : new int[0][][],
                    EvNumber : new float[0][][],
                    EvType : new int[0][],
                    VanishV3 : new Vector3(0.08f, 0.08f, 0.08f),
                    LaunchSfx : "",
                    HitBodySfx : "",
                    HitAnimationSpeed : 1
                );
            return config;
        }

        public abstract PieceConfig MakeConfig();
    }
}
