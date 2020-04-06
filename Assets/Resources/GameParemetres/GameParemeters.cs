using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameParemetres", menuName = "Game settings", order = 51)]
public class GameParemeters : ScriptableObject
{
    [Tooltip("Время игрового уровня")]
    public float gameTime;
    [Tooltip("Количество душ, которое необходимо собрать для победы")]
    public float startSoulsCount;
    [Tooltip("Время за которое скорость души возрастет с нуля до максимального значения после обнаружения игрока")]
    public float accelerationDuration;
    [Tooltip("Время за которое скорость души упадет с максимального значения до нуля после потери игрока из виду")]
    public float slowdownDuration;
    [Tooltip("Скорость души при убегании от игрока")]
    public float soulSpeed;
    [Tooltip("Скорость души при блуждании вдали от игрока")]
    public float soulWalkingSpeed;
    [Tooltip("Диапазон времени (в секундах), обозначающий, как часто душа меняет направление пока блуждает вдали от игрока")]
    public Vector2 soulWalkingChangeDirectionPeriod;
    [Tooltip("Дистанция на которой душа будет обнаруживать другие души и учитывать их вектор движения")]
    public float soulDetectionDistance;
    [Tooltip("Дистанция на которой душа будет обнаруживать игрока")]
    public float playerDetectionDistance;
    [Tooltip("Скорость игрока")]
    public float playerSpeed;
    [Tooltip("Скорость игрока когда он сопровождает души")]
    public float playerSpeedWhenTargetingSouls;
    [Tooltip("Высота камеры над игроком")]
    public float cameraHeightDistance;
    [Tooltip("Сила сглаживания перемещения следящей за игроком камеры. Чем ниже тем плавнее")]
    public float cameraSmoothPosition;
    [Tooltip("Расстояние перед игроком на котором держится камера при передвижении. Учитывает вертикальную ось")]
    public float cameraForwardVerticalDistance;
    [Tooltip("Расстояние перед игроком на котором держится камера при передвижении. Учитывает горизонтальную ось")]
    public float cameraForwardHorizontalDistance;
    [Tooltip("Кривая зависимости выбора типа поведения души от дистанции до игрока")]
    public AnimationCurve behaviourWeightByDistanceCurve;
}
