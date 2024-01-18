using UnityEditor;
using UnityEngine;

public struct TileMetaData 
{
    public Texture Image;
    public int right;
    public int top;
    public int left;
    public int bottom;
}


public class Proc2DEditorWindow : EditorWindow
{
    //private string myText = "Hello, Unity!";

    private TileMetaData[] selectedTilesList = new TileMetaData[21];
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
            selectedTilesList[i].Image = (Texture)EditorGUILayout.ObjectField("Image " + (i + 1), selectedTilesList[i].Image, typeof(Texture), false);
            selectedTilesList[i].right = (int)EditorGUILayout.IntField("Right", selectedTilesList[i].right);
            selectedTilesList[i].top = (int)EditorGUILayout.IntField("top", selectedTilesList[i].top);
            selectedTilesList[i].left = (int)EditorGUILayout.IntField("top", selectedTilesList[i].left);
            selectedTilesList[i].bottom = (int)EditorGUILayout.IntField("top", selectedTilesList[i].bottom);
        }

        if (GUILayout.Button("Load Images"))
        {
            LoadImages();
        }

        EditorGUILayout.EndScrollView();
    }

    //TODO: This will be responsible for creation of Scriptable object which will be used 
    //to create levels
    private void LoadImages()
    {
        for (int i = 0; i < selectedTilesList.Length; i++)
        {
            if (selectedTilesList[i] != null)
            {
                Debug.Log("Loaded Image " + (i + 1) + ": " + selectedTilesList[i].name);
                // Perform any other actions with the loaded image, e.g., display in a scene.
            }
        }
    }
}
