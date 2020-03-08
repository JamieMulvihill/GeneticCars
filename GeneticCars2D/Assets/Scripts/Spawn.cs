using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawn : MonoBehaviour
{
    public GameObject newCar;
    public GameObject[] cars;
    public Vector2 bestRun;
    public Transform bestCar;
    public float delayTime;
    public int deadCars;
    public Color color;


    // Start is called before the first frame update
    void Awake()
    {
        deadCars = 0;
        delayTime = 3.5f;
        bestRun = new Vector2(0, 0);
        
        for (int i = 0; i < 10; i++) {
            cars[i] = Instantiate(newCar, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            cars[i].GetComponent<CarController>().spawner = this;
        }
    }
    private void Start()
    {
      
    }

    void Update() {

        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].transform.position.x > bestCar.transform.position.x)
            {
                bestCar = cars[i].transform;
            }

            cars[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
        }

        if (Time.time > delayTime) {
            for (int i = 0; i < cars.Length; i++) {
                if (cars[i].GetComponent<Rigidbody2D>().velocity.x < .1f) {
                    cars[i].GetComponent<CarController>().DeathCheck(Time.time);
                }
            }
        }
    }

}
