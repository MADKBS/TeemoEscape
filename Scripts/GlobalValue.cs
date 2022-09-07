using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue
{
    public static string Unique_ID = "Player";
    public static string HostName = "Player";
    public static bool Host = false;
    public static bool IamTeemo = true;
    public static List<PlayerList> playerlist = new List<PlayerList>();
    public static int PlayerNum = 0;
    public static float Volume = 1f;
}

public class PlayerList
{
    public string PlayerName = "";
    public bool IamTeemo = true;
}