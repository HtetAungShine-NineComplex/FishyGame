using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerSettings", menuName = "Game Server/Server Settings")]
public class ServerSettings : ScriptableObject
{
    [Header("Smartfox Server")]
    [Tooltip("IP address or domain name of the SmartFoxServer instance")]
    public string host = "127.0.0.1";

    [Tooltip("TCP listening port of the SmartFoxServer instance, used for TCP socket connection")]
    public int tcpPort = 9933;

    [Tooltip("Web Socket port of the SmartFoxServer instance, used for UDP communication")]
    public int webSocketPort = 8080;

    [Tooltip("Name of the SmartFoxServer Zone to join")]
    public string zone = "FishGameKirin";

    [Tooltip("Display SmartFoxServer client debug messages")]
    public bool debug = true;

    [Header("Testing")]
    public string testUserName = "";
}
