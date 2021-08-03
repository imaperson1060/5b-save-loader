/* 5b levelpack parser
 * Code by Ari Meisels
 * 
 * Usage:
 * List<Levelpack> levelpack = LevelpackParser.Parse("levels.txt");
 *
 * For more help, DM me at imaperson#1060 or just look at how I used it
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

public struct Levelpack
{
    public string Title;

    public int Width;
    public int Height;
    public int EntityCount;
    public int Background;
    public string BM;

    public string[] Lines;

    public List<EntityStruct> Entities;

    public List<DialogueStruct> DialogueLines;

    public int NecessaryDeaths;
}

public struct EntityStruct
{
    public int Id;
    public double X;
    public double Y;
    public int Role;

    public int Speed;
    public double Movement;
}

public struct DialogueStruct
{
    public int Id;
    public string Emotion;
    public string Dialogue;
}

class LevelReader
{
    public string loadedLevels;
    public int pos = 0;

    public LevelReader(string filename)
    {
        var ReadText = File.ReadAllText(filename, Encoding.UTF8);
        loadedLevels = HttpUtility.ParseQueryString(ReadText).Get("loadedLevels");
        /*if (HttpUtility.ParseQueryString(ReadText).Get("levelCount") != null)
        {
            levelCount = Convert.ToInt32(HttpUtility.ParseQueryString(ReadText).Get("levelCount"));
        }*/
    }

    public string ReadData(int length, bool ReadFullLine = false)
    {
        if (ReadFullLine)
        {
            string val = "";
            while (loadedLevels.Substring(pos, 1) != "\r")
            {
                val += loadedLevels.Substring(pos, 1);
                pos++;
            }
            return val;
        }
        else
        {
            string val = loadedLevels.Substring(pos, length);
            pos += length;
            return val;
        }
    }

    public int ReadInt(int length)
    {
        string val = loadedLevels.Substring(pos, length);
        pos += length;
        return Convert.ToInt32(val);
    }

    public void NewLine()
    {
        ReadData(2);
    }
}

public class LevelpackParser
{
    public static List<Levelpack> Parse(string filename, List<Levelpack> levelpack = null)
    {
        if (levelpack == null)
        {
            levelpack = new List<Levelpack>();
        }

        if (!File.Exists(filename))
        {
            Console.WriteLine(filename + " doesn't exist.");
            return levelpack;
        }

        LevelReader file = new LevelReader(filename);

        while (file.pos < file.loadedLevels.Length)
        {
            Levelpack Level = new Levelpack();

            file.NewLine(); // Padding
            Level.Title = file.ReadData(0, true);
            file.NewLine(); // Padding
            Level.Width = file.ReadInt(2);
            file.ReadData(1); // Padding
            Level.Height = file.ReadInt(2);
            file.ReadData(1); // Padding
            Level.EntityCount = file.ReadInt(2);
            file.ReadData(1); // Padding
            Level.Background = file.ReadInt(2);
            file.ReadData(1); // Padding
            Level.BM = file.ReadData(1);
            file.NewLine(); // Padding
            Level.Lines = new string[Level.Height];
            for (var i = 0; i < Level.Height; i++)
            {
                if (Level.BM == "L")
                {
                    Level.Lines[i] = file.ReadData(Level.Width);
                }
                else if (Level.BM == "H")
                {
                    Level.Lines[i] = file.ReadData(Level.Width * 2);
                }

                file.NewLine(); // Padding
            }
            Level.Entities = new List<EntityStruct>();
            for (var i = 0; i < Level.EntityCount; i++)
            {
                EntityStruct Entity = new EntityStruct();
                Entity.Id = file.ReadInt(2);
                file.ReadData(1); // Padding
                Entity.X = Convert.ToDouble(file.ReadData(5));
                file.ReadData(1); // Padding
                Entity.Y = Convert.ToDouble(file.ReadData(5));
                file.ReadData(1); // Padding
                Entity.Role = file.ReadInt(2);
                if ((Entity.Role == 3) || (Entity.Role == 4))
                {
                    Entity.Speed = file.ReadInt(2);
                    Entity.Movement = Convert.ToDouble(file.ReadData(0, true));
                }
                Level.Entities.Add(Entity);
                file.NewLine(); // Padding
            }
            var DialogueCount = file.ReadInt(2);
            file.NewLine(); // Padding
            Level.DialogueLines = new List<DialogueStruct>();
            for (var i = 0; i < DialogueCount; i++)
            {
                DialogueStruct Dialogue = new DialogueStruct();
                Dialogue.Id = file.ReadInt(2);
                Dialogue.Emotion = file.ReadData(1);
                file.ReadData(1); // Padding
                Dialogue.Dialogue = file.ReadData(0, true);
                Level.DialogueLines.Add(Dialogue);
                file.NewLine(); // Padding
            }
            Level.NecessaryDeaths = file.ReadInt(6);
            levelpack.Add(Level);

            if (file.pos < file.loadedLevels.Length)
            {
                file.NewLine(); // Padding
            }
        }

        return levelpack;
    }
}