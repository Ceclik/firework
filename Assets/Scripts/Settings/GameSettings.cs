using System;
using System.Reflection;
using UnityEngine;

namespace Settings
{
    //This script get all settings data from UI and write it in PlayerPrefs
    public class GameSettings
    {

        public LevelSettings[] levelSettings = new LevelSettings[4];

        public void Defaults()
        {
            for (int i = 0; i < levelSettings.Length; i++)
            {
                levelSettings[i] = LevelSettings.Default();
            }
        }

        public void SaveSettings()
        {
            for (int i = 0; i < levelSettings.Length; i++)
            {

                Type structType = typeof(LevelSettings);
                FieldInfo[] fields = structType.GetFields();
                //Debug.Log("Saving camera settings:");
                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(double) || field.FieldType == typeof(float))
                    {
                        var value = (float)field.GetValue(levelSettings[i]);
                        //Debug.Log("Field name:" + field.Name+ ", Field value:" + value.ToString());                
                        PlayerPrefs.SetFloat(field.Name+"_"+i.ToString(), value);
                    }
                    if (field.FieldType == typeof(bool))
                    {
                        //Debug.Log("Field name:" + field.Name + ", Field value:" + ((bool)field.GetValue(this)).ToString());                
                        PlayerPrefs.SetString(field.Name+"_"+i.ToString(), ((bool)field.GetValue(levelSettings[i])).ToString());
                    }
                }
            }
        }

        public void LoadSettings()
        {
            for (int a = 0; a < levelSettings.Length; a++)
            {
                Type structType = typeof(LevelSettings);
                FieldInfo[] fields = structType.GetFields();
                //Debug.Log("Loading game settings:");

                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    if (PlayerPrefs.HasKey(field.Name+"_" + a.ToString()))
                    {
                        if (field.FieldType == typeof(double) || field.FieldType == typeof(float))
                        {
                            var value = PlayerPrefs.GetFloat(field.Name+"_"+a.ToString());
                            //Debug.Log("Field name:" + field.Name + ", Field value:" + value.ToString());
                            object boxed = levelSettings[a];
                            field.SetValue(boxed, value);
                            levelSettings[a] = (LevelSettings) boxed;
                            //Debug.Log(levelSettings[a].deadTimer);
                        }
                        if (field.FieldType == typeof(bool))
                        {
                            var value = Convert.ToBoolean(PlayerPrefs.GetString(field.Name+"_"+a.ToString()));
                            //Debug.Log("Field name:" + field.Name + ", Field value:" + value.ToString());
                            field.SetValue(levelSettings[a], value);
                        }
                    }
                }
            }            
        }

        public void SetStarOne(int levelIndex, float value)
        {
            levelSettings[levelIndex].star1Time = value;
        }
        public void SetStarTwo(int levelIndex, float value)
        {
            levelSettings[levelIndex].star2Time = value;
        }
        public float GetStarOne(int levelIndex)
        {
            return levelSettings[levelIndex].star1Time;
        }
        public float GetStarTwo(int levelIndex)
        {
            return levelSettings[levelIndex].star2Time;
        }
        public void SetTimer(int levelIndex, float value)
        {
            levelSettings[levelIndex].deadTimer = value;
        }
        public float GetTimer(int levelIndex)
        {
            return levelSettings[levelIndex].deadTimer;
        }
        public LevelSettings GetSettings(int levelIndex)
        {
            return levelSettings[levelIndex];
        }
    }

    public struct LevelSettings
    {
        public float deadTimer;
        public float star2Time;
        public float star1Time;

        public static LevelSettings Default()
        {
            return new LevelSettings
            {
               deadTimer = 120.0f,
               star2Time = 120f,
               star1Time = 60f
            };
        }

    }
}
