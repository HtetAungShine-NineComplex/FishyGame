using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void Start()
    {
        Destroy(gameObject, 1f);
    }

    public void SetValueText(int amount)
    {
        _text.text = amount.ToString();
    }
}
