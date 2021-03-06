﻿using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System.Collections.Generic;
using SimpleJSON;

public class NetworkManager : MonoBehaviour
{
    WebSocket ws;
    public static NetworkManager instance = new NetworkManager();
    public static List<string> m_Data = new List<string>();
    public string WSAddress;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
    }

    void Start()
    {

        Connect();
        StartCoroutine(RecieveData());
        //초기 불러올 디비들
    }
    void OnApplicationQuit()
    {
        Disconnect();
    }

    void Connect()
    {
        ws = new WebSocket(WSAddress, "echo-protocol");
        Debug.Log("ws : " + ws);
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
        };
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("데이터를 받았어요!");
            Debug.Log("WebSocket Message Type: " + e.Type + ", Data: " + e.Data);
            m_Data.Add(e.Data);
        };
        ws.OnError += (sender, e) =>
        {
        };
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Close");
        };

        ws.Connect();
    }

    void Disconnect()
    {
        Debug.Log("disconnect");
        ws.Close();
        ws = null;
    }

    public void SendData(string message)
    {
        Debug.Log(message);
        Debug.Log("데이터 보냈어요!");
        ws.Send(message);

        /*
		// wait until RecieveData completes
		StartCoroutine("WaitData");
		*/
    }
    /*
	IEnumerator WaitData()
	{
		yield return null;
	}
	*/

    IEnumerator RecieveData()
    {
        while (m_Data != null)
        {

            string data = "";
            foreach (string info in m_Data)
            {

                if (info != null)
                {
                    data = info;
                    m_Data.Remove(info);
                    break;
                }
            }

            if (data.Length != 0)
            {

                Debug.Log(data);

                var jsonData = JSON.Parse(data);
                int flag = jsonData["flag"].AsInt;
                //Debug.Log(flag);
                switch (flag)
                {
                    case 6:
                        LoginButtonCtrl.instance.SaveGetDataNMove(data);
                        break;
                    /*
                    case 1:
                        //InventorySetupController.instance.ForEachItem (data, false);
                        InventorySetupController.inventoryData = data;
                        break;
                    case 2:
                        break;
                    case 3:
                        //The other way to do it:
                        //TreasureSetupController obj = GameObject.FindGameObjectWithTag ("obj").GetComponent<TreasureSetupController> ();
                        //	obj.ForEachGame ();
                        // Now it's implemented using Singleton
                        TreasureSetupController.instance.ForEachGame (data);
                        // save data at static variable
                        TreasureSetupController.userGameTreasureData = data;
                        break;
                    case 7:
                        SearchButtonController.instance.SetupScrollBar (data);
                        // save data at static variable
                        SearchButtonController.searchGameData = data;
                        break;
                    */
                    
                    default:
                        CacheController.instance.DoIt(data);
                        break;
                }
                data = "";
                //StopCoroutine ("WaitData");
            }
            yield return null;
        }
    }
}
