using UnityEngine;
using System.Collections;

public class Energizer : MonoBehaviour {

    private GameManager gm;
    private AudioSource audio;
    public AudioClip eat;

	// Use this for initialization
	void Start ()
	{
        audio = transform.GetComponent<AudioSource>();
	    gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if( gm == null )    Debug.Log("Energizer did not find Game Manager!");
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.name == "pacman")
        {
            audio.PlayOneShot(eat, 0.45f);
            gm.ScareGhosts();
            Destroy(gameObject);
        }
    }
}
