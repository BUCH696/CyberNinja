using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuricanAmmoAdd : MonoBehaviour
{
   [SerializeField] private int HuricanCountAdd;
   [SerializeField] private GameObject ControllerHurican;


    private void OnTriggerEnter2D(Collider2D target)
    {
        if(target.gameObject.CompareTag("Player"))
        {
            if(target.transform.GetComponent<PlayerController>().Hurican + HuricanCountAdd > 5)
            {
                target.transform.GetComponent<PlayerController>().Hurican = 5;
            }else target.transform.GetComponent<PlayerController>().Hurican += HuricanCountAdd;

            Destroy(gameObject);
        }
    }
}
