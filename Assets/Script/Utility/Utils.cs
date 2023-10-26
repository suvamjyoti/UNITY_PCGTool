using UnityEngine;


public static class Utils
{

    //instantiate a empty object
    public static GameObject InstantiateEmptyObject(string Objectname)
    {
        GameObject newObject = new GameObject(Objectname);
        return newObject;
    }

    public static GameObject InstantiateAObject(GameObject gameObject,Vector3 location,Quaternion rotation) 
    {
        GameObject obj = Object.Instantiate(gameObject,location,rotation);

        return obj;
    }

}
