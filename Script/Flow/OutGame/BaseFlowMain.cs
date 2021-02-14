using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アウトゲーム用 フロー基本クラス
/// </summary>
public class BaseFlowMain : MonoBehaviour
{
    // ボタン制御用
    [SerializeField] List<Button> m_UIButtonList = null;
    // 終了判定用
    protected bool m_IsEnd = false;

    /// <summary>
    /// フローが終了したか
    /// </summary>
    /// <returns>フロー終了フラグ</returns>
    public bool IsEnd()
    {
        return m_IsEnd;
    }
    /// <summary>
    /// 全ボタン無効化
    /// </summary>
    /// <param name="enable">有効か</param>
    public void SetDisableAllBtn( bool enable )
    {
        foreach(Button btn in m_UIButtonList.ToArray())
        {
            btn.enabled = enable;
        }
    }
    /// <summary>
    /// 初期化
    /// </summary>
    protected virtual void Init()
    {
    }

    void Awake()
    {
        Init();
    }
    void Update()
    {
        
    }
}
