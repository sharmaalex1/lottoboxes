using UnityEngine;
using System.Collections;

/// <summary>
/// Spawner that handles bringing boxes into the mini game scene to be sorted.
/// </summary>
public class MiniGameSpawner : MonoBehaviour, ISpawnable
{
    // Box to spawn.
    public GameObject boxObj;

    private void OnDestroy()
    {
        MiniGameTutorial.StartMiniGame -= StartSpawning;
    }

    private void Awake()
    {
        MiniGameTutorial.StartMiniGame += StartSpawning;
    }

    // Use this for initialization
    void OnEnable()
    {      
        if (GameObject.Find("CanvasContainer").GetComponent<UISlider>().CurrentElement == MainSceneUIElements.MiniGame)
        {
            //if MiniGameTutorial Instance is null, then just start Spawning
            if (MiniGameTutorial.Instance == null)
            {
                StartCoroutine(Spawn());
            }
        }
    }

    private void StartSpawning()
    {
        StartCoroutine(Spawn());
    }


    /// <summary>
    /// Every second this function will spawn a box and set it up with a downward velocity.
    /// </summary>
    public IEnumerator Spawn()
    {
        //wait for after slide
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            yield return null;

            //spawn
            var newPos = new Vector3(transform.position.x + Random.Range(-0.3f, 0.3f), transform.position.y, transform.position.z);
            Object.Instantiate(boxObj, newPos, Quaternion.identity);
                
            //wait
            yield return new WaitForSeconds(MiniGameDifficultyManager.Instance.SpawnRate);
        }
    }
}
