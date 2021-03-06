using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

public class NakamaConnection : MonoBehaviour
{

    public Connection connection;

    public string connectIP;

    void Start()
    {
        GetNewConnection();
    }

    void Update()
    {
        if (connection != null)
        {
            ////////////////////////////
            if (connection.client != null)
            {
                Debug.Log("CLIENT: " + connection.client);
            }
            else
            {
                Debug.Log("CLIENT: the nakama connection has not established a client");
            }
            ////////////////////////////
            if (connection.session != null)
            {
                Debug.Log("SESSION: " + connection.session);
            }
            else
            {
                Debug.Log("SESSION: the nakama connection does not have an active session");
            }
            ////////////////////////////
            if (connection.socket != null)
            {
                Debug.Log("SOCKET: " + connection.socket);
                Debug.Log("SOCKET: The nakama connection socket has connected state: " + connection.socket.IsConnected);
            }
            else
            {
                Debug.Log("SESSION: the nakama connection does not have an active socket");
            }
            ////////////////////////////
        }
        else Debug.Log("The nakama connection is null");
    }


    //Get a new connection and autheticate it with the server
    public async void GetNewConnection()
    {

        connection = null;

        Debug.Log("CLIENT: Getting a new connection for the client.");
        connection = await ConnectionConstructor.GetNewConnection("New_client_connection", connectIP);
        Debug.Log("CLIENT: New connection was established.");
        Debug.Log("CONNECTION - info: " + connection.socket.IsConnected);
    }

    // public Task OpenSocket()
    // {
    //     var t = connection.socket.ConnectAsync(connection.session);
    //     return t;
    // }


    public async void TryJoinRoom(string roomQuery)
    {
        Debug.Log("CLIENT: Attempting to join match");

        //If we have not yet established a connection with GetNewConnection
        if (connection == null)
        {
            Debug.Log("Not authenticated on the server. Please authenticate the device.");
            return;
        }

        Debug.Log("Searching for match with room code");
        Debug.Log("Room code = " + roomQuery);

        IApiMatchList info = null;

        try
        {
            //List matches available to the client using the room query to match roomcode label
            //Make non case sensitive with to upper
            info = await connection.client.ListMatchesAsync(connection.session, 0, 100, 1, true, roomQuery.ToUpper(), null);
        }
        catch (Exception e)
        {
            Debug.Log("MATCH EXCEPTION: " + e);
        }


        //If we get a hit (1 is returned)
        if (info.Matches.Count() == 1)
        {
            //Join the hit (the only returned match will be at element 0)
            var match = info.Matches.ElementAt(0);

            Debug.Log("Found match with room code " + roomQuery + " , with match ID " + match.MatchId);
            Debug.Log("Joining the matched room");
            await connection.socket.JoinMatchAsync(match.MatchId);
        }

        //If we get more than one hit (duplicate room codes)
        //Exit out
        //TODO: Look at maintaining unique room codes so there can't be duplicates
        else if (info.Matches.Count() > 1)
        {
            Debug.Log("Conflict: Found multiple matches with room code " + roomQuery + " , please provide unique room code");
        }

        else
        {
            Debug.Log("No matches were found with " + roomQuery);
        }
    }


}
