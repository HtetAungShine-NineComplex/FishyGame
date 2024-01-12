using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CannonAmountUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private CannonHandler _cannon;

    private void OnEnable()
    {
        _cannon.AmountChanged += OnAmountChanged;
    }

    private void OnDisable()
    {
        _cannon.AmountChanged -= OnAmountChanged;
    }

    private void OnAmountChanged(int amount)
    {
        _text.text = amount.ToString();
    }
}
