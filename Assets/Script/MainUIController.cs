using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUIController : MonoBehaviour
{
    private const string  LogChannel = "MainUI Controller";


    [Header("Selection_Mode")]

    public GameObject SelectionModeParentObject;

    public Button EditModeButton;
    public Button VanillaModeButton;
    public Button LoadButton;

    [SerializeField] private GameObject LevelMatrixObject;

    #region VanillaModeVariable

    [Header("UserInterface_ VanillaMode")]

    public GameObject VanillaModeParentObject;

    public TMP_InputField V_lengthFeild;
    public TMP_InputField V_breadthFeild;

    public Toggle V_BorderToggle;

    public Toggle V_VisualisationToggle;
    public Slider V_VisualisationDelaySlider;

    public Button V_GenerateButton;
    public Button V_ResetButton;
    public Button V_SaveButton;
    public Button V_ExitButton;

    #endregion

    #region EditModeVariable

    [Header("UserInterface_ EditMode")]

    public GameObject EditModeParentObject;

    public TMP_InputField E_lengthFeild;
    public TMP_InputField E_breadthFeild;

    public Toggle E_BorderToggle;

    public Toggle E_VisualisationToggle;
    public Slider E_VisualisationDelaySlider;

    public Button E_StartButton;
    public Button E_GenerateButton;
    public Button E_ResetButton;
    public Button E_SaveButton;
    public Button E_ExitButton;

    #endregion



    private void Awake()
    {
        
    }

    private void Start()
    {
        InitialiseButtonsAction();

        EditModeParentObject.SetActive(false);
        VanillaModeParentObject.SetActive(false);
    }

    private void InitialiseButtonsAction()
    {
        EditModeButton.onClick.AddListener(onEnterEditMode);
        VanillaModeButton.onClick.AddListener(OnEnterVanillaMode);
        LoadButton.onClick.AddListener(OnClickLoadButton);

        //EditMode
        E_StartButton.onClick.AddListener(OnE_StartButtonClicked);
        E_GenerateButton.onClick.AddListener(OnE_GenerateButtonClicked);
        E_ResetButton.onClick.AddListener(OnE_ResetButtonClicked);
        E_ExitButton.onClick.AddListener(ResetToSelectionMode);
        E_SaveButton.onClick.AddListener(OnClickSaveButton);

        //VanillaMode
        V_GenerateButton.onClick.AddListener(OnV_GenerateButtonClicked);
        V_ResetButton.onClick.AddListener(OnV_ResetButtonClicked);
        V_ExitButton.onClick.AddListener(ResetToSelectionMode);
        V_SaveButton.onClick.AddListener(OnClickSaveButton);

    }

    #region VANILLA

    private void OnEnterVanillaMode()
    {
        ClearLevelMatrix();

        SelectionModeParentObject.SetActive(false);
        GlobalConfigData.GetInstance().SetToolMode(GameEnums.ToolMode.VanillaMode);

        //enable vanillaMode
        VanillaModeParentObject.SetActive(true);
    }

    private void OnV_GenerateButtonClicked()
    {
        V_GenerateButton.enabled = false;
        V_ResetButton.enabled = true;

        GlobalConfigData.GetInstance().StartVanillaGeneration();
    }

    private void OnV_ResetButtonClicked()
    {
        
        GlobalConfigData.GetInstance().ResetVanillaMode();

        V_GenerateButton.enabled = true;
        V_ResetButton.enabled = false;
    }



    #endregion

    #region EditMode


    private void onEnterEditMode()
    {
        ClearLevelMatrix();

        SelectionModeParentObject.SetActive(false);

        GlobalConfigData.GetInstance().SetToolMode(GameEnums.ToolMode.EditMode);

        //enable editMode
        EditModeParentObject.SetActive(true);
    }

    private void OnE_StartButtonClicked()
    {
        GlobalConfigData.GetInstance().StartEditMode();
    }

    private void OnE_GenerateButtonClicked()
    {
        E_GenerateButton.enabled = false;
        E_ResetButton.enabled = true;

        GlobalConfigData.GetInstance().StartEditGeneration();

    }

    private void OnE_ResetButtonClicked()
    {
        GlobalConfigData.GetInstance().ResetEditMode();

        E_GenerateButton.enabled = true;
        E_ResetButton.enabled = false;
    }

    #endregion


    #region SELECTION

    private void ResetToSelectionMode()
    {
        if(GlobalConfigData.GetInstance().currentToolMode ==GameEnums.ToolMode.VanillaMode)
        {
            OnV_ResetButtonClicked();
        }
        else
        {
            OnE_ResetButtonClicked();
        }

        VanillaModeParentObject.SetActive(false);
        EditModeParentObject.SetActive(false);

        SelectionModeParentObject.SetActive(true);

        GlobalConfigData.GetInstance().SetToolMode(GameEnums.ToolMode.None);
    }
    #endregion

    private void OnClickSaveButton()
    {
        TileManager.GetInstance().SaveLevelMatrix();
    }

    private void OnClickLoadButton()
    {
        SaveLoadSystem.LoadLevelMatrix();
    }

    private void ClearLevelMatrix()
    {
        while (LevelMatrixObject.transform.childCount > 0)
        {
            DestroyImmediate(LevelMatrixObject.transform.GetChild(0).gameObject);
        }
    }
}
