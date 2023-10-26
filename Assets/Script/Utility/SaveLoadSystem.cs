using UnityEngine;
using System.IO;

public static class  SaveLoadSystem 
{

   
    //will create and save the data to disk
    public static void SaveLevelMatrixToDisk(UnitObject[,] LevelMatrix,int length, int breadth)
    {
        string SaveData = "";

        SaveData += length.ToString() + ',' + breadth.ToString() + '@';
        
        for(int i=0;i<length;i++)
        {
            for(int j=0;j<breadth;j++)
            {

                SaveData += (int)(LevelMatrix[i, j].collapsedTileValue.metaData.name);
                SaveData += ",";
                SaveData += (int)(LevelMatrix[i, j].rotationValue);
                SaveData += "|";
            }
        }

        SaveTextToFile(SaveData);
    }

    public static void SaveTextToFile(string textToSave)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "level");

        // Write the text to the file
        File.WriteAllText(filePath, textToSave);

        Debug.Log("Text saved to: " + filePath);
    }



    public static string LoadTextFromFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            string text = File.ReadAllText(filePath);
            return text;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
    }

    public static void LoadLevelMatrixFromDisk(string savedData, out int[,] levelMatrix, out int[,] rotation, out int length, out int breadth)
    {
        string[] dataParts = savedData.Split('@');
        string[] dimensions = dataParts[0].Split(',');

        length = int.Parse(dimensions[0]);
        breadth = int.Parse(dimensions[1]);

        string[] matrixData = dataParts[1].Split('|');

        levelMatrix = new int[length, breadth];
        rotation = new int[length, breadth];

        int index = 0;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < breadth; j++)
            {
                string[] values = matrixData[index].Split(',');

                levelMatrix[i, j] = int.Parse(values[0]);
                rotation[i, j] = int.Parse(values[1]);

                index++;
            }
        }
    }


    public static void LoadLevelMatrix()
    {
        string savedata = LoadTextFromFile("level");

        int length;
        int breadth;
        int[,] levelMatrix;
        int[,] rotationMatrix;

        LoadLevelMatrixFromDisk(savedata, out levelMatrix, out rotationMatrix, out length, out breadth);

        TileManager.GetInstance().waveFunctionCollapse.CreateLevelFromLoadData(length,breadth,levelMatrix,rotationMatrix);
    }

}
