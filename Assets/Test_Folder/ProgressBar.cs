using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProgressBar : MonoBehaviour {

    [TextArea]
    public string Notes = "This object should be attached to the canvas in the scene.";

    [SerializeField]
    private Image progressBarFiller;
    [SerializeField]
    private GameManager gameManager;
    private RectTransform thisRect;

    public SkinnedMeshRenderer cassetteWheelLeft;
    public SkinnedMeshRenderer cassetteWheelRight;


    // Might want to move these values to GameManager to have them all at the same place


    [HideInInspector] public float barStartPositionX;
    [HideInInspector]  public float barEndPositionX;
    [HideInInspector]  public float barLength;

    [HideInInspector]  public float barStartPositionY;
    [HideInInspector]  public float barEndPositionY;
    [HideInInspector]  public float barHeight;


    void Start ()
    {
        thisRect = GetComponent<RectTransform>();

        barStartPositionX = transform.position.x - (thisRect.rect.width / 2);      // Get start position of bar from mid of bar - width of bar / 2.
        barEndPositionX = transform.position.x + (thisRect.rect.width / 2);        // Get start position of bar from mid of bar + width of bar / 2.
        barLength = thisRect.rect.width;                                           // Total length of bar.

        barStartPositionY = transform.position.y - (thisRect.rect.height / 2);
        barEndPositionY = transform.position.y + (thisRect.rect.height / 2);
        barHeight = thisRect.rect.height;

    }
	

	void Update ()
    {
        /*
        progressBarFiller.fillAmount = gameManager.posInSong/ gameManager.lengthOfSong;

        cassetteWheelLeft.SetBlendShapeWeight(0, (gameManager.posInSong / gameManager.lengthOfSong)*100);       // Have to go *100 'cause weight is in percentage.
        cassetteWheelRight.SetBlendShapeWeight(0, 100 - ((gameManager.posInSong / gameManager.lengthOfSong) * 100));       // Have to go *100 'cause weight is in percentage.
        */

        cassetteWheelLeft.transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z - 1));
        cassetteWheelRight.transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z - 1));
    }
}
