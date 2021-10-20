using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    private TrackScript trackScript;

    private float interpolationPeriod = 1f;

    private float time;

    public int carIndex;

    // car movment functions
    public float speed;

    public float rotation;

    public float[] carSpeedHistory = new float[100];

    public float[] carRotationHistory = new float[100];

    private float car_distance;

    private float car_time;

    public float car_score;

    private int lastBoost = 0;

    public bool isAlive;

    private int carXaxis = 0;

    void Start()
    {
        carIndex = int.Parse(gameObject.name.Remove(0, 4));
        trackScript =
            GameObject.Find("Main Camera").GetComponent<TrackScript>();
        isAlive = true;
    }

    void Update()
    {
        if (isAlive)
        {
            time += Time.deltaTime;
            car_time += Time.deltaTime;

            if (time >= interpolationPeriod)
            {
                //generate speed
                if (
                    trackScript.aiSpeed[carXaxis] == 0 ||
                    Random.Range(0.00f, 100.00f) <
                    trackScript.probabilities[carXaxis]
                )
                    speed = Random.Range(2.0f, 10.0f);
                else
                {
                    speed =
                        Random
                            .Range(trackScript.aiSpeed[carXaxis] - 2,
                            trackScript.aiSpeed[carXaxis] + 2);
                    if (speed > 10.0f) speed = 10.0f;
                }
                carSpeedHistory[carXaxis] = (speed);

                //generate rotation
                if (
                    trackScript.aiRotation[carXaxis] == 0 ||
                    Random.Range(0.00f, 100.00f) <
                    trackScript.probabilities[carXaxis]
                )
                    rotation = Random.Range(-180.0f, 180.0f);
                else
                    rotation =
                        Random
                            .Range(trackScript.aiRotation[carXaxis] - 20,
                            trackScript.aiRotation[carXaxis] + 20);
                carRotationHistory[carXaxis] = (rotation);

                transform.eulerAngles = Vector3.forward * rotation;

                time = 0.0f;
                carXaxis++;
            }

            transform.position += transform.right * Time.deltaTime * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Boost")
        {
            int boostScore = int.Parse(other.gameObject.name.Remove(0, 6));
            if (boostScore > lastBoost)
            {
                lastBoost = boostScore;
                car_score += boostScore * 50;
            }
        }
        else if (other.gameObject.tag != "Player")
        {
            isAlive = false;
            car_score = calculateCarScore();

            if (car_score > trackScript.bestEverCarScore)
                Debug
                    .Log("Car " +
                    carIndex +
                    " crashed after: " +
                    car_time +
                    "s; With score: " +
                    car_score);

            trackScript.saveObjectData(gameObject.GetComponent<CarScript>());
            Destroy (gameObject);
            trackScript.checkForNewGeneration();
        }
    }

    private float calculateCarScore()
    {
        return car_score - (0.05f * car_time);
    }
}
