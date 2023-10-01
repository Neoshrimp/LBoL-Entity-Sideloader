using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    public class EffectData
    {
        public GameObject effectGo;

        public void CreateEffect(GameObject effectGo, RenderLayer bullet, string nameOverwrite = "")
        {

            if(!string.IsNullOrEmpty(nameOverwrite))
                effectGo.name = nameOverwrite;
            /*            foreach (var c in effectGo.transform)
                        {
                            GameObject.Destroy(((Transform)c).gameObject);
                        }*/

            //effectGo.AddComponent<EffectWidget>();
            //var ew = effectGo.GetComponent<EffectWidget>();
            //effectGo.layer = 10; // bullet 11- effect

            /*            var fireP = UiManager.GetPanel<UltimateSkillPanel>()?.fireParticle1;
                        if (fireP != null)
                        {
                            CopyComponent(effectGo, fireP);
                        }*/
        }

        enum RenderLayer
        {
            Bullet,
            Effect
        }
    }
}