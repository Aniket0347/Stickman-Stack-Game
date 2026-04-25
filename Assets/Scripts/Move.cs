using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool IsMove = true;
    public float speed;



    // Update is called once per frame
    void Update()
    {
        if (IsMove == true)
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
        }
    }



}
