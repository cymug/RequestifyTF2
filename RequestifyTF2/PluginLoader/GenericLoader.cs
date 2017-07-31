﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using RequestifyTF2.Utils;

namespace RequestifyTF2.Api
{
    public static class GenericPluginLoader<T>
    {
        public static ICollection<T> LoadPlugins(string path)
        {
            string[] dllFileNames = null;

            if (Directory.Exists(path))
            {
                new AutoHotKeyChecker().Check(path);
                dllFileNames = Directory.GetFiles(path, "*.dll");
                if (dllFileNames.Length == 0)
                {
                    MessageBox.Show("No plugins found, auto downloading plugins from web.", "WARINING",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    using (var web = new WebClient())
                    {
                        web.Proxy = null;
                        web.DownloadFile("http://earrapify.com/requestifyapi/TTSPlugin.dll", path + "/TTSPlugin.dll");
                        web.DownloadFile("http://earrapify.com/requestifyapi/RequestPlugin.dll",
                            path + "/RequestPlugin.dll");
                        web.DownloadFile("http://earrapify.com/requestifyapi/MTTSPlugin.dll", path + "/MTTSPlugin.dll");
                    }
                    MessageBox.Show("Downloaded. Please restart this programm.", "( ͡° ͜ʖ ͡°)", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    Environment.Exit(0);
                }

                ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
                foreach (var dllFile in dllFileNames)
                {
                    var an = AssemblyName.GetAssemblyName(dllFile);
                    var assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                }

                var pluginType = typeof(T);
                ICollection<Type> pluginTypes = new List<Type>();
                try
                {
                    foreach (var assembly in assemblies)
                        if (assembly != null)
                        {
                            var types = assembly.GetTypes();

                            foreach (var type in types)
                                if (type.IsInterface || type.IsAbstract)
                                {
                                }
                                else
                                {
                                    if (type.GetInterface(pluginType.FullName) != null)
                                        pluginTypes.Add(type);
                                }
                        }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var sb = new StringBuilder();
                    foreach (var exSub in ex.LoaderExceptions)
                    {
                        sb.AppendLine(exSub.Message);
                        var exFileNotFound = exSub as FileNotFoundException;
                        if (exFileNotFound != null)
                            if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                            {
                                sb.AppendLine("Fusion Log:");
                                sb.AppendLine(exFileNotFound.FusionLog);
                            }
                        sb.AppendLine();
                    }
                    MessageBox.Show(sb.ToString());
                    //Display or log the error based on your application.
                }

                ICollection<T> plugins = new List<T>(pluginTypes.Count);
                foreach (var type in pluginTypes)
                {
                    var plugin = (T) Activator.CreateInstance(type);
                    plugins.Add(plugin);
                }

                return plugins;
            }

            return null;
        }
    }
}