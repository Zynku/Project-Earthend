using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class devlab_turret : MonoBehaviour
{
    public GameObject target;
    GameObject Player;
    AudioSource audioSource;
    Animator animator;

    [Foldout("Assignments", true)]
    public GameObject shootingPoint;    //Where bullets are instantiated from
    public GameObject turretGun;
    public GameObject turretCenterCog;
    public GameObject turretLightArm;
    public GameObject turretLight;
    public List<Sprite> turretLightColors;

    [Foldout("Audio", true)]
    [Range(0f, 1f)] public float shootingVolume = 1;
    public AudioClip[] shootingSounds;
    [Range(0f, 1f)] public float activateVolume = 1;
    public AudioClip activateSound;
    private bool activatePlayed;
    [Range(0f, 1f)] public float deactivateVolume = 1;
    public AudioClip deactivateSound;
    private bool deactivatePlayed;
    [Range(0f, 1f)] public float alertVolume = 1;
    public AudioClip alertSound;
    private bool alertPlayed;

    [Foldout("Bullets", true)]
    public GameObject energyBulletPrefab;
    public float bulletSpeed;
    public float bulletDelay;
    public float randomBulletRotOffset;
    private float randomBulletShootDelay = 0.05f;

    [Foldout("Ranges", true)]
    public float alertRange;
    public float attackRange;
    public bool targetInAlertRange;
    public bool targetInAttackRange;

    [Foldout("Variables", true)]
    public float rotationSpeed;
    public float maxDamage, minDamage;
    public float gunRotation;
    public bool targetPlayer;
    public bool onCoolDown;
    public bool lightExtended;

    [Separator("States", true)]
    public Type turretType;
    public AlertState alertState;
    public enum Type
    {
        Repeater
    }

    public enum AlertState
    {
        Idle,
        Warning,
        Active,
        Firing
    }
    void Start()
    {
        gunRotation = turretGun.transform.eulerAngles.z;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {        
        if (!Player) { Player = GameManager.instance.Player; }
        if (targetPlayer) { target = Player; }

        if (Vector3.Distance(target.transform.position, transform.position) < alertRange) { targetInAlertRange = true;}
        else { targetInAlertRange = false;}

        if (Vector3.Distance(target.transform.position, transform.position) < attackRange) { targetInAttackRange = true;}
        else { targetInAttackRange = false;}

        if (!targetInAlertRange && !targetInAttackRange) { alertState = AlertState.Idle; }
        if (targetInAlertRange && !targetInAttackRange) { alertState = AlertState.Warning;}
        if (targetInAlertRange && targetInAttackRange) { alertState = AlertState.Active; }

        if (targetInAttackRange)
        {
            if (!onCoolDown) { StartCoroutine(ShootAtTarget()); onCoolDown = true; }
        }

        switch (alertState)
        {
            case AlertState.Idle:
                animator.SetBool("Target In Shooting Range", false);
                alertPlayed = false;
                deactivatePlayed = false;
                activatePlayed = false;
                break;
            case AlertState.Warning:
                animator.SetBool("Target In Shooting Range", true);
                break;
            case AlertState.Active:
                animator.SetBool("Target In Shooting Range", true);
                break;
            case AlertState.Firing:
                animator.SetBool("Target In Shooting Range", true);
                break;
            default:
                break;
        }

        DoRotation();
    }

    public float LookAtAngle()  //Funky math to calculate the angle between the turret and the target
    {
        float yDiff = target.transform.position.y - turretGun.transform.position.y;
        float xDiff = target.transform.position.x - turretGun.transform.position.x;
        float calculatedAngle = Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg + 90f;
        return calculatedAngle;
    }

    public void DoRotation()
    {
        //Debug.Log($"Gun Rotation is {Mathf.Floor(gunRotation)} & Look angle is {Mathf.Floor(LookAtAngle())}");
        gunRotation = Mathf.Lerp(gunRotation, LookAtAngle(), rotationSpeed * Time.deltaTime);   //Lerps between current rotation and the look at angle

        turretGun.transform.eulerAngles = new Vector3(0, 0, gunRotation);
        turretCenterCog.transform.eulerAngles = new Vector3(0, 0, -gunRotation);
    }

    public IEnumerator ShootAtTarget()
    {
        alertState = AlertState.Firing;
        float randomBulletShotDelay = Random.Range(-randomBulletShootDelay, randomBulletShootDelay);
        yield return new WaitForSeconds(bulletDelay + randomBulletShotDelay);

        float randomBulletOffset = Random.Range(-randomBulletRotOffset, randomBulletRotOffset);

        GameObject bullet = Instantiate(energyBulletPrefab, shootingPoint.transform.position, turretGun.transform.rotation, turretGun.transform);
        bullet.transform.eulerAngles = new Vector3(0, 0, turretGun.transform.eulerAngles.z + 90f + randomBulletOffset);
        devlab_energy_bullet bulletscripp = bullet.GetComponent<devlab_energy_bullet>();
        bulletscripp.speed = bulletSpeed;
        bulletscripp.maxDamage = maxDamage;
        bulletscripp.minDamage = minDamage;
        bulletscripp.firedFrom = gameObject;
        bulletscripp.target = target;

        AudioClip randomClip = shootingSounds[Random.Range(0, shootingSounds.Length - 1)];
        audioSource.volume = shootingVolume;
        audioSource.PlayOneShot(randomClip);

        onCoolDown = false;
    }

    public void AudOnAlert()
    {
        if (!alertPlayed)
        {
            audioSource.volume = alertVolume;
            audioSource.PlayOneShot(alertSound);
        }
    }

    public void AudOnActivate()
    {
        if (!activatePlayed)
        {
            audioSource.volume = activateVolume;
            audioSource.PlayOneShot(activateSound);
        }
    }

    public void AudOnDeactivate()
    {
        if (!deactivatePlayed)
        {
            audioSource.volume = deactivateVolume;
            audioSource.PlayOneShot(deactivateSound);
        }
    }


    private void OnDrawGizmos()
    {
        //Alert range gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(turretCenterCog.transform.position, alertRange);

        //Attack range gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(turretCenterCog.transform.position, attackRange);
    }
}
