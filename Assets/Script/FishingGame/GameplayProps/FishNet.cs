using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishNet : MonoBehaviour
{
    [SerializeField] private float _duration = 0.7f;

    private void Start()
    {
        Destroy(gameObject, _duration);
    }
}
