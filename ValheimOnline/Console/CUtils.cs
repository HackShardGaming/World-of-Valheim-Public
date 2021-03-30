using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ValheimOnline.Console
{
    class CUtils
    {

        public static void Print(string text)
        {
            System.Console.WriteLine(text);
        }

        public static string CombineArgs(string[] args)
        {
            //Remove command
            args[0] = "";
            return String.Join(" ", args).Trim();
        }

        public static List<String> LoadModList(string ModPath)
        {
            List<String> ModList = new List<string>();
            if (!File.Exists(ModPath))
            {
                Debug.Log($"Creating moderator file at {ModPath}");
                string text = "# Lst each moderator Player ID (One per line)";
                File.WriteAllText(ModPath, text);
            }
            else
            {
                Debug.Log($"Loading moderator file: {ModPath}");
            }

            foreach (string text2 in File.ReadAllLines(ModPath))
            {
                if (!string.IsNullOrWhiteSpace(text2) && text2[0] != '#')
                {
                    string[] array2 = text2.Split(Array.Empty<char>());
                    ModList.Add(array2[0]);
                }
            }

            return ModList;
        }
    }
}
