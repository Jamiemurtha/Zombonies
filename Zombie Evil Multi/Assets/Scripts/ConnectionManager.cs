using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SocketIO;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;

    public Camerafollow cameraFollowScript;

    public PointShoot pointShootScript;

    public Canvas canvas;

    public InputField playerNameInput;

    public SocketIOComponent socket;

    public GameObject player;

    public Text timerText;

    public Text escapeTimerText;

    public InputField ipText;

    public InputField portText;

    float timer = 20;

    float escapeTimer = 3;

    SocketIOEvent enemiesJSONBackup;

    public class PlayerList
    {
        public string name;

        public Transform transform;

        public PlayerList(string _name, Transform _transform)
        {
            name = _name;
            transform = _transform;
        }
    }

    public List<PlayerList> playerList = new List<PlayerList>();

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        socket.On("enemies", OnEnemies);
        socket.On("other player connected", OnOtherPlayerConnected);
        socket.On("play", OnPlay);
        socket.On("player move", OnPlayerMove);
        socket.On("player rotate", OnPlayerRotate);
        socket.On("player shoot", OnPlayerShoot);
        socket.On("other player disconnected", OnOtherPlayerDisconnected);

        ipText.text = "35.240.217.107";
        portText.text = "5055";
    }

    void Update()
    {
        if (timer <= 0 && enemiesJSONBackup != null)
        {
            StartCoroutine(SpawnEnemies());
            timer = 20;
        }
        if (enemiesJSONBackup != null)
        {
            timer -= Time.deltaTime;
            int index = timer.ToString().IndexOf(".");
            string timeRemaining = timer.ToString().Remove(index);
            timerText.text = "Enemies respawning in\n" + timeRemaining + " seconds";
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            escapeTimer -= Time.deltaTime;
            if (escapeTimer <= 0)
            {
                Application.Quit();
            }
            int index = timer.ToString().IndexOf(".");
            string timeRemaining = escapeTimer.ToString().Remove(index);
            timeRemaining = timeRemaining.Replace(".", "");
            timeRemaining = timeRemaining.Replace("-", "");
            escapeTimerText.text = "To quit hold [Escape] for\n" + timeRemaining + " seconds";
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            escapeTimer = 3;
            escapeTimerText.text = "To quit hold [Escape] for\n3 seconds";
        }
    }

    IEnumerator SpawnEnemies()
    {
        OnEnemies(enemiesJSONBackup);
        yield return null;
    }

    public void JoinGame()
    {
        socket.url = "ws://" + ipText.text + ":" + portText.text + "/socket.io/?EIO=4&transport=websocket";
        if (playerNameInput.text != "")
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().GetComponentInChildren<Text>().text = "Connecting";
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
            StartCoroutine(ConnectToServer());
        }
    }

    #region Commands
    IEnumerator ConnectToServer()
    {
        yield return new WaitForSeconds(0.5f);

        socket.Emit("player connect");

        yield return new WaitForSeconds(1);

        string playerName = playerNameInput.text;
        bool nameDuplicate = false;

        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].name == playerName)
            {
                nameDuplicate = true;
            }
        }

        while (nameDuplicate)
        {
            playerName += "_";
            bool nameStillDuplicate = false;
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].name == playerName)
                {
                    nameStillDuplicate = true;
                }
            }
            if (!nameStillDuplicate)
            {
                nameDuplicate = false;
            }
        }

        playerNameInput.text = playerName;

        List<SpawnPoint> playerSpawnPoints = GetComponent<PlayerSpawner>().playerSpawnPoint;
        List<SpawnPoint> enemySpawnPoints = GetComponent<EnemySpawner>().enemySpawnPoints;
        PlayerJSON playerJSON = new PlayerJSON(playerName , playerSpawnPoints, enemySpawnPoints);
        string data = JsonUtility.ToJson(playerJSON);
        socket.Emit("play", new JSONObject(data));
        canvas.gameObject.SetActive(false);
    }

    public void Move(Vector2 vector2)
    {
        string data = JsonUtility.ToJson(new PositionJSON(vector2));
        socket.Emit("player move", new JSONObject(data));
    }

    public void Rotate(Quaternion quaternion)
    {
        string data = JsonUtility.ToJson(new RotationJSON(quaternion));
        socket.Emit("player rotate", new JSONObject(data));
    }

    public void Shoot()
    {
        socket.Emit("player shoot");
    }

    #endregion

    #region Listeners

    void OnEnemies(SocketIOEvent socketIOEvent)
    {
        EnemiesJSON enemiesJSON = EnemiesJSON.CreateFromJSON(socketIOEvent.data.ToString());
        EnemySpawner enemySpawner = GetComponent<EnemySpawner>();
        enemySpawner.SpawnEnemies(enemiesJSON);
        enemiesJSONBackup = socketIOEvent;
    }

    void OnOtherPlayerConnected(SocketIOEvent socketIOEvent)
    {
        Debug.Log("Player joined");
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Vector2 position = new Vector2(userJSON.position[0], userJSON.position[1]);
        Quaternion rotation = Quaternion.Euler(userJSON.rotation[0], userJSON.rotation[1], userJSON.rotation[2]);
        GameObject obj = GameObject.Find(userJSON.name) as GameObject;
        if (obj != null)
        {
            return;
        }
        GameObject ply = Instantiate(player, position, rotation) as GameObject;
        Player playerScript = ply.GetComponent<Player>();
        playerScript.canControl = false;
        ply.name = userJSON.name;
        ply.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = ply.name;
        playerList.Add(new PlayerList(ply.name, ply.transform));
        timer = 20;
    }

    void OnPlay(SocketIOEvent socketIOEvent)
    {
        Debug.Log("You joined");
        string data = socketIOEvent.data.ToString();
        UserJSON currentUserJSON = UserJSON.CreateFromJSON(data);
        Vector2 position = new Vector2(currentUserJSON.position[0], currentUserJSON.position[1]);
        Quaternion rotation = Quaternion.Euler(currentUserJSON.rotation[0], currentUserJSON.rotation[1], currentUserJSON.rotation[2]);
        GameObject ply = Instantiate(player, position, rotation) as GameObject;
        Player playerScript = ply.GetComponent<Player>();
        playerScript.canControl = true;
        ply.name = currentUserJSON.name;
        cameraFollowScript.currentPlayer = ply.name;
        ScoreScript.currentPlayerName = ply.name;
        ply.transform.GetChild(1).gameObject.SetActive(true);
        pointShootScript.crosshairs = ply.transform.GetChild(1).gameObject;
        ply.transform.GetComponentInChildren<Text>().text = ply.name;
        playerList.Add(new PlayerList(ply.name, ply.transform));
    }

    void OnPlayerMove(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Vector2 position = new Vector2(userJSON.position[0], userJSON.position[1]);
        if (userJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject ply = GameObject.Find(userJSON.name) as GameObject;
        if (ply != null)
        {
            ply.transform.position = position;
        }
    }

    void OnPlayerRotate(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Quaternion rotation = Quaternion.Euler(userJSON.rotation[0], userJSON.rotation[1], userJSON.rotation[2]);
        if (userJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject ply = GameObject.Find(userJSON.name) as GameObject;
        if (ply != null)
        {
            ply.transform.rotation = rotation;
        }
    }

    void OnPlayerShoot(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        ShootJSON shootJSON = ShootJSON.CreateFromJSON(data);
        GameObject ply = GameObject.Find(shootJSON.name);
        Player playerScript = ply.GetComponent<Player>();
        playerScript.ShootWeapon(ply);
    }

    void OnOtherPlayerDisconnected(SocketIOEvent socketIOEvent)
    {
        Debug.Log("Player disconnected");
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Destroy(GameObject.Find(userJSON.name));
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].name == userJSON.name)
            {
                playerList.RemoveAt(i);
            }
        }
    }

    #endregion

    #region JSONMessageClasses

    [System.Serializable]
    public class PlayerJSON
    {
        public string name;
        public List<PointJSON> playerSpawnPoints;
        public List<PointJSON> enemySpawnPoints;

        public PlayerJSON(string _name, List<SpawnPoint> _playerSpawnPoints, List<SpawnPoint> _enemySpawnPoints)
        {
            playerSpawnPoints = new List<PointJSON>();
            enemySpawnPoints = new List<PointJSON>();
            name = _name;
            foreach (SpawnPoint playerSpawnPoint in _playerSpawnPoints)
            {
                PointJSON pointJSON = new PointJSON(playerSpawnPoint);
                playerSpawnPoints.Add(pointJSON);
            }
            foreach (SpawnPoint enemySpawnPoint in _enemySpawnPoints)
            {
                PointJSON pointJSON = new PointJSON(enemySpawnPoint);
                enemySpawnPoints.Add(pointJSON);
            }
        }
    }

    [System.Serializable]
    public class PointJSON
    {
        public float[] position;
        public float[] rotation;
        public PointJSON(SpawnPoint spawnPoint)
        {
            position = new float[]
            {
                spawnPoint.transform.position.x,
                spawnPoint.transform.position.y
            };
            rotation = new float[]
            {
                spawnPoint.transform.eulerAngles.x,
                spawnPoint.transform.eulerAngles.y,
                spawnPoint.transform.eulerAngles.z
            };
        }
    }

    [System.Serializable]
    public class PositionJSON
    {
        public float[] position;

        public PositionJSON(Vector2 _position)
        {
            position = new float[]
            {
                _position.x,
                _position.y
            };
        }
    }

    [System.Serializable]
    public class RotationJSON
    {
        public float[] rotation;

        public RotationJSON(Quaternion _rotation)
        {
            rotation = new float[]
            {
                _rotation.eulerAngles.x,
                _rotation.eulerAngles.y,
                _rotation.eulerAngles.z
            };
        }
    }

    [System.Serializable]
    public class UserJSON
    {
        public string name;
        public float[] position;
        public float[] rotation;
        
        public static UserJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UserJSON>(data);
        }
    }

    [System.Serializable]
    public class EnemiesJSON
    {
        public List<UserJSON> enemies;

        public static EnemiesJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<EnemiesJSON>(data);
        }
    }

    [System.Serializable]
    public class ShootJSON
    {
        public string name;

        public static ShootJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<ShootJSON>(data);
        }
    }

    #endregion
}
