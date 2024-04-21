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
        WaveManager.Instance.EnterBossStage += n => Shake(6);
    }

    public void Shake(float duration)
    {
        StartCoroutine(ShakeBG(duration));
    }

    IEnumerator ShakeBG(float duration)
    {
        _animator.SetBool("IsShaking", true);
        yield return new WaitForSeconds(duration);

        _animator.SetBool("IsShaking", false);
    }
}
