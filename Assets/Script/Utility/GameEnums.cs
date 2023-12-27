
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
        TileSet_0 = 0,
        TileSet_1 = 1,
        TileSet_2 = 2,
        TileSet_3 = 3,
        TileSet_4 = 4,
        TileSet_5 = 5,
        TileSet_6 = 6,
        TileSet_7 = 7,
        TileSet_8 = 8,
        TileSet_9 = 9,
        TileSet_10 = 10,
        TileSet_11 = 11,
        TileSet_12 = 12,
        TileSet_13 = 13,
        TileSet_14 = 14,
        TileSet_15 = 15,
        TileSet_16 = 16,
        TileSet_17 = 17,
        TileSet_18 = 18,
        TileSet_19 = 19,
        TileSet_20 = 20,
        TileSet_21 = 21,
        TileSet_22 = 22,
        TileSet_23 = 23
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
