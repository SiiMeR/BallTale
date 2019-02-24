using System.Collections;
using TMPro;
using UnityEngine;

namespace Extensions
{
    public static class TextMeshProExtensions
    {
        public static IEnumerator TransitionColorTo(this TextMeshProUGUI text, float transitionTime, Color finalColor)
        {
            var timer = 0f;
            var startColor = text.color;
            while ((timer += Time.deltaTime) < transitionTime)
            {
                text.color = Color.Lerp(startColor, finalColor, timer / transitionTime);
                yield return null;
            }

            text.color = finalColor;
        }
    }
}