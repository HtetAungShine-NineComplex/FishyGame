using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Phoenix : Fish
{
    private Vector3 m_targetPos;
    private float m_elapsedTime;
    private float m_lerpDuration = 50f;
    private bool m_isDead = false;

    [SerializeField] private GameObject shadowGO;
    [SerializeField] private Image featherImg;
    [SerializeField] private Image hitEffectimg;

    [SerializeField] private Image fireEffectImg;
    [SerializeField] private AnimationClip fireEffectAnim;

    [SerializeField] private AnimationClip featherAnim;

    protected override void Update()
    {
        if (!m_isDead)
        {
            base.Update();
        }
        else if (m_deathState == DeathState.DyingPhaseOne)
        {
            LerpToMiddle();
        }
    }
    public override void OnDead()
    {
        
        m_deathState = DeathState.DyingPhaseOne;
        handleDeathState();
        base.OnDead();
        m_isDead = true;
    }
    private enum DeathState
    {
        DyingPhaseOne,
        DyingPhaswTwo
    }
    private DeathState m_deathState;
    private void handleDeathState()
    {
        switch (m_deathState)
        {
            case DeathState.DyingPhaseOne:
                StartCoroutine(IDyingPhaseOneCoroutine());
                break;
            default:
                break;
        }
    }
    private IEnumerator IDyingPhaseOneCoroutine()
    {
        fish_2D.enabled = false;
        shadowGO.gameObject.SetActive(false);       
        featherImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(featherAnim.length*3.5f);

        featherImg.gameObject.SetActive(false);
        fireEffectImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(fireEffectAnim.length);

        fireEffectImg.gameObject.SetActive(false);
        Backgroundmanager.Instance.FireEfectBG.SetActive(true);
        SmallPhoenixManager.Instance.fishesInstantDie();
        yield return new WaitForSeconds(fireEffectAnim.length / 2);

        Backgroundmanager.Instance.Phoenix_HitEffect.SetActive(true);
        Backgroundmanager.Instance.BurningBorderBG.SetActive(true);
        yield return new WaitForSeconds(fireEffectAnim.length + 0.4f);

        Backgroundmanager.Instance.FireEfectBG.SetActive(false);
        Backgroundmanager.Instance.TextCountEffect.SetActive(true);
        SmallPhoenixManager.Instance.SpawnSmallPhoenixGroup();
        SmallPhoenixManager.Instance.FishesInstantDieRepeately();
        yield return new WaitForSeconds(17f);

        Backgroundmanager.Instance.BurningBorderBG.SetActive(false);
        Backgroundmanager.Instance.Phoenix_HitEffect.SetActive(false);
        Backgroundmanager.Instance.TextCountEffect.SetActive(false);


        //yield return new WaitForSeconds(15f);


    }
    private void LerpToMiddle()
    {
        m_targetPos = new Vector3(0, 35f, 0);
        if (m_elapsedTime < m_lerpDuration)
        {
            m_elapsedTime += Time.deltaTime;
            float t = m_elapsedTime / m_lerpDuration;
            RectTransform parentTransform = _move.gameObject.GetComponent<RectTransform>();
            parentTransform.anchoredPosition = Vector3.Lerp(parentTransform.anchoredPosition, m_targetPos, t);
        }
    }
}
