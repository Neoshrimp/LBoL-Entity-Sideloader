using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.UIhelpers
{
    public static class Extensions
    {
        // did position change?
        public static void KeepPositionAfterAction(this Transform transform, Action action, bool outputDebug = false)
        {
            var originalPos = transform.position;
            var originalRot = transform.rotation;
            var originalScale = transform.localScale;


            if (outputDebug)
                Debug.Log($"{transform.gameObject.name} before pos :{transform.position}, rot: {transform.rotation}, scale: {transform.localScale}");

            action.Invoke();

            if (outputDebug)
                Debug.Log($"{transform.gameObject.name} after pos :{transform.position}, rot: {transform.rotation}, scale: {transform.localScale}");



            transform.position = originalPos;
            transform.rotation = originalRot;
            transform.localScale = originalScale;

        }
    }
}
