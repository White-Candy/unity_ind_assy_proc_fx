using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ModelPartMatCol : MonoBehaviour
{
    private Material[] m_DefultMat;

    private Material m_TransparentMat;

    private MeshRenderer m_MeshRender;

    private void Awake()
    {

    }

    public void Start()
    {
        m_MeshRender = this.GetComponent<MeshRenderer>();
        m_DefultMat = m_MeshRender.materials;
        m_TransparentMat = Resources.Load<Material>("Material/TransparentMat");
    }

    /// <summary>
    /// 模型半透明
    /// </summary>
    public void Transparent()
    {
        Material[] mats = new Material[m_MeshRender.materials.Length];
        m_MeshRender.materials = mats;
        for (int i = 0; i < m_MeshRender.materials.Length; i++)
        {
            mats[i] = m_TransparentMat;
        }
        m_MeshRender.materials = mats;
    }

    /// <summary>
    /// 还原模型材质
    /// </summary>
    public void Revert()
    {
        m_MeshRender.materials = m_DefultMat;
    }

}
