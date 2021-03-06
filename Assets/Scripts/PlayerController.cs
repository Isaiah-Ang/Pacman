﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float speed = 0.4f;
    Vector2 _dest = Vector2.zero;
    Vector2 _dir = Vector2.zero;
    Vector2 _nextDir = Vector2.zero;

    [Serializable]
    public class PointSprites
    {
        public GameObject[] pointSprites;
    }

    public AudioClip chomp1;
    public AudioClip chomp2;
    public AudioClip death;
        
    public PointSprites points;

    public static int killstreak = 0;

    // script handles
    private GameGUINavigation GUINav;
    private GameManager GM;
    private ScoreManager SM;

    public Joystick joystick;

    private bool _deadPlaying = false;
    private bool playedChomp1 = false;
    private AudioSource audio;
    private AudioSource background;

    // Use this for initialization
    void Start()
    {
        audio = transform.GetComponent<AudioSource>();
        background = GameObject.Find("Game Manager").GetComponent<AudioSource>();

        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        SM = GameObject.Find("Game Manager").GetComponent<ScoreManager>();
        GUINav = GameObject.Find("UI Manager").GetComponent<GameGUINavigation>();
        _dest = transform.position;

        Debug.Log("Player Controller Class");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
        switch (GameManager.gameState)
        {
            case GameManager.GameState.Game:
                ReadInputAndMove();
                Animate();
                break;

            case GameManager.GameState.Dead:
                if (!_deadPlaying)
                    StartCoroutine("PlayDeadAnimation");
                break;
        }    
        
    }

    public void PlayChompSound(){
        if(playedChomp1){
            //Play Chomp2, set playedChomp1 to false
            audio.PlayOneShot(chomp2, 0.45f);
            playedChomp1 = false;
        }else{
            //Play Chomp1, set playedChomp1 to true
            audio.PlayOneShot(chomp1, 0.45f);
            playedChomp1 = true;
        }
    }

    IEnumerator PlayDeadAnimation()
    {
        _deadPlaying = true;
        background.Stop();
        audio.PlayOneShot(death, 0.45f);
        GetComponent<Animator>().SetBool("Die", true);
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().SetBool("Die", false);
        _deadPlaying = false;

        if (GameManager.lives <= 0)
        {
            Debug.Log("Treshold for High Score: " + SM.LowestHigh());
            if (GameManager.score >= SM.LowestHigh())
            {
                GUINav.getScoresMenu();
                Debug.Log("GetScoresMenu");
            }
            else
            {
                GUINav.H_ShowGameOverScreen();
                Debug.Log("Game Over");
            }
        }

        else
            GM.ResetScene();
    }

    void Animate()
    {
        Vector2 dir = _dest - (Vector2)transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    bool Valid(Vector2 direction)
    {
        // cast line from 'next to pacman' to pacman
        // not from directly the center of next tile but just a little further from center of next tile
        Vector2 pos = transform.position;
        direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
        RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos);
        return hit.collider.name == "pacdot" || (hit.collider == GetComponent<Collider2D>());
    }

    public void ResetDestination()
    {
        _dest = new Vector2(15f, 11f);
        GetComponent<Animator>().SetFloat("DirX", 1);
        GetComponent<Animator>().SetFloat("DirY", 0);
    }

    void ReadInputAndMove()
    {
        // move closer to destination
        Vector2 p = Vector2.MoveTowards(transform.position, _dest, speed);
        GetComponent<Rigidbody2D>().MovePosition(p);

        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        // get the next direction from keyboard
        if (Input.GetAxis("Horizontal") > 0) 
        {
            _nextDir = Vector2.right;
            Debug.Log("Key Pressed");
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            _nextDir = -Vector2.right;
            Debug.Log("Key Pressed");
        } 
        if (Input.GetAxis("Vertical") > 0) 
        {
            _nextDir = Vector2.up;
            Debug.Log("Key Pressed");
        }
        if (Input.GetAxis("Vertical") < 0) 
        {
            _nextDir = -Vector2.up;
            Debug.Log("Key Pressed");
        }
        //joystick.GetComponent<Renderer>().enabled = false;
        
        #else
        if (joystick.Horizontal == 1) 
        { 
            Debug.Log("Horizontal > 0");
            _nextDir = Vector2.right;
        } else if (joystick.Horizontal == -1) 
        { 
            Debug.Log("Horizontal < 0");
            _nextDir = -Vector2.right;
        }

        if (joystick.Vertical == 1)
        {
            Debug.Log("Vertical > 0");
            _nextDir = Vector2.up;
        } else if (joystick.Vertical == -1)
        {
            Debug.Log("Vertical < 0");
            _nextDir = -Vector2.up;
        }

        #endif
        // if pacman is in the center of a tile
        if (Vector2.Distance(_dest, transform.position) < 0.00001f)
        {
            Debug.Log("_nextDir: " + _nextDir);
            if (Valid(_nextDir))
            {
                _dest = (Vector2)transform.position + _nextDir;
                _dir = _nextDir;

                Debug.Log("Moving");
            }
            else   // if next direction is not valid
            {
                if (Valid(_dir))  // and the prev. direction is valid
                    _dest = (Vector2)transform.position + _dir;   // continue on that direction

                Debug.Log("No Movement");
                // otherwise, do nothing
            }
        }
    }

    public Vector2 getDir()
    {
        return _dir;
    }

    public void UpdateScore()
    {
        killstreak++;

        // limit killstreak at 4
        if (killstreak > 4) killstreak = 4;

        Instantiate(points.pointSprites[killstreak - 1], transform.position, Quaternion.identity);
        GameManager.score += (int)Mathf.Pow(2, killstreak) * 100;

    }
}
