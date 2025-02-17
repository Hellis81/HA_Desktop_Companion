﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text.Json;
using System.Windows;

namespace HA_Desktop_Companion.Libraries
{
    public class Configuration
    {
        private string configPath;
        private Dictionary<string, Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>> configurationData = new Dictionary<string, Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>>();

        public Configuration(string path)
        {
            configPath = path;
            if (!File.Exists(configPath))
            {
                //Try to download latest from github if fail use resources
                //Write internall backup if file did not exist 
                File.WriteAllBytes(configPath, Resource1.configuration);
                
                do
                {
                    System.Threading.Thread.Sleep(1);
                } while (!File.Exists(configPath));
            }
        }

        public bool load(){
            if (!File.Exists(configPath))
            {
                return false;
            }

            string[] text = File.ReadAllLines(configPath, System.Text.Encoding.UTF8);

            string section = ""; //ROOT
            int num = 0;
            string subSection = "";
            string category = "";

            foreach (string line in text)
            {
                if (line.Contains(':') == true)
                {
                    string[] splitedString = line.Split(':');
                    string parameter = splitedString[0].Trim();
                    string value = splitedString[1].Trim();

                    if (parameter == "")
                    {
                        continue;
                    }

                    if (splitedString.Length > 2)
                    {
                        for (int i = 2; i < splitedString.Length; i++)
                        {
                            value += ":" + splitedString[i].Trim();
                        }
                    }

                    if (value == "")
                    {
                        section = parameter;
                        num = 0;
                        category = "";
                        subSection = "";
                        continue;
                    }

                    if (parameter.Contains("- ") == true)
                    {
                        subSection = parameter.Replace("- ", "");

                        if (category == value)
                        {
                            num++;
                        }
                        else
                        {
                            if (!configurationData.ContainsKey(section) || !configurationData[section].ContainsKey(subSection) || !configurationData[section][subSection].ContainsKey(value) || configurationData[section][subSection][value].Count <= 0)
                            {
                                num = 0;
                            }
                            else
                            {
                                num = configurationData[section][subSection][value].Count;
                            }
                        }

                        category = value;
                        continue;
                    }


                    if (!configurationData.ContainsKey(section))
                    {
                        configurationData[section] = new Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>();
                    }
                    if (!configurationData[section].ContainsKey(subSection))
                    {
                        configurationData[section][subSection] = new Dictionary<string, List<Dictionary<string, string>>>();
                    }
                    if (!configurationData[section][subSection].ContainsKey(category))
                    {
                        configurationData[section][subSection][category] = new List<Dictionary<string, string>>(100);
                    }
                    if (configurationData[section][subSection][category].Count <= num)
                    {
                        configurationData[section][subSection][category].Add(new Dictionary<string, string>());
                    }

                    configurationData[section][subSection][category][num][parameter] = value.Replace("\"", "");
                }
            }

            return true;
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>> GetConfigurationData()
        {
            //MessageBox.Show(JsonSerializer.Serialize(configurationData));
            return configurationData;
        }
    }
}
