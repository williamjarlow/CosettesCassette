using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class LevelSelect : MonoBehaviour
{

    private int currentFocus;

    private int cassetteAmount;

    [SerializeField] private int rotationAmount;

    public List<RaycastObject> cassettes = new List<RaycastObject>();



    // Use this for initialization

    void Start()

    {

        cassetteAmount = cassettes.Count;

        currentFocus = cassetteAmount - 1;

        print(currentFocus);

    }



    // Update is called once per frame

    void Update()
    {

        if (cassetteAmount != cassettes.Count)

        {

            cassetteAmount = cassettes.Count;

            if (currentFocus < 0)

            {

                currentFocus -= 1;

            }

            else if (currentFocus >= 0)

            {

                currentFocus += 1;

            }

        }



        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward

        {



            if (currentFocus >= -1 && currentFocus < cassetteAmount - 1)

            {

                currentFocus += 1;

                cassettes[currentFocus].transform.Rotate(Vector3.right, rotationAmount);

                print(currentFocus);

            }

        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards

        {



            if (currentFocus > 0 && currentFocus <= cassetteAmount - 1)

            {

                currentFocus -= 1;

                cassettes[currentFocus + 1].transform.Rotate(Vector3.right, -rotationAmount);

                print(currentFocus);

            }

        }

    }

}