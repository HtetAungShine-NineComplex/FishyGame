using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LightingOnClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _lightingObj;
    private Button _btn;

    public static List<GameObject> lights = new List<GameObject>();

    private void Awake()
    {
        _btn = GetComponent<Button>();
        lights.Add(_lightingObj);

        foreach (GameObject obj in lights)
        {
            obj.SetActive(false);
        }
    }

    public void ShowLighting()
    {
        foreach (GameObject obj in lights)
        {
            obj.SetActive(false);
        }

        _lightingObj.SetActive(true);
    }

    public void HideLighting()
    {
        _lightingObj.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowLighting();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideLighting();
    }
}
