using UnityEngine;

public class ClientMessages : MonoBehaviour
{
    public static void UserIdReceived()
    {
        using (Packet answer = new Packet((int) ClientPackets.welcomeReceived))
        {
            answer.Write("User id received and connected");
            answer.Write(Client.clientID);
            SendTcpData(answer);
        }
    }

    public static void AddGamelobby(string player_name, string server_name)
    {
        using (Packet answer = new Packet((int) ClientPackets.addServer))
        {
            answer.Write(player_name);
            answer.Write(server_name);
            SendTcpData(answer);
        }
    }

    public static void StartGamelobby(int secondplayer_id)
    {
        using (Packet answer = new Packet((int) ClientPackets.gameStarted))
        {
            answer.Write(secondplayer_id);
            SendTcpData(answer);
        }
    }

    public static void JoinLobby(int creator_id,string player_name)
    {
        using (Packet answer = new Packet((int) ClientPackets.playerJoined))
        {
            answer.Write(player_name);
            answer.Write(creator_id);
            SendTcpData(answer);
        }
    }

    public static void LeaveLobby(int creator_id)
    {
        using (Packet answer = new Packet((int) ClientPackets.playerLeft))
        {
            answer.Write(creator_id);
            SendTcpData(answer);
        }
    }

    public static void RemoveGamelobby()
    {
        using (Packet answer = new Packet((int) ClientPackets.removeServer))
        {
            SendTcpData(answer);
        }
    }

    public static void ReceiveServerlist()
    {
        using (Packet answer = new Packet((int) ClientPackets.serverlistRequested))
        {
            SendTcpData(answer);
        }
    }

    public static void BroadCastUnitCreation(int unitID, string prefab_name, int targetserver,
        int receiver_id, Vector3 pos, Quaternion rota, int hp, int damage, int melee_armor, int ranged_armor)
    {
        using (Packet answer = new Packet((int) ClientPackets.playerSpawn))
        {
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
    }
    public static void BroadCastBuildingCreation(int building_id, int targetserver,
        int receiver_id, string prefab_name, Vector3 spawnpos, Quaternion spawnrota, BuildingData data)
    {
        using (Packet answer = new Packet((int) ClientPackets.spawnBuilding))
        {
            answer.Write(targetserver);
            answer.Write(receiver_id);
            answer.Write(building_id);
            answer.Write(prefab_name);
            answer.Write(spawnpos);
            answer.Write(spawnrota);
            answer.Write(data.building_hp);
            answer.Write(data.building_dmg);
            answer.Write(data.melee_resistance);
            answer.Write(data.ranged_resistance);
            SendTcpData(answer);
        }
    }
    

    public static void BroadCastUnitPosition(int unitID, int targetserver,int receiver_id, Vector3 pos)
    {
        Debug.Log("sending player movement");
        using (Packet answer = new Packet((int) ClientPackets.playerMovement))
        {
            answer.Write(receiver_id);
            answer.Write(targetserver);
            answer.Write(unitID);
            answer.Write(pos);
            SendTcpData(answer);
        }
    }
    
    private static void SendTcpData(Packet packet)
    {
        packet.WriteLength();
        Client.tcp.SendData(packet);
    }
}