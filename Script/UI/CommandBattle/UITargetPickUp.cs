using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトル用ボタン押し目標強調UIクラス
/// </summary>
public class UITargetPickUp : MonoBehaviour
{
    [SerializeField] private RectTransform m_RectTransform = null;

    /// <summary>
    /// 表示開始
    /// </summary>
    /// <param name="pos">表示位置</param>
    public void StartView( Vector2 pos )
    {
        m_RectTransform.gameObject.SetActive( true );
        m_RectTransform.anchoredPosition = pos;
    }
    /// <summary>
    /// 表示終了
    /// </summary>
    public void StopView()
    {
        m_RectTransform.gameObject.SetActive( false );
    }
}
