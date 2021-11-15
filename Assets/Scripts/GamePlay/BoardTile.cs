using UnityEngine;
using TMPro;
using DG.Tweening;
using DG.Tweening.Core.Enums;

public class BoardTile : MonoBehaviour
{
    public int Value => m_value;
    private SpriteRenderer m_spRender;
    private RectTransform m_rectTrans;
    [SerializeField]
    private int m_value;

    private void Awake()
    {
        m_spRender = GetComponent<SpriteRenderer>();
        m_rectTrans = GetComponent<RectTransform>();
    }
   
    public float GetSizeX () 
    {        
        return m_rectTrans.sizeDelta.x;
    }
    public void SetTile(BoardTilePreset btp) 
    {
        m_value = btp.tileValue;
        m_spRender.color = btp.color;
        
        var mr = transform.GetChild(0).GetComponent<MeshRenderer>();
        var textM = transform.GetChild(0).GetComponent<TextMeshPro>();
        
        textM.SetText(m_value.ToString());
        if (IsBlankObject())
            mr.enabled = false;
        else 
        {
            mr.enabled = true;
        }
    }
    public bool IsBlankObject() { return m_value == 0; }
}
