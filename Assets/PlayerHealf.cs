using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealf : MonoBehaviour
{
    public GameObject Player;

    public List<GameObject> Healf;
    public List<GameObject> Hurican;
    public int CurentHealf;
    public int CurentHurican;
    public int h;



    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        CurentHealf = Healf.Count;
        CurentHurican = Hurican.Count;
    }


    void Update()
    {
        if(CurentHealf != Player.GetComponent<PlayerController>().HealfPoint)
        {
            CurentHealf = Player.GetComponent<PlayerController>().HealfPoint; 
            MinusHP();
        }

        if (CurentHurican != Player.GetComponent<PlayerController>().Hurican)
        {
            CurentHurican = Player.GetComponent<PlayerController>().Hurican;
            HuricanUpdate();
        }



    }

    void MinusHP()
    {
        Healf[Healf.Count - 1].gameObject.SetActive(false);
        Healf.Remove(Healf[Healf.Count - 1]);
    }

    public void HuricanUpdate()
    {
        for (int i = 0; i <= 4; i++)
        {
            if(i <= CurentHurican - 1)
            {
                Hurican[i].gameObject.SetActive(true);
            }
            else Hurican[i].gameObject.SetActive(false);

        }
    }

}
