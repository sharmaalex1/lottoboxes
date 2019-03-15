using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TicketSpriteContainer : MonoBehaviour {

    [SerializeField ]
    private List<Sprite> ticketSprites;

    private static TicketSpriteContainer instance;
    public static TicketSpriteContainer Instance
    {
        get
        {
            return instance;
        }
        private set { }
    }

    private TicketSpriteContainer() { }

    public Sprite ReceiveTicketSprite(int num)
    {
        num = Mathf.Clamp(num, 0, ticketSprites.Count - 1);

        if (ticketSprites[num] == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Please set the ticket sprites for the Ticket Sprite Container");
#endif
        }

        return ticketSprites[num];
    }
    


    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
