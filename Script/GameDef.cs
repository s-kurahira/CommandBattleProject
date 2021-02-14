public class Global
{
    // プレイヤー種類
    public enum PlayerId
    {
        player,
        enemy,

        num,
    };

    // コマンド種類
    public enum CommandType
    {
        punch,
        kick,

        num,
    };

    // 敵の強さ
    public enum ENEMY_LEVEL
    {
        easy,
        normal,
        hard,

        num,
    };
    // 敵の強さに応じたボタン押し成功率 最大100
    public static readonly int[] s_ENEMY_LEVEL_PUSH_SUCCSESS_RATE_100 =
    {
        20,
        50,
        90,
    };
    public static readonly int s_PUSH_SUCCSESS_RATE_MAX = 100;

    // デフォルトのBGMとSE音量
    public static readonly float s_DefaultBGMVolume     = 0.3f;
    public static readonly float s_DefaultSEVolume      = 0.7f;

    // フロー定義
    public enum FLOW_ID
    {
        none,               // 無し

        title,              // タイトル
        levelSelect,        // レベル選択
        tuto,               // 説明
        battle,             // バトル

        Num,
    };
}