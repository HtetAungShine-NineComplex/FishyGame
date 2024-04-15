using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextCountAnimation : MonoBehaviour
{
    public float countDuration = 1.5f;
    Text numberText;
    float currentValue = 0, targetValue = 0;
    Coroutine _C2T;

    void Awake()
    {
        numberText = GetComponent<Text>();
    }

/*    void Start()
    {
        currentValue = float.Parse(numberText.text);
        targetValue = currentValue;
    }*/
    private void OnEnable()
    {
        currentValue = float.Parse(numberText.text);
        targetValue = currentValue;
        StartCoroutine(IAddValue());
    }
    IEnumerator CountTo(float targetValue)
    {
        var rate = Mathf.Abs(targetValue - currentValue) / countDuration;
        while (currentValue != targetValue)
        {
            currentValue = Mathf.MoveTowards(currentValue, targetValue, rate * Time.deltaTime);
            numberText.text = ((int)currentValue).ToString();
            yield return null;
        }
    }

    public void AddValue(float value)
    {
        targetValue += value;
        if (_C2T != null)
            StopCoroutine(_C2T);
        _C2T = StartCoroutine(CountTo(targetValue));
    }

    public void SetTarget(float target)
    {
        targetValue = target;
        if (_C2T != null)
            StopCoroutine(_C2T);
        _C2T = StartCoroutine(CountTo(targetValue));
    }
    
    IEnumerator IAddValue()
    {
        int count =0;
        SetTarget(0);
        while (count < 7)
        {
            yield return new WaitForSeconds(2f);
            AddValue(200);
            count++;
        }
        
    }
}