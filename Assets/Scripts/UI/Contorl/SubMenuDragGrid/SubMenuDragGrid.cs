using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SubMenuDragGrid : MonoBehaviour
{
    public GameObject m_Tool;
    public GameObject m_Material;

    public Button m_ToolBtn;
    public Button m_MaterialBtn;
    // public Button switchButton;

    public SubMenuGrid toolSubMenu;
    public SubMenuGrid materialSubMenu;

    public Button foldButton;

    public GameObject buttonPanel;
    public GameObject toolsPanel;

    private EquStatus m_equStatus = EquStatus.TOOL;

    void Start()
    {
        SwitchLeafPanel();
        m_ToolBtn.onClick.AddListener(OnClickTool);
        m_MaterialBtn.onClick.AddListener(OnClickMaterial);
        foldButton.onClick.AddListener(() => 
        {
            toolsPanel.gameObject.SetActive(false);
            Image img = buttonPanel.GetComponent<Image>();
            UITools.SetImage(ref img, "Textures/NewUI/Training/ToolButtonBG");
            SetToolAndMaterialImg("Tools", "Material");
        });
    }

    /// <summary>
    /// 切换不同的分页
    /// </summary>
    private void SwitchLeafPanel()
    {
        // Debug.Log(m_equStatus);
        Image img = buttonPanel.GetComponent<Image>();
        UITools.SetImage(ref img, "Textures/NewUI/Training/ToolBarSplit_1");
        toolsPanel.gameObject.SetActive(true);

        if (m_equStatus == EquStatus.TOOL)
        {
            m_Tool.gameObject.SetActive(true);
            m_Material.gameObject.SetActive(false);
        }
        else
        {
            m_Tool.gameObject.SetActive(false);
            m_Material.gameObject.SetActive(true);
        }
    }

    private void OnClickTool()
    {
        SetToolAndMaterialImg("ToolsSelected", "Material");
        m_equStatus = EquStatus.TOOL;
        SwitchLeafPanel();
    }

    private void OnClickMaterial()
    {
        SetToolAndMaterialImg("Tools", "MaterialSelected");
        m_equStatus = EquStatus.MATERIAL;
        SwitchLeafPanel();
    }

    private void SetToolAndMaterialImg(string toolImgName, string materialImgName)
    {
        Image toolImg = m_ToolBtn.GetComponent<Image>();
        Image materialImg = m_MaterialBtn.GetComponent<Image>();
        UITools.SetImage(ref toolImg, $"Textures/NewUI/Training/{toolImgName}");
        UITools.SetImage(ref materialImg, $"Textures/NewUI/Training/{materialImgName}");
    }

    // 表示目前显示的是 工具窗口还是材料窗口
    public enum EquStatus
    {
        TOOL,
        MATERIAL,
        NONE
    }
}
