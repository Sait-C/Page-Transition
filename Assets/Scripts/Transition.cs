using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    ///<summary>
    ///This script should be placed on all pages that transition
    /// </summary>
    
    public enum OutTransitionType { Fade, FadeInstant, Flicker }; //OutTransitionType is going to be determining how the currentPage is going to be leaving the screen 
    public OutTransitionType outTransitionType;
    public enum InTransitionType { Fade, FadeInstant, Flicker }; //this is going to be determining how to new page is it going to enter the screen 
    public InTransitionType inTransitionType;
    public string parentTag; //so remember whenever we spawn these objects when we spawn these pages we need to be sure that they're getting set to be child of the correct object so whenever our page first comes into the screen they are going to have no parent and so they won't be child of a canvas and so you won't be able to see them so we use this parent tag to be able to set the pages to be child of the canvas
    public Vector3 spawnPosition = Vector3.zero;
    public float fadeSpeed = 2f;
    public float flickerRate = 0.025f;

    //variables to handle bringing the new page in
    bool transitionInitialized = false;
    bool startTransition = false;
    float inColorAlpha = 0;
    float outColorAlpha = 0;
    Text[] transitionTxts, txts; //transition texts and current page texts
    Image[] transitionImgs, imgs; //transition images and current page images
    RectTransform transitionPage;
    RectTransform thisPage;

    //INITIALIZE
    private void Start()
    {
        //if our transition is fade(fade out), we need our alpha to be fully opaque
        if(outTransitionType == OutTransitionType.Fade) {
            outColorAlpha = 1;
        }
        thisPage = GetComponent<RectTransform>();
        //hold images and texts that are going to be faded
        imgs = GetComponentsInChildren<Image>();
        txts = GetComponentsInChildren<Text>();
    }

    public GameObject InitializeTransitionPage(GameObject transition)
    {
        //set the transition page
        GameObject go = Instantiate(transition as GameObject);
        transitionPage = go.GetComponent<RectTransform>();
        //the transition page parent needs to be the canvas (or one of its children)
        transitionPage.transform.SetParent(GameObject.FindGameObjectWithTag(parentTag).transform);
        transitionPage.transform.localScale = Vector3.one;
        //fill the text and image arrays with components that need to be faded
        transitionTxts = transitionPage.GetComponentsInChildren<Text>();
        transitionImgs = transitionPage.GetComponentsInChildren<Image>();

        //start the page off at transparent
        foreach (Text text in transitionTxts){
            text.color = new Vector4(text.color.r, text.color.g, text.color.b, 0);
        }
        foreach (Image image in transitionImgs){
            image.color = new Vector4(image.color.r, image.color.g, image.color.b, 0);
        }

        transitionInitialized = true;

        return transitionPage.gameObject;
    }

    //END INITIALIZE

    //called from menu controller
    public void StartTransition()
    {
        startTransition = true;
    }

    //UPDATING
    private void Update()
    {
        if (startTransition)
        {
            switch (outTransitionType) //page leaving
            {
                case OutTransitionType.Fade:
                    FadePageOut();
                    break;
                case OutTransitionType.FadeInstant:
                    outColorAlpha = 0;
                    break;
                case OutTransitionType.Flicker:
                    StartCoroutine("FlickerOut", flickerRate);
                    break;
                default:
                    break;
            }
            switch (inTransitionType) //new page coming in
            {
                case InTransitionType.Fade:
                    FadePageIn();
                    break;
                case InTransitionType.FadeInstant:
                    inColorAlpha = 1;
                    break;
                case InTransitionType.Flicker:
                    StartCoroutine("FlickerIn", flickerRate);
                    break;
                default:
                    break;
            }

            UpdateTransitionPageColors();
            UpdateCurrentPageColors();
        }
    }

    //END UPDATING

    //OUT TRANSITION FUNCTIONS
    void FadePageOut()
    {
        outColorAlpha = Mathf.Lerp(outColorAlpha, 0, fadeSpeed * Time.deltaTime);
    }

    IEnumerator FlickerOut(float frequency)
    {
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(frequency);
            outColorAlpha = 0.35f;
            yield return new WaitForSeconds(frequency);
            outColorAlpha = 0.8f;
        }
    }
    //END OUT TRANSITION FUNCTIONS

    //IN TRANSITION FUNCTIONS
    void FadePageIn()
    {
        inColorAlpha = Mathf.Lerp(inColorAlpha, 1, fadeSpeed * Time.deltaTime);
        if (inColorAlpha > 0.99f)
            inColorAlpha = 1; //object wont be destroyed until alpha is 1

        if(inColorAlpha == 1)
        {
            //page is loaded in, so we can destroyed this
            Destroy(gameObject);
        }
    }

    IEnumerator FlickerIn(float frequency)
    {
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(frequency);
            inColorAlpha = 0.35f;
            yield return new WaitForSeconds(frequency);
            inColorAlpha = 1.0f;
        }

        if (inColorAlpha == 1.0f)
            Destroy(gameObject);
    }

    //END IN TRANSITION FUNCTIONS

    //HELPER METHODS
    void UpdateTransitionPageColors()
    {
        if(transitionImgs != null)
        {
            foreach (Image image in transitionImgs){
                image.color = new Vector4(image.color.r, image.color.g, image.color.b, inColorAlpha); ;
            }
        }
        if (transitionTxts!= null)
        {
            foreach (Text text in transitionTxts)
            {
                text.color = new Vector4(text.color.r, text.color.g, text.color.b, inColorAlpha); ;
            }
        }
    }

    void UpdateCurrentPageColors()
    {
        if (imgs != null)
        {
            foreach (Image image in imgs)
            {
                image.color = new Vector4(image.color.r, image.color.g, image.color.b, outColorAlpha); ;
            }
        }
        if (txts != null)
        {
            foreach (Text text in txts)
            {
                text.color = new Vector4(text.color.r, text.color.g, text.color.b, outColorAlpha); ;
            }
        }
    }
}
