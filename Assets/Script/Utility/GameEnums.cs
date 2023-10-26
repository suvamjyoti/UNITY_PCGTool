
public class GameEnums 
{
    public enum MiroredRotation
    { 
        none,
        degree180,
        degree90
    }

    public enum SelfTilling
    {
        none,
        OnZaxis,
        OnXaxis,
        allAxis
    }

    public enum Rotations
    {
        Empty = -1,
        NoRotation = 0,
        QuaterRotation = 1,
        HalfRotation = 2,
        ThreeFourthRotation = 3
    }

    public enum TileObjectName
    {
        None = -1,
        EmptyPlain = 0, 
        CornerPlain = 1,
        UPlain = 2,
        SidePlain = 3,
        TopPlain = 4
    }

    public enum neighbourType
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum EdgeType
    {
        noEdge,
        Left,
        Right,
        Top,
        Bottom
    }

    public enum VisualisationObjectState
    {
        InActive,
        Active,
        Evaluation,
        Collapsed
    }

    public enum ToolMode
    {
        None,   //similar to null
        EditMode,
        VanillaMode
    }
}
