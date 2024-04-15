using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffects : MonoBehaviour
{
    public void ShowFireEffects()
    {
        StartCoroutine(FireEffect());
    }

    IEnumerator FireEffect()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        gameObject.SetActive(true);
    }
}
