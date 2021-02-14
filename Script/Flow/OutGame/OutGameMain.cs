using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// アウトゲームフローメインクラス
public class OutGameMain : MonoBehaviour
{
    GameObject m_FlowObj = null;

    // フロー情報
    public struct FlowData
    {
        public  Global.FLOW_ID  flowId;
        private string          flowPrefabPath;
        public  string          sceneName;

        public FlowData(Global.FLOW_ID _flowId, string _flowPrefub, string _sceneName )
        {
            flowId              = _flowId;
            flowPrefabPath      = _flowPrefub;
            sceneName           = _sceneName;
        }

        // シーン変更するか
        public bool IsChangeScene()
        {
            if(sceneName != "" )
            {
                return true;
            }
            return false;
        }
        // フローPrefubを返す
        public GameObject GetFlowPrefab()
        {
            GameObject flowPrefab = null;
            if(flowPrefabPath != "")
            {
                flowPrefab = (GameObject)Resources.Load( flowPrefabPath );
            }
            return flowPrefab;
        }
    };
    readonly List<FlowData> s_FlowDatas = new List<FlowData>
    {
        new FlowData( Global.FLOW_ID.title,         "Flow/OutGame/TitleMain",       "" ),
        new FlowData( Global.FLOW_ID.levelSelect,   "Flow/OutGame/LevelSelectMain", "" ),
        new FlowData( Global.FLOW_ID.tuto,          "Flow/OutGame/TutoMain",        "" ),
        new FlowData( Global.FLOW_ID.battle,        "",                             "MainBattle" ),
    };

    // 遷移
    enum State
    {
        update,
        createFlow,
        end,
    };
    State m_State = State.update;

    // フロー情報を返す
    FlowData GetFlowData( Global.FLOW_ID id )
    {
        foreach( FlowData data in s_FlowDatas.ToArray() )
        {
            if( data.flowId == id )
            {
                return data;
            }
        }

        // ここに来たらエラー
        Debug.Assert( false, "指定のフロー情報はありません" );
        return s_FlowDatas[ 0 ];
    }

    private void Awake()
    {
        // ゲーム開始時はタイトルから始める
        if( GameParam.I().GetFlowId() == Global.FLOW_ID.none )
        {
            FadeMgr.I().StartFadeOut( 0 );
            GameParam.I().SetFlowId( Global.FLOW_ID.title );
        }

        // フロー生成
        FlowData flowData = GetFlowData( GameParam.I().GetFlowId() );
        m_FlowObj = Instantiate( flowData.GetFlowPrefab(), Vector3.zero, Quaternion.identity );
    }
    // 更新
    void Update()
    {
        switch(m_State)
        {
            case State.update:
                {
                    BaseFlowMain baseFlow = m_FlowObj.GetComponent<BaseFlowMain>();
                    if(false == baseFlow.IsEnd()) break;

                    FlowData nextFlowData = GetFlowData(GameParam.I().GetFlowId());
                    // シーン変更有
                    if(nextFlowData.IsChangeScene())
                    {
                        m_State = State.end;
                    }
                    else
                    {
                        // 現在再生しているフローを削除
                        Destroy(m_FlowObj);

                        m_State = State.createFlow;
                    }
                }
                break;
            case State.createFlow:
                {
                    if(m_FlowObj != null) break;

                    FlowData nextFlowData = GetFlowData(GameParam.I().GetFlowId());
                    // 次に再生するフローを生成
                    m_FlowObj = Instantiate(nextFlowData.GetFlowPrefab(), Vector3.zero, Quaternion.identity);

                    m_State = State.update;
                }
                break;
            case State.end:
                {
                    // シーン移行
                    FlowData nextFlowData = GetFlowData(GameParam.I().GetFlowId());
                    SceneManager.LoadScene(nextFlowData.sceneName);
                }
                break;
        }
    }
}
