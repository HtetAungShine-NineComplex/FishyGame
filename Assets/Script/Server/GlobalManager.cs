using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Util;
using UnityEngine.Events;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;

public class GlobalManager : MonoBehaviour
{
    public UnityAction Connection;
    public UnityAction ConnectionLost;
    public UnityAction Login;
    public UnityAction JoinedRoom;
    public UnityAction<BaseEvent> ExtensionResponse;

    [SerializeField] private ServerSettings _serverSettings;

    public static GlobalManager Instance;

    private SmartFox _sfs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if (_sfs != null)
            _sfs.ProcessEvents();
    }

    private void OnDestroy()
    {
        RemoveEventListeners();
        Instance = null;
    }

    private void OnApplicationQuit()
    {
        // Disconnect from SmartFoxServer if a connection is active
        // This is required because an active socket connection during the application quit process can cause a crash on some platforms
        if (_sfs != null && _sfs.IsConnected)
        {
            RemoveEventListeners();
            _sfs.Disconnect();
        }
    }

    public void Connect()
    {
        if (GetSfsClient() != null && GetSfsClient().IsConnected)
        {
            return;
        }
        Debug.Log("Connecting to server");

        // Set connection parameters
        ConfigData cfg = new ConfigData();
        cfg.Host = _serverSettings.host;
        cfg.Port = _serverSettings.tcpPort;
        cfg.UdpHost = _serverSettings.host;
        cfg.Zone = _serverSettings.zone;
        cfg.Debug = _serverSettings.debug;

        CreateSfsClient();
        _sfs.Logger.EnableConsoleTrace = _serverSettings.debug;

        AddEventListeners();

        _sfs.Connect(cfg);
    }

    private void AddEventListeners()
    {
        if (_sfs == null)
        {
            return;
        }

        _sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        _sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        _sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        _sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        _sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        _sfs.AddEventListener(SFSEvent.UDP_INIT, OnUdpInit);
    }

    private void RemoveEventListeners()
    {
        if (_sfs == null)
        {
            return;
        }

        _sfs.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);
        _sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        _sfs.RemoveEventListener(SFSEvent.LOGIN, OnLogin);
        _sfs.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        _sfs.RemoveEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        _sfs.RemoveEventListener(SFSEvent.UDP_INIT, OnUdpInit);
    }

    public SmartFox CreateSfsClient()
    {
        _sfs = new SmartFox();
        return _sfs;
    }

    public SmartFox CreateSfsClient(UseWebSocket useWebSocket)
    {
        _sfs = new SmartFox(useWebSocket);

        return _sfs;
    }

    public SmartFox GetSfsClient()
    {
        return _sfs;
    }

    private void OnConnection(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            Debug.Log("SFS2X API version: " + _sfs.Version);
            Debug.Log("Connection mode is: " + _sfs.ConnectionMode);
            Connection?.Invoke();
            // Login
            _sfs.Send(new LoginRequest(_serverSettings.testUserName));
        }

    }

    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove CONNECTION_LOST listener
        _sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        _sfs = null;

        // Get disconnection reason
        string connLostReason = (string)evt.Params["reason"];

        Debug.Log("Connection to SmartFoxServer lost; reason is: " + connLostReason);

        ConnectionLost?.Invoke();
        RemoveEventListeners();
        /*if (SceneManager.GetActiveScene().name != "Login")
        {
            if (connLostReason != ClientDisconnectionReason.MANUAL)
            {

                *//*if (connLostReason == ClientDisconnectionReason.IDLE)
                    connLostMsg += "It looks like you have been idle for too much time.";
                else if (connLostReason == ClientDisconnectionReason.KICK)
                    connLostMsg += "You have been kicked by an administrator or moderator.";
                else if (connLostReason == ClientDisconnectionReason.BAN)
                    connLostMsg += "You have been banned by an administrator or moderator.";
                else
                    connLostMsg += "The reason of the disconnection is unknown.";*//*
            }

            // Switch to the LOGIN scene
        }*/
    }

    private void OnLogin(BaseEvent evt)
    {
        Debug.Log("Logged in with username : " + _sfs.MySelf.Name);
        //Login?.Invoke();
        _sfs.InitUDP();
    }

    private void OnLoginError(BaseEvent evt)
    {
        _sfs.Disconnect();
    }

    private void OnUdpInit(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            Login?.Invoke();
        }
        else
        {
            // Disconnect
            // NOTE: this causes a CONNECTION_LOST event with reason "manual", which in turn removes all SFS listeners
            _sfs.Disconnect();
        }
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        ExtensionResponse?.Invoke(evt);
    }

    private void OnRoomJoin(BaseEvent evt)
    {
        Debug.Log("Successfully joined room");
        JoinedRoom?.Invoke();
    }

    public void RequestJoinRoom(string gameRoom)
    {
        SFSObject data = new SFSObject();
        data.PutUtfString("requestRoomType", gameRoom);
        SendToExtension("joinRoom", data);
    }

    public void RequestJoinRoom(string gameRoom, SlotConfiguration slotConfig = SlotConfiguration.KRAKENQUEEN)
    {
        SFSObject data = new SFSObject();
        data.PutUtfString("requestRoomType", gameRoom);

        if (gameRoom == "slot")
        {
            data.PutUtfString("reelConfiguration", slotConfig.ToString());
        }
        SendToExtension("joinRoom", data);
    }

    public void SendToExtension(string eventname, SFSObject data)
    {
        _sfs.Send(new ExtensionRequest(eventname, data));
    }

    public void SendToCurrentRoom(string eventname, SFSObject data)
    {
        _sfs.Send(new ExtensionRequest(eventname, data, _sfs.LastJoinedRoom));
    }
}
