using UnityEngine;
using System.Collections;

public class DialSpinner : MonoBehaviour
{
    private const float RICH_CLASS_UNLOCK_TORQUE = 66.6f;
    private const float POOR_CLASS_UNLOCK_TORQUE = 86.0f;
    private const float MINIMUM_ANGULAR_SPEED = 10.0f;

    public static event System.Action OnDialSpinComplete;

    private Rigidbody2D dialBody;

    // Use this for initialization
    void Start()
    {
        dialBody = this.GetComponent<Rigidbody2D>();

        ClassRouletteUI.OnDialSpun += PlayDialAnimationWrapper;
    }

    void OnDisable()
    {
        ClassRouletteUI.OnDialSpun -= PlayDialAnimationWrapper;
    }

    private void PlayDialAnimationWrapper()
    {
        StartCoroutine(PlayDialAnimation());
    }

    private IEnumerator PlayDialAnimation()
    {
        yield return new WaitForEndOfFrame();
        float torque = !SaveManager.Instance.UnlockedClasses.Contains(Classes.Rich) ? RICH_CLASS_UNLOCK_TORQUE : POOR_CLASS_UNLOCK_TORQUE;
        dialBody.AddTorque(torque, ForceMode2D.Impulse);

        StartCoroutine(PlayDialAudio());

        yield return new WaitForSeconds(1.0f);
        //dialBody.angularVelocity = 0;
        dialBody.angularDrag = 1.2f;

        while (dialBody.angularVelocity > MINIMUM_ANGULAR_SPEED)
        {
            yield return new WaitForSeconds(0.02f);
        }

        dialBody.angularVelocity = 0;
        yield return new WaitForSeconds(1.0f);

        SaveManager.Instance.UnlockIncomeClass(Classes.Rich);
        TriggerOnDialSpinCompleteEvent();

        yield return null;
    }

    IEnumerator PlayDialAudio()
    {
        yield return new WaitForSeconds(0.066f);
        while (dialBody.angularVelocity > MINIMUM_ANGULAR_SPEED)
        {
            AudioManager.Instance.PlayAudioClip(SFXType.Wheel);
            if(57 / dialBody.angularVelocity < 0.05f)
            {
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                yield return new WaitForSeconds(57 / dialBody.angularVelocity);
            }
        }
    }

    private void UnlockNextIncomeClass()
    {
        Classes classToUnlock = (Classes)SaveManager.Instance.UnlockedClasses.Count;
        SaveManager.Instance.UnlockedClasses.Add(classToUnlock);
    }

    private static void TriggerOnDialSpinCompleteEvent()
    {
        if (OnDialSpinComplete != null)
        {
            OnDialSpinComplete();
        }
    }
}
