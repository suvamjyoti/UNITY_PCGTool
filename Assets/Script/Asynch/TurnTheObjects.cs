using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TurnTheObjects : MonoBehaviour
{

    [SerializeField] private List<GameObject> objectsList;
    [SerializeField] private float time = 100; 

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject objct in objectsList) 
        {
            rotateObject(objct);
        }
    }

    private async void rotationObject() 
    {

    }

    private async void  rotateObject(GameObject objet) 
    {
        float currentTime = 0;

        while (currentTime<time) 
        {
            currentTime += Time.deltaTime;

            objet.transform.Rotate(Vector3.up, 10f);

            await Task.Yield();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
