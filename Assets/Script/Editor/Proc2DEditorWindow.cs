using UnityEditor;
using UnityEngine;


public class Proc2DEditorWindow : EditorWindow
{
    private string myText = "Hello, Unity!";
    private Texture[] selectedImages = new Texture[21];
    private Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/My Custom Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<Proc2DEditorWindow>("Proc2DWindow");
    }

    private void OnGUI()
    {
        GUILayout.Label("Image Selector", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < selectedImages.Length; i++)
        {
            selectedImages[i] = (Texture)EditorGUILayout.ObjectField("Image " + (i + 1), selectedImages[i], typeof(Texture), false);
        }

        if (GUILayout.Button("Load Images"))
        {
            LoadImages();
        }

        EditorGUILayout.EndScrollView();
    }

    private void LoadImages()
    {
        for (int i = 0; i < selectedImages.Length; i++)
        {
            if (selectedImages[i] != null)
            {
                Debug.Log("Loaded Image " + (i + 1) + ": " + selectedImages[i].name);
                // Perform any other actions with the loaded image, e.g., display in a scene.
            }
        }
    }
}
