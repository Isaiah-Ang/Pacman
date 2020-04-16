using UnityEngine;
using System.Collections;

public class Pacdot : MonoBehaviour {

	private PlayerController PC;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.name == "pacman")
		{
			GameManager.score += 10;
		    GameObject[] pacdots = GameObject.FindGameObjectsWithTag("pacdot");
            Destroy(gameObject);
			
			PC = GameObject.Find("pacman").GetComponent<PlayerController>();

			PC.PlayChompSound();

		    if (pacdots.Length == 1)
		    {
		        GameObject.FindObjectOfType<GameGUINavigation>().LoadLevel();
		    }
		}
	}
}
