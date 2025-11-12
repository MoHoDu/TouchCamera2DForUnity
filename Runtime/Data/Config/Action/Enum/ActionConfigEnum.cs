namespace CameraBehaviour.DataLayer.Config.Action.Enum
{
    public enum ActionMode
    {
        INSTANT,    // 즉각적 이동
        SMOOTH,     // 선형 이동
    }

    public enum ZoomType
    {
        CONTINUOUS, // 연속적 줌
        STEP,       // 단계별 줌
    }
}