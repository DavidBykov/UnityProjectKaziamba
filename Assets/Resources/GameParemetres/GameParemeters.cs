﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEndCondition
{
    CatchedAllSouls,
    ReceivedRightEnergyAmount,
    BossDefeated
}

[CreateAssetMenu(fileName = "GameParemetres", menuName = "Game settings", order = 51)]
public class GameParemeters : ScriptableObject
{
    [Tooltip("Время игрового уровня")]
    public float gameTime;
    [Tooltip("Количество душ на уровне")]
    public float soulsOnLevel;
    [Tooltip("Количество душ, которое необходимо собрать для победы")]
    public float neededEnergy;
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
    [Tooltip("С какой силой игрока оттолкнет от святой стены если у него есть лишняя жизнь")]
    public float pushFromHolyWallForce = 5f;
    [Tooltip("Максимально возможная сила перемещения по льду (чем выше тем сильнее черт разгоняется и скользит)")]
    public float maxIceVelocitySpeed = 3f;
    [Tooltip("За сколько секунд скорость черта станет максимальной с нуля при обычном перемещении (не на льду)")]
    public float speedUpDuration = 0.3f;
    [Tooltip("Количество жизней игрока")]
    public int playerLifes = 1;
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
    [Tooltip("Время в течении которого засчитывается бонус энергии от ловли душ")]
    public float timeBetwenSoulsCatch;
    [Tooltip("Сколько энергии дается за ловлю души")]
    public int energyForCatchSoul;
    [Tooltip("Условие при котором игрок выигрывает уровень")]
    public GameEndCondition gameEndCondition;
}
