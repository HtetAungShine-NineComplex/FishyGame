using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffectManager : MonoBehaviour
{
    [SerializeField] GameObject[] fireEffects;

    public static ExplosionEffectManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ShowExplosionEffects()
    {

        StartCoroutine(ShowExplosionEffect());
    }

    IEnumerator ShowExplosionEffect()
    {
        foreach (GameObject fireEffect in fireEffects)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, .3f));

            fireEffect.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        foreach (GameObject fireEffect in fireEffects)
        {

            fireEffect.SetActive(false);
        }
    }
}
