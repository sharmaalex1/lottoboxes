#define Testing
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CameraShake : MonoBehaviour {

    public float shakeDuration = 0;

    public float shakeStrength = 0;

    private Vector3 camOriginalPos;

    private bool isRunning;

    private Coroutine currentCoroutine;

	// Use this for initialization
	void Start ()
    {
        camOriginalPos = Camera.main.transform.position;
	
	}

#if Testing
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!isRunning)
            {
                isRunning = true;
                currentCoroutine = StartCoroutine(ShakeCamera());
            }

            else
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(ShakeCamera());
            }
        }
    }

#endif

    public void TriggerCameraShake()
    {
        if (!isRunning)
        {
            isRunning = true;
            currentCoroutine = StartCoroutine(ShakeCamera());
        }

        else
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(ShakeCamera());
        }
    }

    private IEnumerator ShakeCamera()
    {
        float currentTimer = shakeDuration;
        float currentStrength = shakeStrength;

        while(currentTimer > 0)
        {
            Camera.main.transform.position = camOriginalPos + (Vector3)(Random.insideUnitCircle * currentStrength);
            currentTimer -= Time.deltaTime;
            currentStrength = (currentTimer/shakeDuration) * shakeStrength;
            yield return null;
            
        }
        Camera.main.transform.position = camOriginalPos;
        isRunning = false;
        yield break;
    }
}
