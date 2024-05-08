using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusBG : GameBG
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private Transform _root;

    private Coroutine _rotateCorotine;

    protected override void Start()
    {
        base.Start();

        
    }

    protected override void OnEnterBossStage(int mapIndex)
    {
        base.OnEnterBossStage(mapIndex);
        if (mapIndex == this.mapIndex)
        {
            _animator.SetBool("isBossFight", true);
            _rotateCorotine = StartCoroutine(RotateRootRandomly());
        }
    }

    protected override void OnEnterNormalStage(int mapIndex)
    {
        base.OnEnterNormalStage(mapIndex);
        if (mapIndex != this.mapIndex)
        {
            return;
        }
        gameObject.SetActive(true);
    }

    protected override void OnEnterBonusStage(int mapIndex)
    {
        _animator.SetBool("IsBossFight", false);
        if(_rotateCorotine != null)
        {
            StopCoroutine(_rotateCorotine);
            _rotateCorotine = null;
        }

        base.OnEnterBonusStage(mapIndex);
        if (mapIndex == this.mapIndex)
        {
            _group.alpha = 1;
            gameObject.SetActive(true);
        }
        else if (gameObject.activeSelf)
        {
            StartCoroutine(Fade());
        }


    }

    protected override IEnumerator Fade()
    {
        float fadeDuration = 2f;
        float fadeRate = 1 / fadeDuration;

        while (_group.alpha > 0)
        {
            _group.alpha -= fadeRate * Time.deltaTime;

            if (_group.alpha <= 0)
            {
                _group.alpha = 0;
                gameObject.SetActive(false);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator RotateRootRandomly()
    {
        Quaternion currentRotation = _root.rotation;
        Quaternion newRotation = currentRotation;

        while (true)
        {
            float waitTime = Random.Range(3f, 8f);
            yield return new WaitForSeconds(waitTime);
            BGShaker.Instance.Shake(2f);

            currentRotation = _root.rotation;
            float randomAngle = UnityEngine.Random.Range(-360f, 360f); // Choose a random angle between 0 and 360 degrees.
            newRotation = Quaternion.Euler(0, 0, randomAngle);

            float elapsedTime = 0f;
            float rotationDuration = UnityEngine.Random.Range(2f, 4f); // Duration of the rotation between 1 and 3 seconds.

            while (elapsedTime < rotationDuration)
            {
                _root.rotation = Quaternion.Lerp(currentRotation, newRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _root.rotation = newRotation; // Ensure the target rotation is set precisely at the end.
        }
    }

}
