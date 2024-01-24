using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public struct TileMetaData 
{
    public GameEnums.TileObjectName tileName;
    public Texture image;
    public int right;
    public int top;
    public int left;
    public int bottom;
}


public class Proc2DEditorWindow : EditorWindow
{
    //private string myText = "Hello, Unity!";

    private TileMetaData[] selectedTilesList = new TileMetaData[24];
    private string[] selectedImageDataPath = new string[24];
    private Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/Proc2DWindow")]
    public static void ShowWindow()
    {
        GetWindow<Proc2DEditorWindow>("Proc2DWindow");
    }

    private void OnGUI()
    {
        GUILayout.Label("Image Selector", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < selectedTilesList.Length; i++)
        {
            selectedTilesList[i].tileName = (GameEnums.TileObjectName)EditorGUILayout.EnumPopup("Select Option:", selectedTilesList[i].tileName);
            selectedTilesList[i].image = (Texture)EditorGUILayout.ObjectField("Tile " + (i), selectedTilesList[i].image, typeof(Texture), false);
            selectedTilesList[i].right = (int)EditorGUILayout.IntField("Right", selectedTilesList[i].right);
            selectedTilesList[i].top = (int)EditorGUILayout.IntField("Top", selectedTilesList[i].top);
            selectedTilesList[i].left = (int)EditorGUILayout.IntField("Left", selectedTilesList[i].left);
            selectedTilesList[i].bottom = (int)EditorGUILayout.IntField("Bottom", selectedTilesList[i].bottom);


            EditorGUILayout.Space();
            // Add a visible line separator after each loop iteration
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Handles.color = Color.grey; // Set the color of the line
            Handles.DrawLine(new Vector2(lastRect.x, lastRect.yMax), new Vector2(lastRect.xMax, lastRect.yMax));
            //Handles.color = Color.black; // Reset color to default
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("CreateDataModel"))
        {
            CreateDataModel();
        }

        if(GUILayout.Button("LoadDataModel"))
        {
            LoadDataModel();
        }

        if (GUILayout.Button("CreateTileSetAndOtherAssets "))
        {
            //CreateAssets();
        }

        EditorGUILayout.EndScrollView();
    }



    //TODO: This will be responsible for creation of Scriptable object which will be used 
    //to create levels
    private void CreateDataModel()
    {

        for(int i = 0; i < selectedTilesList.Length; i++)
        {
            if (selectedTilesList[i].image != null)
            {
                selectedImageDataPath[i] = AssetDatabase.GetAssetPath(selectedTilesList[i].image);
            }
            else
            {
                selectedImageDataPath[i] = null;

            }

        }


        // Create or get the path to the text file
        string filePath = EditorUtility.SaveFilePanel("Save Tile Data", "", "TileData", "txt");

        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogWarning("File save operation cancelled.");
            return;
        }

        // Open a stream writer to write data into the text file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            string tileData = "";


            for (int i = 0; i < selectedTilesList.Length; i++)
            {
                
                if (selectedImageDataPath[i] != null)
                {
                    tileData +=               selectedTilesList[i].tileName +
                                        "@" + selectedImageDataPath[i]      + 
                                        "@" + selectedTilesList[i].right    +
                                        "@" + selectedTilesList[i].top      +
                                        "@" + selectedTilesList[i].left     +
                                        "@" + selectedTilesList[i].bottom   +   "$";
                }
            }

            writer.WriteLine(tileData);
        }

        Debug.Log("Image data saved to: " + filePath);
    }

    private void LoadDataModel()
    {
        // Open a file dialog to select the text file
        string filePath = EditorUtility.OpenFilePanel("Load Tile Data", "", "txt");

        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogWarning("File open operation cancelled.");
            return;
        }

        // Read the file and load tile data
        using (StreamReader reader = new StreamReader(filePath))
        {
            // Clear existing data before loading new data
            ClearSelectedTilesList();

            while (!reader.EndOfStream)
            {
                string tileData = reader.ReadLine();

                // Split the tileData using the specified delimiter
                string[] dataParts = tileData.Split('$');

                int i = 0;

                foreach(string data in dataParts)
                {
                    string[] actualData = data.Split('@');

                    if (actualData.Length == 6) // Ensure all parts are present
                    {
                        GameEnums.TileObjectName tileName = (GameEnums.TileObjectName)Enum.Parse(typeof(GameEnums.TileObjectName), actualData[0]);
                        string imagePath = actualData[1];
                        int right = int.Parse(actualData[2]);
                        int top = int.Parse(actualData[3]);
                        int left = int.Parse(actualData[4]);
                        int bottom = int.Parse(actualData[5]);

                        // Load the texture based on the asset path
                        Texture image = AssetDatabase.LoadAssetAtPath<Texture>(imagePath);

                        // Add the loaded data to your selectedTilesList
                        AddTileData(tileName,image, right, top, left, bottom,i);

                        i++;
                    }
                    else
                    {
                        Debug.LogWarning("Invalid data format: " + tileData);
                    }

                }

               
            }

            PopulateTileDataInWindow();
        }

        Debug.Log("Tile data loaded from: " + filePath);
    }

    private void ClearSelectedTilesList()
    {
        // Clear existing data before loading new data
        for (int i = 0; i < selectedTilesList.Length; i++)
        {
            selectedTilesList[i].image = null;
            selectedTilesList[i].right = 0;
            selectedTilesList[i].top = 0;
            selectedTilesList[i].left = 0;
            selectedTilesList[i].bottom = 0;
        }
    }

    private void AddTileData(GameEnums.TileObjectName tileName,Texture image, int right, int top, int left, int bottom,int index)
    {
        // Add the loaded data to your selectedTilesList

            if (selectedTilesList[index].image == null)
            {
                selectedTilesList[index].tileName = tileName;
                selectedTilesList[index].image = image;
                selectedTilesList[index].right = right;
                selectedTilesList[index].top = top;
                selectedTilesList[index].left = left;
                selectedTilesList[index].bottom = bottom;
            }
    }


    private void PopulateTileDataInWindow()
    {
        for (int i = 0; i < selectedTilesList.Length; i++)
        {
            if (selectedTilesList[i].image != null)
            {
                selectedTilesList[i].tileName = (GameEnums.TileObjectName)EditorGUILayout.EnumPopup("Select Option:", selectedTilesList[i].tileName);
                selectedTilesList[i].image = (Texture)EditorGUILayout.ObjectField("Tile " + (i + 1), selectedTilesList[i].image, typeof(Texture), false);
                selectedTilesList[i].right = (int)EditorGUILayout.IntField("Right", selectedTilesList[i].right);
                selectedTilesList[i].top = (int)EditorGUILayout.IntField("Top", selectedTilesList[i].top);
                selectedTilesList[i].left = (int)EditorGUILayout.IntField("Left", selectedTilesList[i].left);
                selectedTilesList[i].bottom = (int)EditorGUILayout.IntField("Bottom", selectedTilesList[i].bottom);
            }

            EditorGUILayout.Space();
            // Add a visible line separator after each loop iteration
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Handles.color = Color.grey; // Set the color of the line
            Handles.DrawLine(new Vector2(lastRect.x, lastRect.yMax), new Vector2(lastRect.xMax, lastRect.yMax));
            //Handles.color = Color.black; // Reset color to default
            EditorGUILayout.Space();
        }
    }
}




//selectedImages[i] = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
