using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Class used to move hand objects
/// </summary>

[System.Serializable] 

public class HandMotor : MonoBehaviour
{
    public Color[] skinColors;
    private Box myBox;
    private Vector3 startingPos;

    public Animator[] handAnimators;
    public SpriteRenderer[] handSprites;
    public SpriteRenderer slapEffect;

    public Animator slapAwayAnimator;
    public SpriteRenderer slapAwaySprite;

    private bool wasSortedCorrectly;

    public AnimationClip slapClip;

    public void Initialize(Box box, bool wasSortedRight)
    {
        //divide by two because collider width is from right edge to left edge, 
        //where we want the center to either edge, so half 
        startingPos = Vector3.zero;
        myBox = box;
        wasSortedCorrectly = wasSortedRight;

        SetPositionAndMove();
        HandsSetup();

        HandAnimation();

        slapEffect.enabled = false;
        UISlider.OnSlide += DestructRoutine;
    }

    private void HandsSetup()
    {
        int selectedSkin = Random.Range(0, skinColors.Length);
        if (wasSortedCorrectly)
        {

            for (int i = 0; i < handSprites.Length; i++)
            {
                handSprites[i].color = skinColors[selectedSkin];
            }

            for (int i = 0; i < handAnimators.Length; i++)
            {
                handAnimators[i].SetBool("Play", true);
            }

            slapAwayAnimator.gameObject.SetActive(false);
        }
        else
        {
            slapAwayAnimator.SetBool("Play", true);
            slapAwaySprite.color = skinColors[selectedSkin];

            for (int i = 0; i < handAnimators.Length; i++)
            {
                handAnimators[i].gameObject.SetActive(false);
            }
        }
    }


    //Function Returns the desired position that this object wants to go to.
    //Function also sets where this object should start at
    private void SetPositionAndMove()
    {
        //set this objects position
        float cameraWidthFromCenter = Camera.main.aspect * Camera.main.orthographicSize;

        //we are on the left
        if (transform.position.x < 0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
           
            //set position outside the screen
            startingPos = transform.position = new Vector3((cameraWidthFromCenter * -1) - 3f, myBox.transform.position.y, myBox.transform.position.z);


            //start moving
            //only goes to the box's x becuase this position is offset so that it is located in
            //the palms of the hands(children) so we don't need to offset so the hands line up with the box
            //we would if this position was in the middle of the hands
            // which would be the box's x + collider.x(far side of box reaches middle of arms)
            iTween.MoveTo(gameObject, iTween.Hash("x", myBox.gameObject.transform.position.x,
                    "easetype", iTween.EaseType.easeOutExpo, "time", .25f));
        }

        //we are on the right
        else if (transform.position.x > 0)
        {
            //set position 
            startingPos = transform.position = 
                new Vector3(cameraWidthFromCenter + 3f, myBox.transform.position.y, myBox.transform.position.z);
            
            //start moving
            iTween.MoveTo(gameObject, iTween.Hash("x", myBox.gameObject.transform.position.x,
                    "easetype", iTween.EaseType.easeOutExpo, "time", .25f));
        }       
    }

    IEnumerator SlapEffectRoutine()
    {
        yield return new WaitForSeconds(0.01f);
        slapEffect.enabled = true;

        iTween.PunchScale(slapEffect.gameObject, Vector3.one * 0.4f, 0.12f);

        yield return new WaitForSeconds(0.12f);
        slapEffect.enabled = false;
    }

    void HandAnimation()
    {
        if (wasSortedCorrectly)
        {
            for (int i = 0; i < handAnimators.Length; i++)
            {
                handAnimators[i].SetBool("Play", true);
            }
        }
        else
        {
            slapAwayAnimator.SetBool("Play", true);
        }

        StartCoroutine(WaitForAnimationTime());
    }

    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }

    private IEnumerator WaitForAnimationTime()
    {
      
        yield return new WaitForSeconds(slapClip.length / 2);

        StartCoroutine(SlapEffectRoutine());

        if (!wasSortedCorrectly)
        {
            myBox.SlapOffScreen();

            AudioManager.Instance.PlayAudioClip(SFXType.HandSlapsBox);
        }
        else if (wasSortedCorrectly)
        {
            myBox.gameObject.transform.SetParent(this.gameObject.transform);
        }

        yield return new WaitForSeconds(slapClip.length / 2);

        if (wasSortedCorrectly)
        {

            AudioManager.Instance.PlayAudioClip(SFXType.HandSlapsBox, 0.5f);
        }
        MoveOffScreen();

        yield break;
    }

    private void MoveOffScreen()
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", startingPos,
                "easetype", iTween.EaseType.easeInExpo, "time", 0.25f));
    }

    private void DestructRoutine(int levelSwitchTo)
    {
        if (levelSwitchTo == (int)MainSceneUIElements.MainGame)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        UISlider.OnSlide -= DestructRoutine;
    }
}
