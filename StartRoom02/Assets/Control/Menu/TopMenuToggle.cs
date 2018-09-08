using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopMenuToggle : MonoBehaviour
{
    public string MenuType;
    public string BtnText;

    private MainMenu _mainMenu;
    private GameObject _btnOn;
    private GameObject _btnOff;

    private void Awake()
    {
        // получим ссылку на скрипт MainMenu
        GameObject mainMenuCanvas = GameObject.Find("MenuCanvas");
        _mainMenu = mainMenuCanvas.GetComponent < MainMenu > ();

        // получим ссылки на кнопки, для управления видом, подпишемся на событие onClick
        _btnOn = transform.Find("Btn_On").gameObject;
        Button btnOnScript = _btnOn.GetComponent<Button>();
        btnOnScript.onClick.AddListener(ToggleState);
        _btnOff = transform.Find("Btn_Off").gameObject;
        Button btnOffScript = _btnOff.GetComponent<Button>();
        btnOffScript.onClick.AddListener(ToggleState);

        // получим ссылки на текстовые поля, для установки подписей к кнопкам
        GameObject onGameObjText = transform.Find("Btn_On/Text").gameObject;
        Text onText = onGameObjText.GetComponent<Text>();
        onText.text = BtnText;
        GameObject offGameObjText = transform.Find("Btn_Off/Text").gameObject;
        Text offText = offGameObjText.GetComponent<Text>();
        offText.text = BtnText;

        _btnOff.SetActive(false);
    }

    // привязана кнопкам, вызывается по onClick
    private void  ToggleState()
    {
        //print("ToggleState");
        if(_btnOff.activeSelf)
        {
            _mainMenu.TopMenuHide(MenuType);
            SetNorm();
        }
        else
        {
            SetPress();
            _mainMenu.TopMenuShow(MenuType);
        }
    }

    public void SetNorm()
    {
        _btnOff.SetActive(false);
        _btnOn.SetActive(true);
    }
    public void SetPress()
    {
        _btnOff.SetActive(true);
        _btnOn.SetActive(false);
    }



}
