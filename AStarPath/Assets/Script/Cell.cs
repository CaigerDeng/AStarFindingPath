using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerDownHandler
{
    private Map map;
    private Image img;

    void Start()
    {
        map = transform.parent.GetComponent<Map>();
        img = transform.GetComponent<Image>();
    }

    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (transform.tag == "Wall")
        {
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            map.SetStartImage(img);
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            map.SetEndImage(img);
        }
    }



}


