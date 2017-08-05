﻿using System;
using System.IO;
using System.Reflection;
using RequestifyTF2.Api;

public class Logger
{
    private static string _mExePath = string.Empty;
    public enum Status
    {
        Code,
        Error,
        Info,
        STATUS
    }

    public static void Write(Status status, string text, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        LogWrite("[" + status + "][" + DateTime.Now + "] " + text);
        Console.ForegroundColor = ConsoleColor.White;


    }

    public static void Write(Status status, string text)
    {
        LogWrite("[" + status + "][" + DateTime.Now + "] " + text);
  
    }

    public void Write(string text)
    {
        LogWrite("[" + DateTime.Now + "] " + text);


    }
  private static void LogWrite(string logMessage)
    {
        
        if (_mExePath == "")
        {
            _mExePath = AppDomain.CurrentDomain.BaseDirectory;
        }

        try
        {
            using (StreamWriter w = File.AppendText(_mExePath + "\\" + "log.txt"))
            {
                Log(logMessage, w);
            }
        }
        catch (Exception ex)
        {
        }
    }

    private static void Log(string logMessage, TextWriter txtWriter)
    {
        try
        {
            txtWriter.WriteLine($"{logMessage}");
            Console.WriteLine(logMessage);
        }
        catch (Exception)
        {
        }
    }
    public static void Log(IRequestifyPlugin plugin, string message)
    {
        Write(Status.Info, $"{plugin.Name} => {message}");
    }

    public static void LogError(IRequestifyPlugin plugin, string message)
    {
        Write(Status.Error, $"{plugin.Name} => {message}");
    }
}

