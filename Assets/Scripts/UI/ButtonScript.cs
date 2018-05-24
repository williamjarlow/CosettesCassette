using UnityEngine;
using UnityEngine.UI;


// ** Written by Hannes Gustafsson ** //
public class ButtonScript : MonoBehaviour {

    // isDown == false means the button is pressed/up. isDown == true means the button is not pressed/down
    private bool isDown;
    [Tooltip("The distance the button will travel in Z-position when pressed")]
    [SerializeField] private float buttonDepth = 5;
    [Tooltip("Material for when the button is pressed. The material used when the button is not pressed is the material in the mesh renderer")]
    [SerializeField] private Material selectedMaterial;
    private Material originalMaterial;

    private Vector3 originalPosition;
    private Vector3 newPosition;

    private AudioManager audioManager;

	void Start ()
    {
        isDown = false;
        originalPosition = transform.position;
        
        originalMaterial = GetComponent<MeshRenderer>().material;

        audioManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().audioManager;
	}

    public void TogglePosition()
    {
        // Set the newPosition according to buttonDepth here, so that it updates in real-time
        newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + buttonDepth);

        if (audioManager.startedMusic)
        {
            // If button is 'down'
            if (isDown)
            {
                // Move to original position
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, buttonDepth);

                // Switch material and toggle the bool
                gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
                isDown = false;
            }

            // If button is 'up'
            else if (!isDown)
            {
                // Increase z position
                transform.position = Vector3.MoveTowards(transform.position, newPosition, buttonDepth);

                // Switch material and toggle the bool
                gameObject.GetComponent<MeshRenderer>().material = selectedMaterial;
                isDown = true;
            }
        }
    }

    public void SetPositionUp()
    {
        // Move to original position
        transform.position = Vector3.MoveTowards(transform.position, originalPosition, buttonDepth);

        // Switch material and toggle the bool
        gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
        isDown = false;
    }

    public void SetPositionDown()
    {
        // Increase z position
        transform.position = Vector3.MoveTowards(transform.position, newPosition, buttonDepth);

        // Switch material and toggle the bool
        gameObject.GetComponent<MeshRenderer>().material = selectedMaterial;
        isDown = true;
    }
}
