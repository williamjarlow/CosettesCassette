using UnityEngine;
using UnityEngine.UI;


// ** Written by Hannes Gustafsson ** //
public class TEST_ButtonScript : MonoBehaviour {

    // isDown == false means the button is pressed/up. isDown == true means the button is not pressed/down
    private bool isDown;
    [SerializeField] private float speed;
    private Vector3 originalPosition;

	void Start ()
    {
        isDown = false;
        originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Button button = hit.transform.GetComponent<Button>();

                // Make sure the button is enabled because Invoke() calls the function even if the button is disabled
                if (button.enabled && button.interactable)
                    button.onClick.Invoke();
            }
        }	
	}

    public void TestFunc()
    {
        Debug.Log("Clicked button"); 
    }

    public void TogglePosition()
    {
        // If button is 'down'
        if(isDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, speed * Time.deltaTime);
            Debug.Log("should move towards original pos");
            isDown = false;
        }

        // If button is 'up'
        else if(!isDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, -Camera.main.transform.position, speed * Time.deltaTime);
            Debug.Log("should move towrads camera");
            isDown = true;
        }
    }
}
