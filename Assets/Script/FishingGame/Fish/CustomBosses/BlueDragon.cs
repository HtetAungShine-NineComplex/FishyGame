using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueDragon : Fish
{
    private bool isDead = false;
    private Vector3 targetPos;
    private float elapsedTime;
    private float lerpDuration = 50f;

    [SerializeField] private Image fireBallImg;
    [SerializeField] private GameObject ShadowGO;
    [SerializeField] private Text MultipleTxt;
       
    private enum DeathState
    {
        DyingPhaseOne,
        DyingPhaseTwo
    }

    private DeathState deathState;

    protected override void Update()
    {
        if (!isDead)
        {
            base.Update();
        }
        else if (deathState == DeathState.DyingPhaseOne)
        {
            LerpToMiddle();
        }
    }
    public override void OnDead()
    {
        deathState = DeathState.DyingPhaseOne;
        HandleDeathState();
        base.OnDead();
        isDead = true;
    }

    private void HandleDeathState()
    {
        switch (deathState)
        {
            case DeathState.DyingPhaseOne:
                StartCoroutine(DyingPhaseOneCoroutine());
                break;
            default: 
                break;

        }
    }

    private IEnumerator DyingPhaseOneCoroutine() // Need to apply multiply text
    {
        ShadowGO.gameObject.SetActive(false);
        fish_2D.enabled = false;
        fireBallImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.2f);
;
        SmallDragonManager.Instance.SpawnSmallDragon_Left();
        SmallDragonManager.Instance.SpawnSmallDragon_Right();
        yield return new WaitForSeconds(2.2f);

        SmallDragonManager.Instance.SpawnSecSmallDragon_Left();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Right();
        yield return new WaitForSeconds(2.1f);

        SmallDragonManager.Instance.SpawnSmallDragon_Left();
        SmallDragonManager.Instance.SpawnSmallDragon_Right();
        yield return new WaitForSeconds(2.0f);

        SmallDragonManager.Instance.SpawnSecSmallDragon_Top();
        SmallDragonManager.Instance.SpawnSmallDragon_Bot();
        yield return new WaitForSeconds(1.9f);

        SmallDragonManager.Instance.SpawnSecSmallDragon_Left();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Right();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Top();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Bot();
        yield return new WaitForSeconds(1.8f);

        SmallDragonManager.Instance.SpawnSmallDragon_Left();
        SmallDragonManager.Instance.SpawnSmallDragon_Right();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Top();
        SmallDragonManager.Instance.SpawnSmallDragon_Bot();
        yield return new WaitForSeconds(1.7f);

/*        SmallDragonManager.Instance.SpawnSecSmallDragon_Left();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Right();
        SmallDragonManager.Instance.SpawnSmallDragon_Top();
        SmallDragonManager.Instance.SpawnSmallDragon_Bot();
        yield return new WaitForSeconds(2.2f);*/
        fireBallImg.gameObject.SetActive(false);
        //multiply Text

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