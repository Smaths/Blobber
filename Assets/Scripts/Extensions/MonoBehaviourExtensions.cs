using System;
using System.Collections;
using UnityEngine;

namespace Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static void InvokeSafe(this MonoBehaviour behavior, float delayInSeconds, Action method)
        {
            behavior.StartCoroutine(InvokeSafeRoutine(delayInSeconds, method));
        }

        public static void InvokeRepeatingSafe(this MonoBehaviour behavior, float delayInSeconds,
            float repeatRateInSeconds, Action method)
        {
            behavior.StartCoroutine(InvokeSafeRepeatingRoutine(delayInSeconds, repeatRateInSeconds, method));
        }

        private static IEnumerator InvokeSafeRepeatingRoutine(float delayInSeconds, float repeatRateInSeconds,
            Action method)
        {
            yield return new WaitForSeconds(delayInSeconds);

            while (true)
            {
                if (method != null) method.Invoke();
                yield return new WaitForSeconds(repeatRateInSeconds);
            }
        }

        private static IEnumerator InvokeSafeRoutine(float delayInSeconds, Action method)
        {
            yield return new WaitForSeconds(delayInSeconds);
            if (method != null) method.Invoke();
        }
    }
}