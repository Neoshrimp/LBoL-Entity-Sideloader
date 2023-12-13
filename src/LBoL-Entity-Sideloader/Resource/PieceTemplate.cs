using LBoL.ConfigData;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

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
        /// Id : ,
        /// Type : ,
        /// Projectile : ,
        /// ShootType : 0/1/2/3,
        /// ParentPiece : ,
        /// AddParentAngle : ,
        /// LastWave : ,
        /// FollowPiece : ,
        /// ShootEnd : ,
        /// HitAmount : ,
        /// HitInterval : ,
        /// ZeroHitNotDie : ,
        /// Scale : ,
        /// Color : ,
        /// RootType : ,
        /// X : ,
        /// Y : ,
        /// Radius : ,
        /// RadiusA : ,
        /// Aim : ,
        /// StartTime : ,
        /// GInterval : ,
        /// Group : ,
        /// Way : ,
        /// GAngle : ,
        /// Range : ,
        /// Life : ,
        /// LaserLastWave : ,
        /// StartSpeed : ,
        /// StartAcc : ,
        /// StartAccAngle : ,
        /// EvStart : ,
        /// EvDuration : ,
        /// EvNumber : ,
        /// EvType : ,
        /// VanishV3 : ,
        /// LaunchSfx : ,
        /// HitBodySfx : ,
        /// HitAnimationSpeed : 
        /// </summary>
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
                    Scale : 1,
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
                    Life : 300,
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
