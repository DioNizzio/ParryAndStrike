using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject smallCircumference;

    private GameObject smallCircumferenceClone;

    public void InstanceSmallCircumference(Vector3 worldFirstPosition){
        smallCircumferenceClone = Instantiate(smallCircumference, worldFirstPosition, Quaternion.identity);
    }

    public void DeleteSmallCircumference(){
        Destroy(smallCircumferenceClone);
    }
}
