using LBoL.Presentation.Effect;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    /// <summary>
    /// Class for creating gameObject with EffectWidget
    /// </summary>
    public class EffectWidgetData
    {
        public GameObject effectGo;

        static ExtraElementProperties defaultProps = new ExtraElementProperties()
        {
            changeColor = false,
            dieType = EffectWidget.DieType.Inactivate,
            lowPerformance = false
        };

        public EffectWidgetData() { }
        /// <summary>
        /// See EffectWidgetData.CreateEffect
        /// </summary>
        /// <param name="effectGo"></param>
        /// <param name="particleSystemProperties"></param>
        /// <param name="trailRendererProperties"></param>
        /// <param name="sortingLayer"></param>
        /// <param name="nameOverwrite"></param>
        public EffectWidgetData(GameObject effectGo, Queue<ExtraElementProperties> particleSystemProperties, Queue<ExtraElementProperties> trailRendererProperties = null, SortingLayer sortingLayer = SortingLayer.Bullet, string nameOverwrite = "") 
        {
            this.effectGo = EffectWidgetData.CreateEffect(effectGo, particleSystemProperties, trailRendererProperties, sortingLayer, nameOverwrite);
        }

        /// <summary>
        /// Adds EffectWidget Monobehaviour to effectGo.
        /// effectGo should be a prefab loaded via AssetBundle with all visual Particle/Trail systems in its children gameObjects.
        /// Additional minor properties can be provided to Particle/Trail systems via queue arguments.
        /// First element in the queue will go to the first system discovered (the top most in the hierarchy), second element to the second discovered and so on.
        /// When queue has only one element remaining, the properties of the element will be applied to any systems discovered afterwards. This way a queue with single element can be provided to apply the same properties to all the systems.
        /// If queue is null defaultProps will be used.
        /// </summary>
        /// <param name="effectGo"></param>
        /// <param name="particleSystemProperties"></param>
        /// <param name="trailRendererProperties"></param>
        /// <param name="sortingLayer"></param>
        /// <param name="nameOverwrite"></param>
        /// <returns></returns>
        public static GameObject CreateEffect(GameObject effectGo, Queue<ExtraElementProperties> particleSystemProperties, Queue<ExtraElementProperties> trailRendererProperties = null, SortingLayer sortingLayer = SortingLayer.Bullet, string nameOverwrite = "")
        {

            if(!string.IsNullOrEmpty(nameOverwrite))
                effectGo.name = nameOverwrite;

            int layer = 0;
            switch (sortingLayer)
            {
                case SortingLayer.Bullet:
                    layer = 10;
                    break;
                case SortingLayer.Effect:
                    layer = 11;
                    break;
                default:
                    break;
            }


            effectGo.transform.position = Vector3.zero;
            var ew = effectGo.AddComponent<EffectWidget>();


            var particleSystemElements = new List<EffectWidget.ParticleSystemElement>();
            var trailRendererElements = new List<EffectWidget.TrailRendererElement>();


            foreach (var c in effectGo.transform.IterateHierarchy())
            {
                c.layer = layer;

                if (c.TryGetComponent<ParticleSystem>(out var particleSystem))
                {
                    var props = WrapQ(particleSystemProperties);
                    particleSystemElements.Add(new EffectWidget.ParticleSystemElement() { particleSystem = particleSystem, changeColor = props.changeColor, dieType = props.dieType, lowPerformance = props.lowPerformance });
                }

                if (c.TryGetComponent<ParticleSystemRenderer>(out var particleSystemRenderer))
                {
                    particleSystemRenderer.sortingLayerName = sortingLayer.ToString();
                }

                if (c.TryGetComponent<TrailRenderer>(out var trailRenderer))
                {
                    trailRenderer.sortingLayerName = sortingLayer.ToString();
                    var props = WrapQ(trailRendererProperties);
                    trailRendererElements.Add(new EffectWidget.TrailRendererElement() { trailRenderer = trailRenderer, changeColor = props.changeColor, dieType = props.dieType, lowPerformance = props.lowPerformance });
                }

            }

            ew.particleSystemElements = particleSystemElements.ToArray();
            ew.trailRendererElements = trailRendererElements.ToArray();
            // init colors and shit
            ew.Awake();

            return effectGo;
        }

        private static ExtraElementProperties WrapQ(Queue<ExtraElementProperties> queue)
        {
            if (queue == null)
                return defaultProps;
            if (queue.Count == 1)
                return queue.Last();
            return queue.Dequeue();
        }

        public class ExtraElementProperties
        {
            public bool changeColor;
            public EffectWidget.DieType dieType;
            public bool lowPerformance;
        }



        public enum SortingLayer
        {
            Bullet,
            Effect
        }
    }
}