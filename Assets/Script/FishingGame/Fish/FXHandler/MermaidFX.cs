using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MermaidFX : MonoBehaviour
{
    public static MermaidFX Instance;

    [SerializeField] private GameObject _fx;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void DamageFishes()
    {
        List<FishHealth> fishList = GeneratedFishManager.Instance.GetGeneratedFishList();

        for (int i = 0; i < fishList.Count; i++)
        {
            fishList[i].InstantDie();
        }
    }


    public void PlayFX()
    {
        StartCoroutine(PlayMermaidFX());
    }

    IEnumerator PlayMermaidFX()
    {
        _fx.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        DamageFishes();

        yield return new WaitForSeconds(1f);

        _fx.SetActive(false);
    }
}
