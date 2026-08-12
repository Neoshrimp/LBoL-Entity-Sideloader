using LBoL.ConfigData;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using LBoLEntitySideloader.ExtraFunc.GunHelpers;

namespace LBoLEntitySideloader.Resource
{
    public abstract class PieceTemplate : EntityDefinition,
        IConfigProvider<PieceConfig>
    {
        public override Type ConfigType() => typeof(PieceConfig);
        public override Type EntityType() => throw new InvalidDataException();
        public override Type TemplateType() => typeof(PieceTemplate);


        static public int ConvertGunId(int gunId, int pieceNumber = 0)
        {
            if(pieceNumber < 0 || pieceNumber > 99)
            {
                throw new ArgumentException($"Exception while registering piece for GunId {gunId}: {pieceNumber} is out of range 0-99");
            }
            return gunId * 100 + pieceNumber;
        }

        /// <summary>
        /// 2d arrays have a maximum 4x2 most of the time. (Up to 4 subarrays, each up to 2 values).  <see cref="PieceMatrixHelper"/>.
        /// Id : Unique id. Must equal gun ID * 100; last two digits being its index within the gun,
        /// Type : false for normal bullet; true for laser,
        /// Projectile : name of the projectile; see readable bullet or laser configs,
        /// ShootType : 0/1/2/3. Determines how the bullet spawns in relation to its parent piece,
        /// ParentPiece : index of parent piece. only used with shoot type 2 and 3,
        /// AddParentAngle : adds the parent's current angle when spawning,
        /// LastWave : damage update only triggers when this piece hits the enemy,
        /// FollowPiece : id of an earlier piece; copies the pos and angle of bullets with the same group and way indices,
        /// ShootEnd : frames until the player's shoot animation stops,
        /// HitAmount : must be over 1. How many times a bullet can hit an enemy before dying,
        /// HitInterval : only for lasers. Time between each hit,
        /// ZeroHitNotDie : bullet will not die when its hit amount reaches 0,
        /// Scale : Size of bullets,
        /// Color : Color of bullets. See <see cref="PieceColorHelper"/>.,
        /// RootType : 0/1/2. Bullet spawns relative to the shooter/target/the world (0,0 is center),
        /// X : Offset from the spawn point,
        /// Y : Offset from the spawn point,
        /// Radius : Spawns bullet at a distance from the spawnpoint,
        /// RadiusA : Changes angle of bullet after Radius,
        /// Aim : 0 for aimed bullets, 1 for unaimed bullets.,
        /// StartTime : Frames till piece starts,
        /// GInterval : Interval between each group,
        /// Group : how many times bullets are spawns,
        /// Way : how many bullets spawned per group,
        /// GAngle : angle of the group,
        /// Range : Spread angle of the bullets for each group,
        /// Life : frame lifetime of bullets,
        /// LaserLastWave : Timer before laser registers its first hit,
        /// StartSpeed : Speed of bullets,
        /// StartAcc : Acceleration (Change in speed) of bullets,
        /// StartAccAngle : Angle acceleration of bullets,
        /// EvStart : Start of events,
        /// EvDuration : Duration of events,
        /// EvNumber : Value for the event,
        /// EvType : Type of event,
        /// VanishV3 : 0.08,
        /// LaunchSfx : SFX when launching bullet,
        /// HitBodySfx : SFX when hitting an enemy,
        /// HitAnimationSpeed : changes speed of enemy animation when they are hit
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
