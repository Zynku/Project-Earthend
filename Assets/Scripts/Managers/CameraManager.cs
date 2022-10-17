using Cinemachine;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Separator("Cameras")]
    public CinemachineBrain cMBrain;
    public CinemachineVirtualCamera mainCamera;
    [ReadOnly] public CinemachineVirtualCameraBase mainCameraBase;
    CinemachineBasicMultiChannelPerlin camPerlin;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
    }

    public void SetupCameras()
    {
        if(gameManager == null) { gameManager = GameManager.instance; }
        cMBrain = FindObjectOfType<CinemachineBrain>();
        //mainCamera = FindObjectOfType<CinemachineVirtualCamera>();
        camPerlin = mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        mainCamera.Follow = gameManager.Player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DoScreenShake(float intensity, float time)
    {
        camPerlin.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(time);
        camPerlin.m_AmplitudeGain = 0;
    }
}
