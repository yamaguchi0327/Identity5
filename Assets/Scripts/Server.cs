using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

public class Server : MonoBehaviour
{
    static WebSocketServer server;

    void Start()
    {
        DontDestroyOnLoad(this);
        server = new WebSocketServer(3000);

        server.AddWebSocketService<Echo>("/");
        server.Start();
        Debug.Log("server-start");

    }

    public void OnDestroy()
    {
        server.Stop();
        server = null;
    }

}

public class Echo : WebSocketBehavior
{
    //Server se;
    protected override void OnMessage(MessageEventArgs e)
    {
        //受け取って送る
        Sessions.Broadcast(e.Data);
        //if (e.Data.Equals("game_end"))
        //{
        //    Debug.Log("end");
        //}
    }
}

