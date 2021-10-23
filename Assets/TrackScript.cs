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

    public float[] aiSpeed = new float[100];

    public float[] aiRotation = new float[100];

    void Start()
    {
        //resetPreviousFiles();
        loadPreviousFiles();
        startGeneration();
    }

    public void resetPreviousFiles()
    {
        PlayerPrefs.SetFloat("Generation", 0);
        PlayerPrefs.SetFloat("bestEverCarScore", 0);
        for (int i = 0; i < numberOfCars; i++)
        {
            PlayerPrefs.SetFloat("probabilities_" + i, 0);
            PlayerPrefs.SetFloat("aiSpeed_" + i, 0);
            PlayerPrefs.SetFloat("aiRotation_" + i, 0);
        }
    }

    public void loadPreviousFiles()
    {
        generationNumber = PlayerPrefs.GetFloat("Generation", 0);
        bestEverCarScore = PlayerPrefs.GetFloat("bestEverCarScore", 0);
        for (int i = 0; i < numberOfCars; i++)
        {
            probabilities[i] = PlayerPrefs.GetFloat("probabilities_" + i, 0);
            aiSpeed[i] = PlayerPrefs.GetFloat("aiSpeed_" + i, 0);
            aiRotation[i] = PlayerPrefs.GetFloat("aiRotation_" + i, 0);
        }
    }

    public void startGeneration()
    {
        generationNumber++;
        PlayerPrefs.SetFloat("Generation", generationNumber);

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
            if (
                bestGenCarScore > bestEverCarScore &&
                carData[bestCarIndex].carSpeedHistory[i] != 0
            )
            {
                aiSpeed[i] = carData[bestCarIndex].carSpeedHistory[i];
                PlayerPrefs.SetFloat("aiSpeed_" + i, aiSpeed[i]);
                aiRotation[i] = carData[bestCarIndex].carRotationHistory[i];
                PlayerPrefs.SetFloat("aiRotation_" + i, aiRotation[i]);
            }

            if (aiSpeed[i] == 0 || probabilities[i] == 0)
            {
                probabilities[i] = 100;
            }
            else if (carData[bestCarIndex].carSpeedHistory[i] != 0)
            {
                probabilities[i] *= 0.99f;
                PlayerPrefs.SetFloat("probabilities_" + i, probabilities[i]);
            }
        }

        if (bestGenCarScore > bestEverCarScore)
        {
            bestEverCarScore = bestGenCarScore;
            PlayerPrefs.SetFloat("bestEverCarScore", bestEverCarScore);
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
