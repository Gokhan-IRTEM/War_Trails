using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed;
  
    private void Update()
    {
        transform.localRotation *= Quaternion.AngleAxis(speed * Time.deltaTime , Vector3.up);
    }

    public float time;
    public float number;
    private void FixedUpdate()
    {

        time += Time.deltaTime;
        if (time < 60)
        {
            number++;
        }
        else
        {
            Debug.Log(number + "russianbias");
        }

    }

}
