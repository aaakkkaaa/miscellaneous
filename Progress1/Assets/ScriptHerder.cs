using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScriptHerder : MonoBehaviour
{
    private WorldController _worldController;
    // содержит список action данного шага
    private ScriptData _scriptData;

    void Awake()
    {
        _worldController = GetComponent<WorldController>();
    }

    public void Load(string fName)
    {
        _scriptData = ScriptData.Load( Path.Combine(Application.dataPath, fName) );
        string worldFName = _scriptData.worldFileName;
        // TODO: возможно состояние мира на начало шага "стандартное" или "пользовательское" - это разные файлы

        _worldController.Load( worldFName );
    }
}
