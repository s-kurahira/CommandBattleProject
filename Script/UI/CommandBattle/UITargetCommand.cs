using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// コマンド画像情報
/// </summary>
[System.Serializable]
public class CommandTexData {
    public Global.CommandType type = Global.CommandType.punch;
	public Sprite iconSprite = null;
	public Sprite baseSprite = null;
}
/// <summary>
/// ボタン押し対象コマンドUIクラス
/// </summary>
public class UITargetCommand : MonoBehaviour
{
    [SerializeField] private Global.CommandType     m_CommandType           = Global.CommandType.punch;
	[SerializeField] private Image                  m_IconImage             = null;
	[SerializeField] private Image                  m_BaseImage             = null;
    [SerializeField] private List<CommandTexData>   m_CommandTexDataList    = new List<CommandTexData>();
    [SerializeField] private RectTransform          m_RectTransform         = null;

    /// <summary>
    /// コマンド種類を設定
    /// </summary>
    /// <param name="type">コマンド種類</param>
    public void SetCommandType( Global.CommandType type )
    {
        m_CommandType = type;
        
        // 画像差し替え
        CommandTexData texData = GetCommandTexData( type );
        if( m_IconImage )
        {
            m_IconImage.sprite = null;	// 現在ついてるものは外す
			m_IconImage.sprite = texData.iconSprite;
        }
        if( m_BaseImage )
        {
            m_BaseImage.sprite = null;	// 現在ついてるものは外す
			m_BaseImage.sprite = texData.baseSprite;
        }
    }
    /// <summary>
    /// コマンド種類を返す
    /// </summary>
    /// <returns>コマンド種類</returns>
    public Global.CommandType GetCommandType()
    {
        return m_CommandType;
    }
    /// <summary>
    /// 現在の表示位置を返す
    /// </summary>
    /// <returns>現在の表示位置</returns>
    public Vector2 GetPos()
    {
        return m_RectTransform.anchoredPosition;
    }

    /// <summary>
    /// 指定したコマンドの画像情報を返す
    /// </summary>
    /// <param name="type">コマンド種類</param>
    /// <returns>コマンドの画像情報</returns>
    private CommandTexData GetCommandTexData( Global.CommandType type )
    {
        foreach( CommandTexData data in m_CommandTexDataList.ToArray() )
        {
            if( type == data.type )
            {
                return data;
            }
        }

        return null;
    }

    private void Awake()
    {
        if( m_IconImage == null )
        {
            Debug.Assert( false, "Icon用のImageがありません！");
        }
        if( m_BaseImage == null )
        {
            Debug.Assert( false, "Base用のImageがありません！");
        }
    }
}
