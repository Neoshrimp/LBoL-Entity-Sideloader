using Cysharp.Threading.Tasks;
using LBoL.Presentation.Units;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    public class ModelOption
    {
        public readonly UnitView.ModelType modelType;

        public UniTask<Sprite> loadSprite;

        /// <summary>
        /// Spine must have "idle" animation. Can also have "blink", "pose1" and "fly" animations. Also untested
        /// </summary>
        public UniTask<SkeletonDataAsset> loadSpine;

        // not implemented yet
        public IdContainer effectId;

        public ModelOption(UniTask<Sprite> loadSprite)
        {
            this.modelType = UnitView.ModelType.SingleSprite;
            this.loadSprite = loadSprite;
        }

        public ModelOption(UniTask<SkeletonDataAsset> loadSpine)
        {
            this.modelType = UnitView.ModelType.Spine;
            this.loadSpine = loadSpine;
        }

        public ModelOption(IdContainer effectId)
        {
            throw new NotImplementedException();
            this.modelType = UnitView.ModelType.Effect;
            this.effectId = effectId;
        }
    }
}
