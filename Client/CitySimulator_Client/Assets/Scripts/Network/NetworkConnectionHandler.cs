﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System.Text;

/// <summary>
/// Name:       CitySimulator 
/// Author:     Gisu Kim A00959494
/// Date:       2017-10-02
/// Updated by: 2017-10-07
/// Updated by: 2017-10-11
/// Updated by: 2017-10-12
/// Updated by: 2017-10-27
/// Updated by: 2017-10-30
/// Updated by: 2017-11-03
/// What the superviosr should know: N/A
/// </summary>
/// 
public static class NetworkConnectionHandler
{
    public static bool socketReady = false;
    public static TcpClient socket;
    public static NetworkStream stream;
    public static StringBuilder sb = new StringBuilder();
    public static string json;
    public static string host = "127.0.0.1";
    public static int port = 13456;

    public static void ConnectToServer()
    {
        if (socketReady)
        {
            return;
        }

        //default host / port values
        string host = "127.0.0.1";
        int port = 13456;

        //		//overwrite default host / port values, if there is something in thoese boxes
        //		string h;
        //		int p;
        //
        //		if (h != "")
        //			host = h;
        //		if (p != 0)
        //		{
        //			port = p;
        //		}

        //cresate the socket
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket errror: " + e.Message);
        }
    }


    public static string ReadFromServer()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                byte[] buffer = new byte[stream.Length];
                int data = stream.Read(buffer, 0, buffer.Length);
                if (data != 0)
                {
                    string readData = Convert.ToBase64String(buffer);
                    //create json string
                    json = JsonUtility.ToJson(readData);

                }
                stream.Flush();

            }
        }

        CloseSocket();
        return json;
    }

    public static string WriteForServer(string data)
    {
        if (!socketReady)
        {
            Debug.Log("Socket not ready");
            Console.WriteLine("Socket not ready");
            ConnectToServer();
        }
        byte[] dataToSend = Encoding.ASCII.GetBytes(data);
        stream.Write(dataToSend, 0, dataToSend.Length);
        stream.Flush();

        Debug.Log("data sent");

        return ReadFromServer();
    }


    private static void OnIncomingData(string data)
    {
        //        if (data == "%NAME")
        //        {
        //            Send("&NAME| " + clientName);
        //            return;
        //        }

        //detect data and parse into an object
        DataManager.ToObject(data);


    }

    /// <summary>
    ///  check whether or not a client is connected
    /// </summary>
    private static bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Closes the socket.
    /// </summary>
    private static void CloseSocket()
    {
        if (!socketReady)
            return;

        stream.Close();
        socket.Close();
        socketReady = false;
    }
    /// <summary>
    /// Raises the application quit event.
    /// </summary>
    private static void OnApplicationQuit()
    {
        CloseSocket();
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    private static void OnDisable()
    {
        CloseSocket();
    }

}

