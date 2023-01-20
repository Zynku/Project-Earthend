using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMathTests : MonoBehaviour
{
    GameObject player;
    public ExtendDir extendAxis;

    public float extension;
    public float extensionRateOfChange;
    public float updateRate;
    public float distanceFromPlayer;
    private float startingX;
    private float startingY;
    private float startingXscale;
    private float startingYscale;

    public float variableA;
    public float variableB;
    public float variableC;
    public float timeVariable;

    public enum ExtendDir
    {
        XExtend,
        YExtend
    }

    private void Start()
    {
        StartCoroutine(CalculateRateOfChange());
            player = GameManager.instance.Player;
        startingX = transform.position.x;
        startingY = transform.position.y;
        startingXscale = transform.localScale.x;
        startingYscale = transform.localScale.y;
    }

    private void Update()
    {
        CalculateExtension();
        distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
        timeVariable += Time.unscaledTime;

        switch (gameObject.name)
        {
            case "Square":
                switch (extendAxis)
                {
                    case ExtendDir.XExtend:
                        transform.localScale = new Vector3(extension, startingYscale, transform.localScale.z);
                        transform.position = new Vector3(extension / 2 + startingX, startingY, transform.localPosition.z);
                        break;
                    case ExtendDir.YExtend:
                        transform.localScale = new Vector3(startingXscale, extension, transform.localScale.z);
                        transform.position = new Vector3(startingX, extension / 2 + startingY, transform.localPosition.z);
                        break;
                    default:
                        break;
                }
                break;

            case "Circle":
                transform.localScale = new Vector3(extension, extension, transform.localScale.z);
                //transform.position = new Vector3(extension / 2 + startingX, extension / 2 + startingY, transform.localPosition.z);
                break;

            default:
                switch (extendAxis)
                {
                    case ExtendDir.XExtend:
                        transform.localScale = new Vector3(extension, startingYscale, transform.localScale.z);
                        transform.position = new Vector3(extension / 2 + startingX, startingY, transform.localPosition.z);
                        break;
                    case ExtendDir.YExtend:
                        transform.localScale = new Vector3(startingXscale, extension, transform.localScale.z);
                        transform.position = new Vector3(startingX, extension / 2 + startingY, transform.localPosition.z);
                        break;
                    default:
                        break;
                }
                break;
        }
    }

    public void CalculateExtension()
    {
        //extension = (Mathf.Pow(distanceFromPlayer, 3) + Mathf.Pow(distanceFromPlayer, 2))/variableA;
        //extension = Mathf.Pow(variableA, .04761904761f) * Mathf.Pow(timeVariable, .04761904761f);

        //A modified sine wave. variable A controls the speed at which the pulsing happens, variable B controls the "Y shift" of the sin wave. A high shift
        //causes all values to be above the X axis, meaning no values are clamped, a lower shift moves the sine wave partially below the X axis, clamping
        //those values. variableC controls the amplitude.
        extension = Mathf.Clamp((
            (Mathf.Sin(timeVariable / variableA) + variableB) * variableC)
            ,0, Mathf.Infinity) ;
    }

    public IEnumerator CalculateRateOfChange()
    {
        while (true)
        {
            float currentEx = extension;
            yield return new WaitForSecondsRealtime(updateRate);
            extensionRateOfChange = Mathf.Abs((extension - currentEx) / updateRate);
        }
    }
}
