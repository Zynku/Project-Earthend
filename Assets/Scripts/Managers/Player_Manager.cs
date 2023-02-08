using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Player_Manager : MonoBehaviour
{
    Charcontrol charcontrol;
    Charanimation charanimation;
    public GameObject playerCamFollowObject;

    public GameObject playerPrefab;
    [ReadOnly] public GameObject playerLiveRef;
    [ReadOnly] public GameObject handSprite;
    [ReadOnly] public GameObject weapon;
    [ReadOnly] public GameObject playerRespawnPoint;
    public bool preventPlayerMovement = false;

    [HideInInspector] public Color playerStartColor;
    [HideInInspector] public Color handStartColor;
    [HideInInspector] public Color weaponStartColor;
    private float lerpTimeElapsed;

    public enum WalkDirection
    {
        LEFT,
        RIGHT
    }

    void Start()
    {
        charcontrol = playerLiveRef.GetComponent<Charcontrol>();
        charanimation = playerLiveRef.GetComponent<Charanimation>();

        playerRespawnPoint = GameManager.instance.playerRespawnPoint;
        playerStartColor = playerLiveRef.GetComponent<SpriteRenderer>().color;
        handStartColor = handSprite.GetComponent<SpriteRenderer>().color;
        weaponStartColor = weapon.GetComponent<SpriteRenderer>().color;
    }

    public GameObject SpawnPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLiveRef = player;

            SpriteRenderer[] children = player.GetComponentsInChildren<SpriteRenderer>();   //Finds the hand sprite and weapon sprites and assigns their gameObjects
            foreach (var item in children)
            {
                if (item.gameObject.name == "Melee Object") { weapon = item.gameObject; }
                if (item.gameObject.name == "Hand Sprite") { handSprite = item.gameObject; }
            }

            Charcontrol charcontrol = playerLiveRef.GetComponent<Charcontrol>();
            Charhealth charhealth = playerLiveRef.GetComponent<Charhealth>();

            charcontrol.playerDead = false;
            charhealth.playerDead = false;
            charhealth.SetHealthIntially();
            EnablePlayer();
            return player;
        }
        else
        {
            playerRespawnPoint = GameManager.instance.playerRespawnPoint;
            playerLiveRef = Instantiate(playerPrefab, playerRespawnPoint.transform.position, Quaternion.identity, gameObject.transform);
            playerLiveRef.SetActive(true);

            SpriteRenderer[] children = player.GetComponentsInChildren<SpriteRenderer>();   //Finds the hand sprite and weapon sprites and assigns their gameObjects
            foreach (var item in children)
            {
                if (item.gameObject.name == "Melee Object") { weapon = item.gameObject; }
                if (item.gameObject.name == "Hand Sprite") { handSprite = item.gameObject; }
            }

            Charcontrol charcontrol = playerLiveRef.GetComponent<Charcontrol>();
            Charhealth charhealth = playerLiveRef.GetComponent<Charhealth>();

            charcontrol.playerDead = false;
            charhealth.playerDead = false;
            charhealth.SetHealthIntially();
            EnablePlayer();
            return playerLiveRef;
        }
    }

    public void DespawnPlayer()
    {
        playerLiveRef.SetActive(false);
        Destroy(playerLiveRef);
    }

    public void DisablePlayer()
    {
        playerLiveRef.GetComponent<Renderer>().enabled = false;
        playerLiveRef.GetComponent<Animator>().enabled = false;
        playerLiveRef.GetComponentInChildren<ParticleSystem>().Pause();
        foreach (var item in playerLiveRef.GetComponentsInChildren<Renderer>())
        {
            var itemRenderer = item.GetComponent<Renderer>();
            var itemAnimator = item.GetComponent<Animator>();

            if (itemRenderer != null) { itemRenderer.enabled = false; }
            if (itemAnimator != null) { itemAnimator.enabled = false; }
        }
        preventPlayerMovement = true;
    }

    public void EnablePlayer()
    {
        playerLiveRef.GetComponent<Charcontrol>().currentState = Charcontrol.State.Idle;
        
        playerLiveRef.GetComponent<Renderer>().enabled = true;
        playerLiveRef.GetComponent<Animator>().enabled = true;
        playerLiveRef.GetComponent<Animator>().Play("Low Poly Idle");          //Plays default anim


        playerLiveRef.GetComponentInChildren<ParticleSystem>().Play();
        playerLiveRef.GetComponent<Charhealth>().Start();

        BoxCollider2D[] boxCols = playerLiveRef.GetComponentsInChildren<BoxCollider2D>();

        foreach (var col in boxCols)    //Disables melee gameObjects
        {
            GameObject colGO = col.gameObject;
            if (colGO.CompareTag("player_attackhitbox"))
            {
                colGO.SetActive(true);
            }
        }

        foreach (var item in playerLiveRef.GetComponentsInChildren<Renderer>())
        {
            item.gameObject.SetActive(true);
            var itemRenderer = item.GetComponent<Renderer>();
            var itemAnimator = item.GetComponent<Animator>();

            if (itemRenderer != null) { itemRenderer.enabled = true; }
            if (itemAnimator != null) { itemAnimator.enabled = true; }
        }
        preventPlayerMovement = false;
        
    }

    public void ResetPlayer()
    {
        playerLiveRef.GetComponent<Charcontrol>().currentState = Charcontrol.State.Idle;

        playerLiveRef.GetComponent<Renderer>().enabled = true;
        playerLiveRef.GetComponent<Animator>().enabled = true;
        playerLiveRef.GetComponent<Animator>().Play("Low Poly Idle");          //Plays default anim


        playerLiveRef.GetComponentInChildren<ParticleSystem>().Play();

        BoxCollider2D[] boxCols = playerLiveRef.GetComponentsInChildren<BoxCollider2D>();

        foreach (var col in boxCols)    //Disables melee gameObjects
        {
            GameObject colGO = col.gameObject;
            if (colGO.tag == "player_attackhitbox")
            {
                colGO.SetActive(true);
            }
        }

        foreach (var item in playerLiveRef.GetComponentsInChildren<Renderer>())
        {
            item.gameObject.SetActive(true);
            var itemRenderer = item.GetComponent<Renderer>();
            var itemAnimator = item.GetComponent<Animator>();

            if (itemRenderer != null) { itemRenderer.enabled = true; }
            if (itemAnimator != null) { itemAnimator.enabled = true; }
        }
        preventPlayerMovement = false;
    }

    public void SnapCameraToPosition(Vector2 position)
    {
        charcontrol.cameraFollowObject.transform.position = position;
    }

    public void ResetCameraToPlayer()
    {
        charcontrol.cameraFollowObject.transform.position = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Forces the player to walk in a certain direction for a given amount of time
    /// </summary>
    public void ForceWalk(WalkDirection walkDir, float duration)
    {
        switch (walkDir)
        {
            case WalkDirection.LEFT:    //Forcing the character to walk left
                if (charcontrol.facingDir == -1) //If we're facing left, its already the right direction
                {
                    Debug.Log("Must go left, and the player is facing left. Great!");
                }
                else                            //If, however, we're facing right, fix that
                {
                    Debug.Log("Must go left, and the player is facing right. Flipping X dir to match.");
                    charcontrol.FlipXDirLeft();
                }
                break;
            case WalkDirection.RIGHT:
                if (charcontrol.facingDir == 1) //If we're facing right, its already the right direction
                {
                    Debug.Log("Must go right, and the player is facing right. Great!");
                }
                else                            //If, however, we're facing left, fix that
                {
                    Debug.Log("Must go right, and the player is facing left. Flipping X dir to match.");
                    charcontrol.FlipXDirRight();
                }
                break;
        }
        StartCoroutine(ForceWalkCO(5));
    }

    /// <summary>
    /// Forces the player to run in a certain direction for a given amount of time
    /// </summary>
    public void ForceRun(WalkDirection runDir, float duration)
    {
        switch (runDir)
        {
            case WalkDirection.LEFT:    //Forcing the character to walk left
                if (charcontrol.facingDir == -1) //If we're facing left, its already the right direction
                {
                    Debug.Log("Must go left, and the player is facing left. Great!");
                }
                else                            //If, however, we're facing right, fix that
                {
                    Debug.Log("Must go left, and the player is facing right. Flipping X dir to match.");
                    charcontrol.FlipXDirLeft();
                }
                break;
            case WalkDirection.RIGHT:
                if (charcontrol.facingDir == 1) //If we're facing right, its already the right direction
                {
                    Debug.Log("Must go right, and the player is facing right. Great!");
                }
                else                            //If, however, we're facing left, fix that
                {
                    Debug.Log("Must go right, and the player is facing left. Flipping X dir to match.");
                    charcontrol.FlipXDirRight();
                }
                break;
        }
        StartCoroutine(ForceRunCO(duration));
    }

    private IEnumerator ForceWalkCO(float duration)
    {
        float forceWalkTimer = duration;
        while (forceWalkTimer > 0)
        {
            yield return new WaitForEndOfFrame();
            forceWalkTimer -= Time.deltaTime;
            Debug.Log($"Forcing walk for {forceWalkTimer} more seconds");
            charcontrol.ForceWalking();
        }
    }

    private IEnumerator ForceRunCO(float duration)
    {
        float forceRunTimer = duration;
        charanimation.animator.SetBool("Running", true);
        StartCoroutine(Charinputs.instance.DisableAllInputsForDuration(0.7f));
        while (forceRunTimer > 0)
        {
            yield return new WaitForEndOfFrame();
            forceRunTimer -= Time.deltaTime;
            Debug.Log($"Forcing walk for {forceRunTimer} more seconds");
            charcontrol.ForceRunning();
            charcontrol.currentState = Charcontrol.State.Running;
        }
        Debug.Log("Done running");
        charanimation.animator.SetBool("Running", false);
    }

    public IEnumerator DoFadeOut(float duration)    //Fades from current player color to transparent over duration time
    {
        SpriteRenderer pSpriteRenderer = playerLiveRef.GetComponent<SpriteRenderer>();  //Gets the sprite renderers
        SpriteRenderer hSpriteRenderer = handSprite.GetComponent<SpriteRenderer>();
        SpriteRenderer wSpriteRenderer = weapon.GetComponent<SpriteRenderer>();

        Color pStartColor = playerStartColor;   //Gets their starting color so we know where to start lerping from
        Color hStartColor = handStartColor;
        Color wStartColor = weaponStartColor;

        Color pTransParentColor = new(pStartColor.r, pStartColor.g, pStartColor.b, 0f);    //Finds the color we're trying to get to for each
        Color hTransParentColor = new(hStartColor.r, hStartColor.g, hStartColor.b, 0f);
        Color wTransParentColor = new(wStartColor.r, wStartColor.g, wStartColor.b, 0f);
        lerpTimeElapsed = 0;
        while (pSpriteRenderer.color != pTransParentColor)  //While loop to continually update values until they match
        {
            //Debug.Log($"Lerping color out. Player color is {spriteRenderer.color}");
            yield return new WaitForEndOfFrame();
            lerpTimeElapsed += Time.deltaTime;
            float percentageComplete = lerpTimeElapsed / duration;
            pSpriteRenderer.color = Color.Lerp(pStartColor, pTransParentColor, percentageComplete);
            hSpriteRenderer.color = Color.Lerp(hStartColor, hTransParentColor, percentageComplete);
            wSpriteRenderer.color = Color.Lerp(wStartColor, wTransParentColor, percentageComplete);
        }
        //Debug.Log($"Player is transparent!");
    }
    
    public IEnumerator DoFadeIn(float duration) //Fades from transparent to initial player color over time
    {
        SpriteRenderer pSpriteRenderer = playerLiveRef.GetComponent<SpriteRenderer>();  //Gets the sprite renderers
        SpriteRenderer hSpriteRenderer = handSprite.GetComponent<SpriteRenderer>();
        SpriteRenderer wSpriteRenderer = weapon.GetComponent<SpriteRenderer>();

        Color pStartColor = playerStartColor;   //Gets their starting color so we know where to start lerping from
        Color hStartColor = handStartColor;
        Color wStartColor = weaponStartColor;

        Color pTransParentColor = new Color(pStartColor.r, pStartColor.g, pStartColor.b, 0f);
        Color hTransParentColor = new Color(hStartColor.r, hStartColor.g, hStartColor.b, 0f);
        Color wTransParentColor = new Color(wStartColor.r, wStartColor.g, wStartColor.b, 0f);

        lerpTimeElapsed = 0;
        while (pSpriteRenderer.color != pStartColor)
        {
            //Debug.Log($"Lerping color in. Player color is {spriteRenderer.color}");
            yield return new WaitForEndOfFrame();
            lerpTimeElapsed += Time.deltaTime;
            float percentageComplete = lerpTimeElapsed / duration;
            pSpriteRenderer.color = Color.Lerp(pTransParentColor, pStartColor, percentageComplete);
            hSpriteRenderer.color = Color.Lerp(hTransParentColor, hStartColor, percentageComplete);
            wSpriteRenderer.color = Color.Lerp(wTransParentColor, wStartColor, percentageComplete);
        }
        //Debug.Log($"Player is opaque!");
    }

    bool playerPosRecorded = false;
    Vector2 playerPosAtPause;
    // Update is called once per frame
    public void Update()
    {
        if (preventPlayerMovement)
        {
            if (!playerPosRecorded)
            {
                playerPosAtPause = playerLiveRef.transform.position;
                playerPosRecorded = true;
            }
            playerLiveRef.transform.position = playerPosAtPause;
        }
        else
        {
            playerPosRecorded = false;
        }
    }
}
