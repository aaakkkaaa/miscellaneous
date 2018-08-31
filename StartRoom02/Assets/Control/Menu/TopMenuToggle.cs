using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopMenuToggle : MonoBehaviour
{
    public string MenuType;
    public string Text;

    private GameObject _btnOn;
    private GameObject _btnOff;

    private void Awake()
    {

        _btnOn = transform.Find("Btn_On").gameObject;
        Button btnOnScript = _btnOn.GetComponent<Button>();
        btnOnScript.onClick.AddListener(ToggleState);
        _btnOff = transform.Find("Btn_Off").gameObject;
        Button btnOffScript = _btnOff.GetComponent<Button>();
        btnOffScript.onClick.AddListener(ToggleState);

        GameObject onGameObjText = transform.Find("Btn_On/Text").gameObject;
        Text onText = onGameObjText.GetComponent<Text>();
        onText.text = Text;
        GameObject offGameObjText = transform.Find("Btn_Off/Text").gameObject;
        Text offText = offGameObjText.GetComponent<Text>();
        offText.text = Text;

        _btnOff.SetActive(false);
    }

    private void Start ()
    {
    }

    public void  ToggleState()
    {
        print("ToggleState");
        if(_btnOff.activeSelf)
        {
            _btnOff.SetActive(false);
            _btnOn.SetActive(true);
        }
        else
        {
            _btnOff.SetActive(true);
            _btnOn.SetActive(false);
        }
    }

}
