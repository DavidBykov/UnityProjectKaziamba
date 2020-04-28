using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level ", menuName = "Level configuration", order = 51)]
public class LevelConfiguration : ScriptableObject
{
    [Tooltip("Название выводимое в меню")]
    public string LevelName;
    [Tooltip("Полное описание игрового уровня")]
    public string LevelDescription;
    [Tooltip("Подсказка на загрузочном экране")]
    public string LevelLoadingHint;
    [Tooltip("Параметры уровня, применяющиеся при его старте")]
    public GameParemeters LevelGameSettings;
    [Tooltip("Сцена которая соответствует уровню")]
    public string LoadingScene;
    [Tooltip("Начинать без описания задания")]
    public bool startWithoutDescription;
    [Tooltip("Показать подсказку при открытии задания")]
    public bool showHint;
    [Tooltip("ID уровня для сохранения")]
    public string levelSaveLoadID;

    //public bool completed;
}
