using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LionTurtle : Fish
{
    private enum DeathState
    {
        DyingPhase1,
        DyingPhase2,
        DyingPhase3,
        DyingPhase4
    }

    [SerializeField] private Image shellImg;
    [SerializeField] private Image shellRotateImg;
    [SerializeField] private Image fireEffectImg;
    [SerializeField] private Image shellWithFire;
    [SerializeField] private AnimationClip shellAnimationClip;
    [SerializeField] private AnimationClip fireEffectAnimationClip;
    [SerializeField] private AnimationClip fireEffectBGAnimationClip;

    private DeathState deathState;
    private float lerpDuration = 50f;
    private Vector3 targetPos;
    private float elapsedTime;
    private bool isDead = false;
    private int deathCycleCount = 0;
    private int deathCycleMax = 3;

    protected override void Update()
    {
        if (!isDead)
        {
            base.Update();
        }
        else if (deathState == DeathState.DyingPhase1)
        {
            LerpToMiddle();
        }
    }

    public override void OnDead()
    {
        BGShaker.Instance.Shake(20);
        deathState = DeathState.DyingPhase1;
        HandleDeathState();
        base.OnDead();      
        isDead = true;
    }

    private void HandleDeathState()
    {
        switch (deathState)
        {
            case DeathState.DyingPhase1:
                StartCoroutine(DyingPhase1Coroutine());
                break;
            default:
                break;
        }
    }

    private IEnumerator DyingPhase1Coroutine()
    {
        CanvasInstance.Instance.GetCannonHandler().ActiveByOtherPowerUp();
        fish_2D.enabled = false;
        shellImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        shellImg.gameObject.SetActive(false);
        shellRotateImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(shellAnimationClip.length);
        shellRotateImg.gameObject.SetActive(false);
        StartCoroutine(DyingPhase2Coroutine());
        Debug.Log("Finished Change Image and Rotation Animation");
    }

    private IEnumerator DyingPhase2Coroutine()
    {

        shellWithFire.gameObject.SetActive(true);
        FishesInstantDie();
        yield return new WaitForSeconds(0.5f);
        shellWithFire.gameObject.SetActive(false);
        shellRotateImg.gameObject.SetActive(true);
        shellRotateImg.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 450);
        fireEffectImg.gameObject.SetActive(true);
        FishesInstantDie();
        yield return new WaitForSeconds(fireEffectAnimationClip.length);
        fireEffectImg.gameObject.SetActive(false);
        Backgroundmanager.Instance.FireEfectBG.SetActive(true);
        FishesInstantDie();
        yield return new WaitForSeconds(fireEffectBGAnimationClip.length / 2);
        Backgroundmanager.Instance.BurningBorderBG.SetActive(true);
        FishesInstantDie();
        yield return new WaitForSeconds(fireEffectBGAnimationClip.length + 0.4f);
        Backgroundmanager.Instance.FireEfectBG.SetActive(false);
        shellRotateImg.GetComponent<RectTransform>().sizeDelta = new Vector2(290, 290);
        shellRotateImg.gameObject.SetActive(false);

        for (int i = 0; i < deathCycleMax; i++)
        {
            shellWithFire.gameObject.SetActive(true);
            FishesInstantDie();
            yield return new WaitForSeconds(0.5f);
            shellWithFire.gameObject.SetActive(false);
            shellRotateImg.gameObject.SetActive(true);
            shellRotateImg.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 450);
            fireEffectImg.gameObject.SetActive(true);
            FishesInstantDie();
            yield return new WaitForSeconds(fireEffectAnimationClip.length);
            fireEffectImg.gameObject.SetActive(false);
            Backgroundmanager.Instance.FireEfectBG.SetActive(true);
            FishesInstantDie();
            yield return new WaitForSeconds(fireEffectBGAnimationClip.length / 2);
            Backgroundmanager.Instance.BurningBorderBG.SetActive(true);
            FishesInstantDie();
            yield return new WaitForSeconds(fireEffectBGAnimationClip.length + 0.4f);
            Backgroundmanager.Instance.FireEfectBG.SetActive(false);
            shellRotateImg.GetComponent<RectTransform>().sizeDelta = new Vector2(290, 290);
            shellRotateImg.gameObject.SetActive(false);
            FishesInstantDie();
        }

        shellRotateImg.gameObject.SetActive(false);
        CoinManager.Instance.ShowCoin(GetComponent<RectTransform>().anchoredPosition, CoinSpawnAmount, Score);
        Debug.Log("Finished Fire Particle Animation");
        yield return new WaitForSeconds(5f);

        CanvasInstance.Instance.GetCannonHandler().DisablePowerUp();
        Backgroundmanager.Instance.BurningBorderBG.SetActive(false);
    }

    private static void FishesInstantDie()
    {
        List<FishHealth> fishList = GeneratedFishManager.Instance.GetGeneratedFishList();

        for (int a = 0; a < fishList.Count; a++)
        {
            fishList[a].InstantDie();
        }
    }

    private void LerpToMiddle()
    {
        targetPos = new Vector3(0, 35f, 0);
        if (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lerpDuration;
            RectTransform parentTransform = _move.gameObject.GetComponent<RectTransform>();
            parentTransform.anchoredPosition = Vector3.Lerp(parentTransform.anchoredPosition, targetPos, t);
        }
    }
}