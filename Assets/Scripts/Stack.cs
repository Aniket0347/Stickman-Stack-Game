using System.Collections;
using UnityEngine;

public class Stack : MonoBehaviour
{
    public GameObject[] Stacker;
    public bool canStack = true;
    private GameObject Player;
    public GameObject canvas;

   
    private float stacker0OriginalX;
    private float stacker1OriginalX;

    private void Start()
    {
        Player = GameObject.FindWithTag("Player");

        
        stacker0OriginalX = Stacker[0].transform.position.x;
        stacker1OriginalX = Stacker[1].transform.position.x;
    }

    public void Stacked()
    {
        if (canStack == true)
        {
            StartCoroutine(Rebot_());

            GameObject Detector_ = GameObject.FindWithTag("Onit");
            if (Detector_ != null)
                Detector_.SetActive(false);

            foreach (GameObject t in Player.GetComponent<Stickman>().cubes)
            {
                t.gameObject.tag = "Ground";
                t.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }

            Stacker[0].transform.position += new Vector3(0, 0.19f, 0);
            Stacker[1].transform.position += new Vector3(0, 0.19f, 0);

            canvas.GetComponent<Points>().score++;
            Stacker[0].GetComponent<Spawn>().Check_interval = true;
            canvas.GetComponent<Points>().Pluspoint();
        }
    }

    IEnumerator Rebot_()
    {
        canStack = false;

       
        yield return new WaitForSeconds(0.5f);

       
        Vector3 pos0 = Stacker[0].transform.position;
        Vector3 pos1 = Stacker[1].transform.position;
        Stacker[0].transform.position = new Vector3(stacker0OriginalX, pos0.y, pos0.z);
        Stacker[1].transform.position = new Vector3(stacker1OriginalX, pos1.y, pos1.z);

        canStack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Stack")
        {
            other.gameObject.GetComponent<Move>().IsMove = false;
            other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Player.GetComponent<Stickman>().cubes.Add(other.gameObject);
            Stacked();
        }
    }
}
