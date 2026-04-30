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

    // Tracks which side the last prefab came from
    // Spawn.cs writes this before each spawn so Rebot_ resets to the correct X
    [HideInInspector] public bool lastSpawnFromRight = true;

    // These are read from Spawn.cs so Rebot_ knows what X to reset to
    // They are set automatically — no need to assign in Inspector
    [HideInInspector] public float currentStopperX0 = 0f;
    [HideInInspector] public float currentStopperX1 = 0f;

    private void Start()
    {
        Player = GameObject.FindWithTag("Player");

        // In case Stickman is on a child object
        if (Player != null && Player.GetComponent<Stickman>() == null)
            Player = Player.GetComponentInChildren<Stickman>().gameObject;

        stacker0OriginalX = Stacker[0].transform.position.x;
        stacker1OriginalX = Stacker[1].transform.position.x;

        // Default current stopper X to original positions
        currentStopperX0 = stacker0OriginalX;
        currentStopperX1 = stacker1OriginalX;
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

        // Reset X to wherever Spawn.cs last moved the stopper (left or right)
        // Y is kept so the tower height is preserved
        Vector3 pos0 = Stacker[0].transform.position;
        Vector3 pos1 = Stacker[1].transform.position;
        Stacker[0].transform.position = new Vector3(currentStopperX0, pos0.y, pos0.z);
        Stacker[1].transform.position = new Vector3(currentStopperX1, pos1.y, pos1.z);

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