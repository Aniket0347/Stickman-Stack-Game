using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stickman : MonoBehaviour
{
    public float jumpForce = 5f;
    private Rigidbody rb;
    public bool isGrounded;
    private Animator anim;
    private GameObject Stopper;
    public List<GameObject> cubes;
    public GameObject[] impact_fx;
    public bool impact = false;
    public GameObject canvas;

    
    private bool processingOnit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        Stopper = GameObject.FindWithTag("Use");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Jump();
            impact = true;
        }

        if (impact == false)
        {
            foreach (GameObject fx in impact_fx)
            {
                fx.SetActive(false);
            }
        }

        if (isGrounded == true && impact == true)
        {
            StartCoroutine(Runfx());
        }
    }

    IEnumerator Runfx()
    {
        foreach (GameObject fx in impact_fx)
        {
            fx.SetActive(true);
        }
        yield return new WaitForSeconds(0.2f);
        impact = false;
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Onit" && !processingOnit)
        {
            processingOnit = true;

            print("Onit");
            isGrounded = true;

            GameObject Hurdel = other.transform.parent.gameObject;
            cubes.Add(Hurdel);

            GameObject OnDie = GameObject.FindWithTag("Kill");
            if (OnDie != null)
                OnDie.SetActive(false);

            Hurdel.GetComponent<Move>().IsMove = false;

           
            Stack stackComp = Stopper.GetComponent<Stack>();
            if (stackComp != null && stackComp.canStack)
            {
                Transform ChangeY = stackComp.Stacker[1].transform;
                ChangeY.position = new Vector3(Hurdel.transform.position.x - 0.4f, ChangeY.position.y, ChangeY.position.z);
                stackComp.Stacked();
            }

            other.gameObject.SetActive(false);

            
            StartCoroutine(ResetOnitGuard());
        }

        if (other.tag == "Kill")
        {
            GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("Stack");
            rb.constraints = RigidbodyConstraints.None;
            foreach (GameObject c in foundObjects)
            {
                c.GetComponent<Move>().IsMove = false;
                cubes.Add(c);
            }
            SaveHighScore();
            print("died");
            rb.AddForce(-40f, 200f, 0);
            StartCoroutine(loadScene());
        }
    }

    IEnumerator ResetOnitGuard()
    {
        yield return new WaitForSeconds(0.6f);
        processingOnit = false;
    }

    public void SaveHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (canvas.GetComponent<Points>().score > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore", canvas.GetComponent<Points>().score);
            PlayerPrefs.Save();
        }
    }

    IEnumerator loadScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("SampleScene");
    }
}
