using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        // cached References
        CanvasGroup canvasGroup;
        Coroutine currentlyActiveFade = null;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;            
        }

        //the currentlyActiveFade keeps track if a fadein or fadeout coroutine is still running
        //if calling the fadeIn coroutine it e.g. first stops the fadeout coroutine
        //this to ensure both can't be fighting over the aplha setting which may never end.
        public Coroutine FadeOut(float fadeTime)
        {
            return Fade(1f, fadeTime);   
        }

        public Coroutine FadeIn(float fadeTime)
        {
            return Fade(0f, fadeTime);
        }

        private IEnumerator FadeRoutine(float target, float fadeTime)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.unscaledDeltaTime / fadeTime);
                yield return null; //waits for every frame
            }
        }

        public Coroutine Fade(float target, float fadeTime)
        {
            if (currentlyActiveFade != null)
            {
                StopCoroutine(currentlyActiveFade);
            }
            currentlyActiveFade = StartCoroutine(FadeRoutine(target, fadeTime));
            return currentlyActiveFade;
        }

        public void FadeOutImmediate()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1;
        }
    }
}
