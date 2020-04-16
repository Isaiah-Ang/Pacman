using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data;
using MySql.Data.MySqlClient;

public class ScoreManager : MonoBehaviour {

    private string username;
    private int _highscore;
    private int _lowestHigh;
    private bool _scoresRead;
    private bool _isTableFound;

    public class Score
    {
        public string name { get; set; }
        public int score { get; set; }

        public Score(string n, int s)
        {
            name = n;
            score = s;
        }

        public Score(string n, string s)
        {
            name = n;
            score = Int32.Parse(s);
        }
    }

    List<Score> scoreList = new List<Score>(10);

    void OnLevelWasLoaded(int level)
    {
        // Read scores from the database
        StartCoroutine("ReadScoresFromDB");

        _highscore = scoreList[0].score;

        if (level == 2) StartCoroutine("UpdateGUIText");    // if scores is loaded
        if (level == 1) _lowestHigh = _highscore;

        Debug.Log("Score Manager Class");
    }

    IEnumerator GetHighestScore()
    {
        Debug.Log("GETTING HIGHEST SCORE");
        // wait until scores are pulled from database
        float timeOut = Time.time + 4;
        while (!_scoresRead)
        {
            yield return new WaitForSeconds(0.01f);
            if (Time.time > timeOut)
            {
                Debug.Log("Timed out");
                //scoreList.Clear();
                //scoreList.Add(new Score("GetHighestScore:: DATABASE CONNECTION TIMED OUT", -1));
                break;
            }
        }

        // Retrieves first(highest) score in the list
        _highscore = scoreList[0].score;
        _lowestHigh = scoreList[scoreList.Count - 1].score;
    }

    // High score menu 
    IEnumerator UpdateGUIText()
    {
        // wait until scores are pulled from database
        float timeOut = Time.time + 4;

        // while(!_scoresRead){
        //     scoreList.Clear();
        //     scoreList.Add(new Score("DATABASE TEMPORARILY UNAVAILABLE", 9999999));
        // }

        GameObject.FindGameObjectWithTag("ScoresText").GetComponent<Scores>().UpdateGUIText(scoreList);
        Debug.Log("Score List: " + scoreList);
        yield return new WaitForSeconds(0f);
    }

    IEnumerator ReadScoresFromDB()
    {
        string cs = "Server=127.0.0.1;Database=pacman;Uid=root;CharSet=utf8;Password=12345678;";
        
        try{
    		MySqlConnection con = new MySqlConnection(cs);
            con.Open();

            Debug.Log("Connected to Database");

            string stmt = "SELECT * FROM highscore ORDER BY score DESC LIMIT 10";
   			MySqlCommand cmd = new MySqlCommand(stmt, con);

            MySqlDataReader rdr = cmd.ExecuteReader();

            if(rdr.HasRows == true)
            {
                while(rdr.Read()){
                    username = rdr["name"].ToString();
                    _highscore = int.Parse(rdr["score"].ToString());
                    Debug.Log(username + _highscore);
                    scoreList.Add(new Score(username, _highscore));
                }
                _scoresRead = true;
            } else
            {
                _scoresRead = false;
            } 

        } catch(Exception ex)
        {
            Debug.Log(ex);
            _scoresRead = false;

            _highscore = PlayerPrefs.GetInt("Score", 0);
            username = PlayerPrefs.GetString("Name", "ABC");
            scoreList.Add(new Score(username, _highscore));
            StartCoroutine(UpdateGUIText());
        }

        // if(_scoresRead == false)
        // {
            
        // } else;

        yield return null;

    }

    public int High()
    {
        return _highscore;
    }

    public int LowestHigh()
    {
        return _lowestHigh;
    }
}
