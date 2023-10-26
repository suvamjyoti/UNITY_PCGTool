using System;

[Serializable]
public class MetaDataModel
{
    //    Top View
    //           Z  top
    //          |_______
    //          |       |
    //      left|       |right
    //          |_______|__X
    //              botttom

    public GameEnums.TileObjectName name;
    public bool left;
    public bool right;
    public bool top;
    public bool bottom;
  //  public GameEnums.SelfTilling selfTilling;               //will the mesh connect to each other withoput rotation 
  //  public GameEnums.MiroredRotation mirroredRotation;      //will the mesh connect after some rotation

    private string mainAttachmentRule;                              //the tiles connection data will be stored in it
                                                                //or eg. 1010 will become 10 in order LTRB - left, top, right, bottom

    private string quaterRotaAttachmentRule;                          //tile connection rule for 90degree rotation
    private string halfRotaAttachmentRule;                        //tile connection rulle for 180degree rotation
    private string threeFourthRotaAttachmentRule;                 //tile connection rule for 270degree rotation

    private int _joiningEdge;
    public int joiningEdge=>_joiningEdge;
    private int _nonJoiningEdge;
    public int nonJoiningEdge => _nonJoiningEdge;

    public void SetAttachmentValue() 
    {
        //set the initial AttachmentValue
        mainAttachmentRule = GetBoolToIntegerValue(left, top, right, bottom);
        //this will be BLTR after 90 rotation clockwise
        quaterRotaAttachmentRule = GetBoolToIntegerValue(bottom,left, top, right);
        //this will be LTRB after 180 rotation clockwise
        halfRotaAttachmentRule = GetBoolToIntegerValue(right, bottom, left,top);
        //TRBL after 270 rotation
        threeFourthRotaAttachmentRule = GetBoolToIntegerValue(top, right, bottom,left);

        //order in this case doesnot matter
        SetJoiningEdgeVariables(left, top, right, bottom);
    }

    public System.Collections.Generic.List<string> GetRotationValueList()
    {
        System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
        
        list.Add(mainAttachmentRule);
        list.Add(quaterRotaAttachmentRule);
        list.Add(halfRotaAttachmentRule);
        list.Add(threeFourthRotaAttachmentRule);

        return list;
    }

    //incase we need to access only one value
    public string GetAttachmentValue(GameEnums.Rotations rotation) 
    {
        switch (rotation)
        {
            case GameEnums.Rotations.NoRotation:
                return mainAttachmentRule;
            case GameEnums.Rotations.QuaterRotation:
                return quaterRotaAttachmentRule;
            case GameEnums.Rotations.HalfRotation:
                return halfRotaAttachmentRule;
            case GameEnums.Rotations.ThreeFourthRotation:
                return threeFourthRotaAttachmentRule;
            default:
                // Handle any unexpected or unhandled rotation value
                // Return a default value or throw an exception if needed
                throw new ArgumentException("Invalid rotation value");
        }
    }

    public void SetJoiningEdgeVariables(bool val1, bool val2, bool val3, bool val4)
    {
        _joiningEdge = (val1 ? 1 : 0 ) + (val2 ? 1 : 0 ) + (val3 ? 1 : 0) + (val4 ? 1 : 0);
        _nonJoiningEdge = 4 - _joiningEdge;
    }

    private string GetBoolToIntegerValue(bool val1,bool val2,bool val3,bool val4) 
    {
        //now this is a binary value in string format i.e  for eg. say "10110"
        string binaryString = string.Concat(val1?"1":"0", val2 ? "1" : "0", val3 ? "1" : "0", val4 ? "1" : "0");

        //now we need to convert it to integer
        return binaryString;
    }

    //will return the edgevalue for a tileObject wghich is rotated
    public GameEnums.EdgeType GetRotatedEdgeValue(GameEnums.Rotations forRotation, GameEnums.EdgeType forEdgeType)
    {
        switch (forRotation)
        {
            case GameEnums.Rotations.QuaterRotation:
                switch (forEdgeType)
                {
                    case GameEnums.EdgeType.Left:
                        return GameEnums.EdgeType.Bottom;

                    case GameEnums.EdgeType.Top:
                        return GameEnums.EdgeType.Left;

                    case GameEnums.EdgeType.Right:
                        return GameEnums.EdgeType.Top;

                    case GameEnums.EdgeType.Bottom:
                        return GameEnums.EdgeType.Right;

                    default:
                        // Handle the case when an invalid EdgeType is provided for QuarterRotation
                        throw new ArgumentException("Invalid EdgeType for QuarterRotation");
                }

            case GameEnums.Rotations.HalfRotation:
                switch (forEdgeType)
                {
                    case GameEnums.EdgeType.Left:
                        return GameEnums.EdgeType.Right;

                    case GameEnums.EdgeType.Top:
                        return GameEnums.EdgeType.Bottom;

                    case GameEnums.EdgeType.Right:
                        return GameEnums.EdgeType.Left;

                    case GameEnums.EdgeType.Bottom:
                        return GameEnums.EdgeType.Top;

                    default:
                        // Handle the case when an invalid EdgeType is provided for HalfRotation
                        throw new ArgumentException("Invalid EdgeType for HalfRotation");
                }

            case GameEnums.Rotations.ThreeFourthRotation:
                switch (forEdgeType)
                {
                    case GameEnums.EdgeType.Left:
                        return GameEnums.EdgeType.Top;

                    case GameEnums.EdgeType.Top:
                        return GameEnums.EdgeType.Right;

                    case GameEnums.EdgeType.Right:
                        return GameEnums.EdgeType.Bottom;

                    case GameEnums.EdgeType.Bottom:
                        return GameEnums.EdgeType.Left;

                    default:
                        // Handle the case when an invalid EdgeType is provided for ThreeFourthRotation
                        throw new ArgumentException("Invalid EdgeType for ThreeFourthRotation");
                }

            default:
                // Handle the case when an invalid Rotation value is provided
                throw new ArgumentException("Invalid Rotation value");
        }
    }

}
