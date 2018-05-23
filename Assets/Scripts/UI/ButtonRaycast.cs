using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRaycast : MonoBehaviour {
    private Camera mainCam;
	void Start ()
    {

	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
