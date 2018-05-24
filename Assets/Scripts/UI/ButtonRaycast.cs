using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]

public class ButtonRaycast : MonoBehaviour {

    private AudioManager audioManager;
    [SerializeField] private Camera m_MainCamera;

    void Start ()
    {
        audioManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().audioManager;
    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetMouseButtonDown(0) && m_MainCamera.enabled)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Only consider objects with buttons
                if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.gameObject.GetComponent<Button>() != null)
                {
                    Button button = hit.transform.gameObject.GetComponent<Button>();

                    // Make sure the button is enabled because Invoke() calls the functions even if the button is disabled
                    if (button.enabled && button.interactable)
                    {
                        // Invoke all buttons that the button has
                        button.onClick.Invoke();
                    }
                }
            }
    }
}

