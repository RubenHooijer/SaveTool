using UnityEngine;

public class ConvertString
{
    #region Typename strings
    private const string VECTOR3NAME = "Vector3";
    private const string QUATERNIONNAME = "Quaternion";
    private const string INTNAME = "int";
    private const string FLOATNAME = "float";
    #endregion

    public static object ThisType<T>(string value, T toType) where T : System.Type
    {
        switch (toType.Name)
        {
            case VECTOR3NAME:
                return ToVector3(value);
            case QUATERNIONNAME:
                return ToQuaternion(value);
            case INTNAME:
                return (int)ToFloat(value);
            case FLOATNAME:
                return ToFloat(value);
            default:
                break;
        }

        Debug.LogError("This type is not yet supported!\n You can add a Convertion in this script, see the example");
        return null;
    }

    //Write example in this region
    #region ConvertTO functions
    /*EXAMPLE CONVERT FUNCTION
     * private static <TYPE> To<TYPE> (string text){
     * Code to turn from string to <TYPE>
     * }
     * When done you can add this function to the ThisType<T> function (don't forget to add a const string for your <TYPE> name
         */

    private static Vector3 ToVector3(string text)
    {
        string[] splitText = TrimSplit(text);
        Vector3 vector = new Vector3();

        for (int i = 0; i < splitText.Length; i++)
        {
            float nFloat = ToFloat(splitText[i]);

            switch (i)
            {
                case 0:
                    vector.x = nFloat;
                    break;
                case 1:
                    vector.y = nFloat;
                    break;
                case 2:
                    vector.z = nFloat;
                    break;
                default:
                    break;
            }
        }

        return vector;
    }

    private static Quaternion ToQuaternion(string text)
    {
        string[] splitText = TrimSplit(text);
        Quaternion quaternion = new Quaternion();

        for (int i = 0; i < splitText.Length; i++)
        {
            float nFloat = ToFloat(splitText[i]);

            switch (i)
            {
                case 0:
                    quaternion.x = nFloat;
                    break;
                case 1:
                    quaternion.y = nFloat;
                    break;
                case 2:
                    quaternion.z = nFloat;
                    break;
                case 3:
                    quaternion.w = nFloat;
                    break;
                default:
                    break;
            }
        }

        return quaternion;
    }

    private static float ToFloat(string text)
    {
        string[] splitValue = text.Split('.');
        float nFloat;
        if (splitValue.Length < 2)
        {
            nFloat = float.Parse(text);
        } else
        {
            nFloat = float.Parse(text) / Mathf.Pow(10, splitValue[1].Length);
        }

        return nFloat;
    }
    #endregion

    private static string[] TrimSplit(string text)
    {
        return text.Trim('(', ')').Split(',');
    }
}