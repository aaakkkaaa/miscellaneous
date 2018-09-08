using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTuning : MonoBehaviour
{
    private MyGlobals _myGlobals;

    private Toggle _tips;
    private Toggle _instruc;
    private Toggle _joystick;
    private Slider _volume;
    private Text _value;

    void Awake ()
    {
        GameObject boss = GameObject.Find("Boss");
        _myGlobals = boss.GetComponent<MyGlobals>();
        // получить ссылки на объекты этого меню
        _tips = GameObject.Find("Content/IsTips").GetComponent<Toggle>();
        _instruc = GameObject.Find("Content/IsInstruc").GetComponent<Toggle>();
        _joystick = GameObject.Find("Content/IsJoystick").GetComponent<Toggle>();
        _volume = GameObject.Find("Content/SoundVolume").GetComponent<Slider>();
        _value = GameObject.Find("Content/SoundValue").GetComponent<Text>();
        // подписка на изменения состояния контролов
        _tips.onValueChanged.AddListener(OnTipsClick);
        _instruc.onValueChanged.AddListener(OnInstrucClick);
        _joystick.onValueChanged.AddListener(OnJoystickClick);
        _volume.onValueChanged.AddListener(OnSoundVolumeChanged);
    }

    // При получении сообщения меняем состояние переменных в _myGlobals
    private void OnTipsClick(bool isToggle)
    {
        _myGlobals.isTips = isToggle;
    }
    private void OnInstrucClick(bool isToggle)
    {
        _myGlobals.isInstruc = isToggle;
    }
    private void OnJoystickClick(bool isToggle)
    {
        _myGlobals.isJoystick = isToggle;
    }
    private void OnSoundVolumeChanged(float value)
    {
        _myGlobals.sndVolume = value;
        int percent = (int)(value * 100);
        _value.text = percent.ToString();

    }

    // вызывается перед показом меню для устаовки флажков и слайдера в правильное состояние
    public void GetValuesFromGlobals()
    {
        _tips.isOn = _myGlobals.isTips;
        _instruc.isOn = _myGlobals.isInstruc;
        _joystick.isOn = _myGlobals.isJoystick;
        _volume.value = _myGlobals.sndVolume;
    }


}
