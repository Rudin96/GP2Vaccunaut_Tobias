using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Team14
{
    public static class Settings
    {
        private static string dir => $"{Directory.GetCurrentDirectory()}\\Settings";
        private static string fileName => "settings.json";
        private static string inputFileName => "input.json";
        private static string path => Path.Combine(dir, fileName);
        private static string inputPath => Path.Combine(dir, inputFileName);
        public static void SetSettings(GameSettings settings)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(settings));
        }

        public static bool FullScreen
        {
            get
            {
                return GetSettings().Fullscreen;
            }
            set
            {
                GameSettings newSettings = GetSettings(); newSettings.Fullscreen = value; SetSettings(newSettings);
            }
        }

        public static int ResX => GetSettings().ResX;
        public static int ResY => GetSettings().ResY;

        public static void SetResolution(int width, int height)
        {
            GameSettings settings = GetSettings();
            settings.ResX = width; settings.ResY = height;
            Screen.SetResolution(width, height, settings.Fullscreen);
            SetSettings(settings);
        }

        public static void LockCursorToGame()
        {
            if(Cursor.lockState != CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.Locked;
        }

        public static void SetQualityLevel(int level)
        {
            GameSettings settings = GetSettings();
            settings.GraphicsQuality = level;
            SetSettings(settings);
        }

        public static void SetMouseSensitivity(float newSens)
        {
            GameSettings settings = GetSettings();
            settings.Sensitivity = newSens;
            SetSettings(settings);
        }

        public static void InitSettings()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(path))
                SetSettings(new GameSettings());

            SetResolution(ResX, ResY);
            FullScreen = FullScreen;
            SetQualityLevel(GetSettings().GraphicsQuality);
        }

        public static GameSettings GetSettings()
        {
            return JsonConvert.DeserializeObject<GameSettings>(File.ReadAllText(path));
        }

        public static void SaveInputToFile(string v)
        {
            File.WriteAllText(inputPath, v);
        }

        public static void ResetInputOverrides()
        {
            if (!File.Exists(inputPath))
                return;
            File.Delete(inputPath);
        }

        public static string GetInputFromFile()
        {
            if (!File.Exists(inputPath))
            {
                var stream = File.Create(inputPath);
                stream.Close();
            }
            var str = File.OpenRead(inputPath);
            if(str.Length == 0)
            {
                str.Close();
                return null;
            }
            else
            {
                str.Close();
                return File.ReadAllText(inputPath);
            }
            //else
            //    return null;
        }
    }
}
