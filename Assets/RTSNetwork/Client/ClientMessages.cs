using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMessages : MonoBehaviour
{
    public static bool welcomePackageReceived = false;

    public static void OnFirstConnect(SimpleObject welcomeMessage)
    {
        int myId = welcomeMessage.ReadIntRange();
        string msg = welcomeMessage.ReadStringRange();
        Client.clientID = myId;
        Debug.Log(msg + " " + myId);
        UserIdReceived();
    }

    private static void SendTcpData(SimpleObject simpleObject)
    {
        Client.tcp.SendData(simpleObject);
    }

    public static void UserIdReceived()
    {
        SimpleObject answer = new SimpleObject();
        answer.Write("welcome");
        answer.Write(Client.clientID);
        answer.Write("User id received and connected");
        SendTcpData(answer);
    }

    public static void AddGamelobby(int creator_id, string player_name, string server_name)
    {
        SimpleObject answer = new SimpleObject();
        answer.Write("addserver");
        answer.Write(creator_id);
        answer.Write(player_name);
        answer.Write(server_name);
        SendTcpData(answer);
    }

    public static void StartGamelobby(int creator_id, int secondplayer_id)
    {
        SimpleObject answer = new SimpleObject();
        answer.Write("start_game");
        answer.Write(creator_id);
        answer.Write(secondplayer_id);
        SendTcpData(answer);
    }

    public static void JoinLobby(int creator_id, int player_id, string player_name)
    {
        SimpleObject answer = new SimpleObject();
        answer.Write("joinserver");
        answer.Write(player_id);
        answer.Write(player_name);
        answer.Write(creator_id);
        SendTcpData(answer);
    }

    public static void LeaveLobby(int creator_id, int player_id)
    {
        SimpleObject answer = new SimpleObject();
        answer.Write("leaveserver");
        answer.Write(player_id);
        answer.Write(creator_id);
        SendTcpData(answer);
    }

    public static void RemoveGamelobby(int player_id)
    {
        SimpleObject answer = new SimpleObject();
        answer.Write("removeserver");
        answer.Write(player_id);
        SendTcpData(answer);
    }

    public static void ReceiveServerlist()
    {
        Debug.Log("servers angefragt");
        SimpleObject answer = new SimpleObject();
        answer.Write("receiveservers");
        answer.Write(Client.clientID);
        SendTcpData(answer);
    }

    public static void BroadCastUnitCreation(int unitID, string prefab_name, int targetserver, int sender_id,
        int receiver_id, Vector3 pos, Quaternion rota, int hp, int damage, int melee_armor, int ranged_armor)
    {
        SimpleObject answer = new SimpleObject();
        answer.Write("unit_created");
        answer.Write(sender_id);
        answer.Write(targetserver);
        answer.Write(receiver_id);
        answer.Write(prefab_name);
        answer.Write(unitID);
        answer.Write(hp);
        answer.Write(damage);
        answer.Write(melee_armor);
        answer.Write(ranged_armor);
        answer.Write(pos);
        answer.Write(rota);
        SendTcpData(answer);
    }

    public static void BroadCastUnits(int unitID, int targetserver, int sender_id,int receiver_id, Vector3 pos, Quaternion rota)
    {
        SimpleObject answer = new SimpleObject();
        answer.Write("broadcast_pos");
        answer.Write(sender_id);
        answer.Write(receiver_id);
        answer.Write(targetserver);
        answer.Write(unitID);
        answer.Write(pos);
        answer.Write(rota);
        SendTcpData(answer);
    }
}