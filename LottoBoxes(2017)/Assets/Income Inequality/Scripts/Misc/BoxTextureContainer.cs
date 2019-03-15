using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxTextureContainer : MonoBehaviour {

    #region Singleton
    private static BoxTextureContainer instance;
    public static BoxTextureContainer Instance
    {
        get
        {
            return instance;
        }
        private set { }
    }

    private BoxTextureContainer() { }

    #endregion

    [SerializeField]
    private List<Texture> boxTextures;

    // Use this for initialization
    void Start ()
    {
        instance = this;
	}

    public Texture ReceiveTexture()
    {
        return boxTextures[Random.Range(0, boxTextures.Count)];
    }

}
