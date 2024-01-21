using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingGameSceneManager : GameSceneManager
{
    private void Start()
    {
        BGSoundManager.Instance.PlaySound(BGSoundManager.Instance._gameplaySound);
    }
}
