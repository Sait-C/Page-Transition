using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    ///<summary>
    ///This controller will be responsible for determining which pages are being loaded
    /// </summary>

    //add all pages that can be transitioned to from your menu
    public GameObject[] pages;
    //the string array contains names that are codes to set each page
    public string[] pageNames;

    GameObject currentPage;
    //the enter screen bool is for pause or other in game menus
    bool enterScreen = false;

    public bool EnterScreen
    {
        get { return enterScreen; }
        set { enterScreen = value; }
    }

    private void Start()
    {
        SetCurrentPage(pages[0]);
    }

    //the following method is only required to load the first menu page
    void SetCurrentPage(GameObject page)
    {
        GameObject p = Instantiate(page as GameObject);

        p.transform.SetParent(transform); //we want to make sure that the page that's coming into the screen is a child of the canvas, if it's not a child of the canvas it won't be seen 
        RectTransform rt = p.GetComponent<RectTransform>();
        Transition t = p.GetComponent<Transition>();

        //distance to anchors 
        rt.offsetMax = new Vector2(t.spawnPosition.x, t.spawnPosition.y); //initial position
        rt.offsetMin = new Vector2(t.spawnPosition.x, t.spawnPosition.y);

        p.transform.localScale = Vector3.one; //you're gonnna be able to maintain that scale to what you want it to be 

        currentPage = p;
    }

    //the menu controller listens for click events from certain buttons and runs this method when they are clicked
    public void SetNextPage(string PAGE_CODE)
    {
        for (int i = 0; i < pageNames.Length; i++)
        {
            if (PAGE_CODE == pageNames[i])
            {
                //the pageNames have to be in the same order as pages for this to work
                RevealPageInUI(i);
            }
        }
    }

    void RevealPageInUI(int index)
    {
        Transition t = currentPage.GetComponent<Transition>();
        //start transitioning the currentPage - this will active the "out" transition
        t.StartTransition();
        //set the currentPage to the page that is coming to
        currentPage = t.InitializeTransitionPage(pages[index]);
        //position the page based on the page offset
        RectTransform rt = currentPage.GetComponent<RectTransform>();
        t = currentPage.GetComponent<Transition>();

        rt.offsetMax = new Vector2(t.spawnPosition.x, t.spawnPosition.y); 
        rt.offsetMin = new Vector2(t.spawnPosition.x, t.spawnPosition.y);
    }
}
