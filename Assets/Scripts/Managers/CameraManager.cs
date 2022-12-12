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
    public GameObject blackOutScreen;
    public bool blackedOut;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        blackOutScreen.SetActive(false);
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

    public void FadeToBlack()
    {
        blackOutScreen.SetActive(true);
        blackOutScreen.GetComponent<Animator>().Play("Wipe to Black from Left");
        blackedOut = true;
    }

    public void FadeFromBlack()
    {
        blackOutScreen.GetComponent<Animator>().Play("Wipe from Black to Right");

        Invoke(nameof(SetScreensInactive), 1f);
        blackedOut = false;
    }

    public void SetScreensInactive()
    {
        blackOutScreen.SetActive(false);
    }

    public IEnumerator DoScreenShake(float intensity, float time)
    {
        camPerlin.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(time);
        camPerlin.m_AmplitudeGain = 0;
    }
}
