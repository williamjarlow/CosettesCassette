using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NotesHuntHandler : MonoBehaviour {

    [SerializeField] private GameObject notePrefab;
    [SerializeField] private int noteAmount;
    [SerializeField] private float speed;
    [Tooltip("Minimum x spawn coordinate")][SerializeField] private float xSpawnRandomMin;
    [Tooltip("Maximum x spawn coordinate")][SerializeField] private float xSpawnRandomMax;
    [Tooltip("Interval in second that notes will spawn in")][SerializeField] private float spawnInterval;
    [SerializeField] private float rayLength = 100;

    public enum NoteType { CORRECT, INCORRECT };
    public NoteType noteType { get; set; }
    // First value represents the spawn time in milliseconds and the second value represents the type of note that will be spawned
    public List<KeyValuePair<int, NoteType>> noteSpawns; //= new List<KeyValuePair<int, NoteType>>();

    void Start ()
    {
        
	}
	
	
	void Update ()
    {
	    if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if(Physics.Raycast(ray, out hit, rayLength)) 
            {
                Debug.Log("Hit the object: " + hit.transform.name);
                // Add/remove points
                Destroy(hit.transform.gameObject);
            }
        }

        // ** Temporary for testing ** // 
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, rayLength))
            {
                Debug.Log("Hit the object: " + hit.transform.name);
                // Add/remove points
                Destroy(hit.transform.gameObject);
            }
        }

    }


    private void SpawnNotes()
    {

        // Spawn the note and set the speed
        GameObject note = Instantiate(notePrefab, new Vector3(Random.Range(xSpawnRandomMin, xSpawnRandomMax), 0), Quaternion.identity);
        note.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.up * speed);

    }

    private IEnumerator SpawnCooldown()
    {
        for (int i = 0; i < noteAmount; i++)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnNotes();
        }
    }
}
