using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainMenu : MonoBehaviour
{
    // логин ученика, будет использоваться для определения пути к папке с world_xxx.xml
    private string _userLogin = "";

    private ScriptHerder _scriptHerder;
    private WorldController _worldController;
    private ScrHerder _scrHerder;
    // Общие параметры и методы
    MyGlobals _myGlobals;


    void Awake()
    {
        GameObject boss = GameObject.Find("Boss");
        //_scriptHerder = boss.GetComponent<ScriptHerder>();
        _scrHerder = boss.GetComponent<ScrHerder>();
        _worldController = boss.GetComponent<WorldController>();
        _myGlobals = boss.GetComponent<MyGlobals>();
    }

    // наверное, в меню сперва должен быть выбор пользователя и ввод пароля
    // потом загрузка собственно меню 
    // потом считывание файлов world для данного пользователя из его каталога
    // и формирование графического меню, в котором стоят отметки, какой step уже пройден

    // как будто был нажат пункт меню
    public void StartLoad()
    {
        // это будет определятся по нажатому пункту меню
        string topicPartStep = "10.10.10";

        // имя файла описания вида world_101010.xml
        string worldName;
        // полный путь к файлу - описанию мира
        string worldFilePath;

        print(" ====== MainMenu -> Кнопка Загрузка -> StartLoad() ====== ");

        // генерируем имя файла мира вида world_101010.xml
        string[] worldNameParts = topicPartStep.Split('.');
        worldName = "world_" + worldNameParts[0] + worldNameParts[1] + worldNameParts[2] + ".xml";

        // Полный путь к файлу XML состояния мира
        string dataPath = Path.Combine(Directory.GetCurrentDirectory(), _myGlobals.myDataPath);
        if (_userLogin == "")
        {
            worldFilePath = Path.Combine(dataPath, _myGlobals.myXWorldPath, worldName);
        }
        else
        {
            // TODO: определять, что этот файл существует, и если да, то грузить его
            worldFilePath = Path.Combine(dataPath, _myGlobals.myXWorldPath, _userLogin, worldName);
            // а если нет, то грузить из общей папки
        }

        // загрузить состояние мира (всех контролов), инициализировать их
        _worldController.Load(worldFilePath);

        /*
        // Полный путь к файлу XML сценария
        lessonFilePath = Path.Combine(Directory.GetCurrentDirectory(), XMLDataPath, XMLLessonsPath, XMLLessonFileName);
        */

        // загрузить сценарий и запустить его с требуемого места
        _scrHerder.Load(topicPartStep);

        gameObject.SetActive(false);        // спрятать меню
    }
}
