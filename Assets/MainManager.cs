using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MongoDB.Driver;
using MongoDB;
using System.Linq;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    Dictionary<string, int> playerScoreDb = new Dictionary<string, int>();

    Dictionary<string, string> playerPasswordDb = new Dictionary<string, string>();

    string Names = "";
    string Scores = "";
    public Text myText;
    public Text myText2;
    public Text Warning;
    public Text username;
    public Text password;
    public InputField Pass;



    private void Awake()
    {
        GetHighScores();
        Pass.contentType = InputField.ContentType.Password;
    }
    async void GetHighScores()
    {
        const string connectionUri = "mongodb+srv://arda:admin@cluster0.qcgp3dk.mongodb.net/?retryWrites=true&w=majority";
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        // Create a new client and connect to the server
        var client = new MongoClient(settings);
        string databaseName = "gameDB";
        string collectionName = "Player";
        var db = client.GetDatabase(databaseName);
        var collection = db.GetCollection<Player>(collectionName);

        var results = await collection.FindAsync(_ => true);

        foreach (var result in results.ToList())
        {
            playerPasswordDb.Add(result.Name, result.Password);
            playerScoreDb.Add(result.Name, int.Parse(result.Score));
        }

        var orderedDict = from entry in playerScoreDb orderby entry.Value descending select entry;

        Dictionary<string, int> orderedPlayerScoreDb = orderedDict.ToDictionary<KeyValuePair<string, int>, string, int>(pair => pair.Key, pair => pair.Value);
        Names = "";
        Scores = "";
        int l = 1;
        foreach (string result in orderedPlayerScoreDb.Keys)
        {
            Names += l.ToString()+". "+result + "\n";
            Scores += orderedPlayerScoreDb[result] + "\n";
            l++;
        }


    }
    void Start()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) {
            OnLoginButtonPressed();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myText.text = Names;
        myText2.text = Scores;
    }

   
    public void Register()
    {
        if (username.text == "" || username.text.Length < 3)
            Warning.text = "Username must contain at least 3 characters";
        else if (playerPasswordDb.ContainsKey(username.text))
            Warning.text = "Username already taken";
        else if (Pass.text == "" || Pass.text.Length < 4)
            Warning.text = "Password must contains at least 4 characters";
        else
        {
            Player player = new Player { Name = username.text, Password = Pass.text, Score = "10000" };
            InsertToDB(player);
            Warning.text=$"Dear {username.text}, welcome to Base Casino!\nYou have 10000 Base coins, enjoy!";
        }
    }
    private async void InsertToDB(Player player)
    {
        const string connectionUri = "mongodb+srv://arda:admin@cluster0.qcgp3dk.mongodb.net/?retryWrites=true&w=majority";
        var settings = MongoClientSettings.FromConnectionString(connectionUri);

        // Create a new client and connect to the server
        var client = new MongoClient(settings);
        string databaseName = "gameDB";
        string collectionName = "Player";
        var db = client.GetDatabase(databaseName);
        var collection = db.GetCollection<Player>(collectionName);
        await collection.InsertOneAsync(player);

        var results = await collection.FindAsync(_ => true);
        playerPasswordDb.Clear();
        playerScoreDb.Clear();

        foreach (var result in results.ToList())
        {
            playerPasswordDb.Add(result.Name, result.Password);
            playerScoreDb.Add(result.Name, int.Parse(result.Score));
        }

        var orderedDict = from entry in playerScoreDb orderby entry.Value descending select entry;
        Dictionary<string, int> orderedPlayerScoreDb = orderedDict.ToDictionary<KeyValuePair<string, int>, string, int>(pair => pair.Key, pair => pair.Value);

        Names = "";
        Scores = "";
        int l = 1;
        foreach (string result in orderedPlayerScoreDb.Keys)
        {
            Names += l.ToString() + ". " + result + "\n";
            Scores += orderedPlayerScoreDb[result] + "\n";
            l++;
        }

    }
    public void OnLoginButtonPressed()
    {
        if (username.text == "")
            Warning.text = "Username can't be empty";
        else if (!playerPasswordDb.ContainsKey(username.text))
            Warning.text = "Username not found";
        else if (playerPasswordDb[username.text] == Pass.text)
        {
            Warning.text = "Login successful";
            PlayerPrefs.SetInt("score", playerScoreDb[username.text]);
            PlayerPrefs.SetString("username", username.text);
            PlayGame();
        }
            
        else
            Warning.text = "Wrong Password";
    }


    public void PlayGame()
    {
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene("Game");
    }
}
