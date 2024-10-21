using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    // this static member allows every script in the project to access these variables and anything that is public in the children. an example of this is Singleton.Global.Game.Gravity or something.
    public static Singleton Global { get; private set; }
    
    public GameManager Game { get; private set; }
    public AudioManager Audio { get; private set; }

    // use this for all random variables in the game. Keeps the backend from making multiple System.Randoms. Access this by typing Singleton.Global.Random
    public System.Random Random { get; private set; } = new System.Random();

    private void Awake()
    {
        if (Global != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Global = this;
            DontDestroyOnLoad(gameObject);
        }

        Game = GetComponentInChildren<GameManager>();
        Audio = GetComponentInChildren<AudioManager>();

        if (Game == null)
        {
            Debug.LogError("Singleton must have a child with the GameManager script.");
        }
        if (Audio == null)
        {
            Debug.LogError("Singleton must have a child with the AudioManager script.");
        }
    }
}
