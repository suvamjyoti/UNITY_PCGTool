using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public struct TileMetaData 
{
    public Sprite image;
    public int right;
    public int top;
    public int left;
    public int bottom;
}


public class Proc2DEditorWindow : EditorWindow
{
    //private string myText = "Hello, Unity!";

    private TileMetaData[] selectedTilesList = new TileMetaData[21];
    private string[] selectedImageDataPath = new string[21];
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
            selectedTilesList[i].image = (Sprite)EditorGUILayout.ObjectField("Tile " + (i + 1), selectedTilesList[i].image, typeof(Sprite), false);
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
            for (int i = 0; i < selectedTilesList.Length; i++)
            {
                
                if (selectedImageDataPath[i] != null)
                {
                    string tileData =   selectedImageDataPath[i] + 
                                        "@" + selectedTilesList[i].right +
                                        "@" + selectedTilesList[i].top +
                                        "@" + selectedTilesList[i].left +
                                        "@" + selectedTilesList[i].bottom + "$";

                    writer.WriteLine(tileData);
                }
            }
        }

        Debug.Log("Image data saved to: " + filePath);
    }
}


//selectedImages[i] = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
