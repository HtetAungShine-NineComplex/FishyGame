using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGShaker : MonoBehaviour
{
    private Animator _animator;

    public static BGShaker Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        WaveManager.Instance.EnterBossStage += n => Shake();
    }

    public void Shake()
    {
        _animator.SetTrigger("Shake");
    }
}
