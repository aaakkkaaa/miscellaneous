using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;

public class MenuBtn : MonoBehaviour
{
    public MenuAccount Menu;
    private string _btnText;
    private Text _normText;
    private Text _selText;
    private GameObject _btnNorm;
    private GameObject _btnSelect;

    public string BtnText => _btnText;

    private void Awake()
    {
        // получим ссылки на кнопки, для управления видом, подпишемся на событие onClick
        _btnNorm = transform.Find("Btn_Norm").gameObject;
        Button btnOnScript = _btnNorm.GetComponent<Button>();
        btnOnScript.onClick.AddListener(ToggleState);
        _btnSelect = transform.Find("Btn_Select").gameObject;

        // получим ссылки на текстовые поля, для установки подписей к кнопкам
        GameObject onGameObjText = transform.Find("Btn_Norm/Text").gameObject;
        _normText = onGameObjText.GetComponent<Text>();
        GameObject offGameObjText = transform.Find("Btn_Select/Text").gameObject;
        _selText = offGameObjText.GetComponent<Text>();
        _selText.text = _btnText;

        SetNorm();
    }

    private void Start()
    {
        SetNorm();
    }

    public void SetText(string txt)
    {
        _btnText = txt;
        _normText.text = _btnText;
        _selText.text = _btnText;
    }

    // привязана кнопкe, вызывается по onClick
    private void ToggleState()
    {
        if (_btnNorm.activeSelf)
        {
            SetPress();
            Menu.SelectPerson(_btnText);
        }
    }

    public void SetNorm()
    {
        _btnSelect.SetActive(false);
        _btnNorm.SetActive(true);
    }
    public void SetPress()
    {
        _btnSelect.SetActive(true);
        _btnNorm.SetActive(false);
    }
}
