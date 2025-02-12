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

    [SerializeField] private Image CountImg;
    [SerializeField] private List<Sprite> MultiplyCount2Ds = new List<Sprite>();

    private enum DeathState
    {
        DyingPhaseOne,
        DyingPhaseTwo
    }

    private DeathState m_deathState;

    protected override void Update()
    {
        if (!isDead)
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
        isDead = true;
    }

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

    private IEnumerator IDyingPhaseOneCoroutine() // Need to apply multiply text
    {
        ShadowGO.gameObject.SetActive(false);
        fish_2D.enabled = false;
        fireBallImg.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        BGShaker.Instance.Shake(1.5f);
        CountImg.gameObject.SetActive(true);
        CountImg.transform.rotation = new Quaternion(0, 0, 0, 1);

        CountImg.sprite = MultiplyCount2Ds[0];
        SmallDragonManager.Instance.SpawnSmallDragon_Left();
        SmallDragonManager.Instance.SpawnSmallDragon_Right();
        FishesInstantDie();
        yield return new WaitForSeconds(2.2f);

        BGShaker.Instance.Shake(1.5f);
        CountImg.sprite = MultiplyCount2Ds[1];
        SmallDragonManager.Instance.SpawnSecSmallDragon_Left();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Right();
        FishesInstantDie();
        yield return new WaitForSeconds(2.1f);

        BGShaker.Instance.Shake(1.5f);
        CountImg.sprite = MultiplyCount2Ds[2];
        SmallDragonManager.Instance.SpawnSecSmallDragon_Top();
        SmallDragonManager.Instance.SpawnSmallDragon_Bot();
        FishesInstantDie();
        yield return new WaitForSeconds(1.9f);

        BGShaker.Instance.Shake(1.5f);
        CountImg.sprite = MultiplyCount2Ds[3];
        SmallDragonManager.Instance.SpawnSecSmallDragon_Left();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Right();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Top();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Bot();
        FishesInstantDie();
        yield return new WaitForSeconds(1.8f);

        BGShaker.Instance.Shake(1.5f);
        CountImg.sprite = MultiplyCount2Ds[4];
        ExplosionEffectManager.Instance.ShowExplosionEffects();
        SmallDragonManager.Instance.SpawnSmallDragon_Left();
        SmallDragonManager.Instance.SpawnSmallDragon_Right();
        SmallDragonManager.Instance.SpawnSecSmallDragon_Top();
        SmallDragonManager.Instance.SpawnSmallDragon_Bot();
        FishesInstantDie();

        yield return new WaitForSeconds(1.7f);

        BGShaker.Instance.Shake(3f);
        CountImg.gameObject.SetActive(false);
        fireBallImg.gameObject.SetActive(false);
        //multiply Text

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
