using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Finish : MonoBehaviour
{
    Transform Target;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.transform.tag == "Player")
        {
            if(target.transform.GetComponent<PlayerController>().playerLose != true)
            target.transform.GetComponent<PlayerController>().playerWin = true;
        }
    }
}

