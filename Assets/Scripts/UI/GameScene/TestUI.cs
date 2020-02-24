using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TestUI : MonoBehaviour
{
    public Player player;
    public PlayerCamera playerCamera;
    public Camera mainCamera;

    public Text playerSpeedText;
    public Text cameraSpeedText;
    public Text isometryText;

    public void IncreasePlayerSpeed()
    {
        player.speed += 10f;
        playerSpeedText.text = player.speed.ToString();
    }

    public void DecreasePlayerSpeed()
    {
        player.speed -= 10f;
        playerSpeedText.text = player.speed.ToString();
    }

    public void IncreaseCameraSmooth()
    {
        playerCamera.smooth += 0.01f;
        cameraSpeedText.text = "0" + playerCamera.smooth.ToString("#.##");
    }

    public void DecreaseCameraSmooth()
    {
        playerCamera.smooth -= 0.01f;
        cameraSpeedText.text = "0" + playerCamera.smooth.ToString("#.##");
    }

    public void TurnOnIsometry()
    {
        isometryText.text = "Yes";
        mainCamera.orthographic = true;
    }

    public void TurnOffIsometry()
    {
        isometryText.text = "No";
        mainCamera.orthographic = false;
    }
}
