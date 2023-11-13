using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationBooker : MonoBehaviour
{
    public GameObject MainPage;
    public GameObject Map;

    public GameObject[] panels;

    public void page1()
    {
        Debug.Log("Switching to MainPage");
        MainPage.SetActive(true);
        Map.SetActive(false);
       
    }

    public void page2()
    {
        Debug.Log("Switching to Map");
        Map.SetActive(true);
        MainPage.SetActive(false);
       
    }



}

