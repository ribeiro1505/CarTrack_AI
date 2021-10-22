using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScript : MonoBehaviour
{
    public float generationNumber;

    public float numberOfCars;

    public GameObject carPrefab;

    public GameObject[] carInstances = new GameObject[100];

    public CarScript[] carData = new CarScript[100];

    public float[] probabilities = new float[100];

    // general functions
    public float bestEverCarScore = 0;

    public float bestGenCarScore = 0;

    public float[] aiSpeed = new float[999];

    public float[] aiRotation = new float[999];

    void Start()
    {
        generationNumber = 0;
        startGeneration();
    }

    public void startGeneration()
    {
        generationNumber++;

        for (int i = 0; i < numberOfCars; i++)
        {
            GameObject currentCar = (GameObject) Instantiate(carPrefab);
            currentCar.name = string.Format("Car_" + i);
            carInstances[i] = currentCar;
        }
    }

    public void checkForNewGeneration()
    {
        bool stillCarsAlive = false;
        for (int i = 0; i < numberOfCars; i++)
        {
            if (carInstances[i] != null)
            {
                if (carInstances[i].GetComponent<CarScript>().isAlive)
                {
                    stillCarsAlive = true;
                }
            }
            else
            {
                if (carData[i].isAlive)
                {
                    stillCarsAlive = true;
                }
            }
        }

        if (!stillCarsAlive)
        {
            loadNewData();
            startGeneration();
        }
    }

    public void saveObjectData(CarScript carScript)
    {
        carData[carScript.carIndex] = carScript;
    }

    private void loadNewData()
    {
        int bestCarIndex = bestScoreCar();

        for (int i = 0; i < probabilities.Length; i++)
        {
            if (bestGenCarScore > bestEverCarScore)
            {
                aiSpeed[i] = carData[bestCarIndex].carSpeedHistory[i];
                aiRotation[i] = carData[bestCarIndex].carRotationHistory[i];
            }
            if (aiSpeed[i] == 0 || probabilities[i] == 0)
            {
                probabilities[i] = 100;
            }
            else
            {
                probabilities[i] *= 0.99f;
            }
        }

        if (bestGenCarScore > bestEverCarScore)
        {
            bestEverCarScore = bestGenCarScore;
        }

        bestGenCarScore = 0;
    }

    public int bestScoreCar()
    {
        int bestCarIndex = 0;
        for (int i = 0; i < numberOfCars; i++)
        {
            CarScript carScript = carData[i];

            if (carScript.car_score > bestGenCarScore)
            {
                bestGenCarScore = carScript.car_score;
                bestCarIndex = i;
            }
        }
        return bestCarIndex;
    }
}
