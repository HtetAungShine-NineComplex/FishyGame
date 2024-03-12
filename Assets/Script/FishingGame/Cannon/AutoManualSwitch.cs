using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoManualSwitch : MonoBehaviour
{
    [SerializeField] private CannonController _cannonController;
    [SerializeField] private Sprite _autoSprite;
    [SerializeField] private Sprite _manualSprite;
    [SerializeField] private Button _btn;

    public void Start()
    {
        _btn.onClick.AddListener(Switch);
    }

    public void Switch()
    {
        _cannonController.ToggleAutoShoot();
        if (_btn.image.sprite == _manualSprite)
        {
            _btn.image.sprite = _autoSprite;
        }
        else
        {
            _btn.image.sprite = _manualSprite;
        }
    }
}
