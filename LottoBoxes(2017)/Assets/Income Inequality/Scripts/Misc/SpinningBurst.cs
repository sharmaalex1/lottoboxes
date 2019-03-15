using UnityEngine;
using System;
using System.Collections;

public class SpinningBurst : MonoBehaviour {

    private const float ROTATION_SPEED_PER_SEC = 60f;
    private Coroutine corReference;

    // Use this for initialization
    void Start ()
    {
        corReference = StartCoroutine(Rotate());
	}

    private IEnumerator Rotate()
    {
        //Can't use time.delta time because timescale is set to zero when ticket is found
        DateTime startTime = DateTime.Now;
        float startingZ = transform.eulerAngles.z;
        while (true)
        {
            //minus for clockwise
            float newZ = startingZ - (ROTATION_SPEED_PER_SEC * (float)(DateTime.Now - startTime).TotalSeconds);

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, newZ);
            yield return null;
        }
              
    }

    private void OnDestroy()
    {
        if(corReference != null)
        {
            StopCoroutine(corReference);
        }
    }

}
