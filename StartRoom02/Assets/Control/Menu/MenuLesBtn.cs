using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;

public class MenuLesBtn : MonoBehaviour
{
    public MenuLessons Menu;
    private string _menuType;     // les, part, step
    private Text _normText;
    private Text _selText;
    private GameObject _btnNorm;
    private GameObject _btnSelect;

    public string BtnNum;
    public string BtnText;

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

        SetNorm();
    }

    private void Start()
    {
        SetNorm();
    }

    public void SetDate(string num, string txt, string type)
    {
        BtnNum = num;
        BtnText = txt;
        _normText.text = txt;
        _selText.text = txt;
        _menuType = type;
    }

    // привязана кнопкe, вызывается по onClick
    private void ToggleState()
    {
        if (_btnNorm.activeSelf)
        {
            SetPress();
            Menu.SelectPunct(_menuType, BtnNum );
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
