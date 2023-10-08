using LBoL.ConfigData;
using LBoL.EntityLib.Exhibits.Shining;
using LBoLEntitySideloader.Entities.ConfigBuilders.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LBoLEntitySideloader.Entities.ConfigBuilders.Piece.PieceConverter;

namespace LBoLEntitySideloader.Entities.ConfigBuilders.Piece
{
    [Serializable]
    public class PieceReadableConfig : ReadableConfig<PieceConfig>
    {
        public int Id;
        public ProjectileType projectileType = ProjectileType.Bullet;
        public string projectile = "";
        public ShootType shootType = ShootType.One;
        public int parentPiece = 0;
        public bool addParentAngle = false;
        public bool lastWave = true;
        public int followPiece = 0;
        public int shootEnd = 0;
        public int hitAmount = 1;
        public int laserHitInterval = 6;
        public bool zeroHitNotDie = false;
        public float scale = 1f;
        public PieceColor color = new PieceColor();
        public RootType rootType = RootType.Zero;
        public D2ArrayEncode<float> X = new D2ArrayEncode<float>();
        public D2ArrayEncode<float> Y = new D2ArrayEncode<float>();
        public D2ArrayEncode<float> radius = new D2ArrayEncode<float>();
        public D2ArrayEncode<float> radiusA = new D2ArrayEncode<float>();
        public Aim aim = Aim.FirstGroupOnly;
        public int startTime = 0;
        public int gInterval = 0;
        public int group = 1;
        public Way way = new Way();
        public D2ArrayEncode<float> gAngle = new D2ArrayEncode<float>();
        public D2ArrayEncode<float> range = new D2ArrayEncode<float>();
        public int life = 300;
        public int laserLastWave = 0;
        public D2ArrayEncode<float> startSpeed = new D2ArrayEncode<float>(10f); 
        //new D2ArrayEncode<float>() { mode = D2ArrayEncode<float>.Mode.Direct, baseValue = new NumberOption<float>() { mode = NumberOption<float>.Mode.Literal, value = 10f} };
        public D2ArrayEncode<float>  startAcc = new D2ArrayEncode<float>();
        public D2ArrayEncode<float> startAccAngle = new D2ArrayEncode<float>();

        public BulletEvents bulletEvents = new BulletEvents();

        public Vector3 vanishV3 = new Vector3(0.08f, 0.08f, 0.08f);
        public string launchSfx= "";
        public string hitBodySfx= "";
        public float hitAnimationSpeed = 1f;

        public PieceReadableConfig() { }

        public override PieceConfig ConvertSelf() 
        {
            var builder = new PieceBuilder();
            return builder.BuildConfig(this);
        }
    }
}
