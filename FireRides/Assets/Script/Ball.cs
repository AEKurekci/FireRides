using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    [SerializeField]public float ballSpeed = 2f;
    private Rigidbody rigidB;
    [SerializeField] Transform cameraHolder;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float volumeOfDeath;
    Vector3 tempVec;
    [Header("Walls")]
    [SerializeField] GameObject wallTop;
    [SerializeField] GameObject wallBottom;
    [SerializeField] GameObject wallFloor;
    [SerializeField] Material wallMaterialBlue;
    [SerializeField] int wallNumber = 100;
    Vector3 wallPositionTop;
    Vector3 wallPositionBottom;
    Vector3 wallPositionFloor;
    private float startingPointZ;
    [Header("Circle")]
    [SerializeField] GameObject circle;
    

    // Start is called before the first frame update
    void Start()
    {
        cameraHolder = Camera.main.transform.parent;
        wallPositionTop = new Vector3(-1.5f, 70f, 156);
        wallPositionBottom = new Vector3(-1.5f, -70f, 156);
        wallPositionFloor = new Vector3(-450f, -83f, 156);
        BuildWalls(wallTop,wallBottom,wallFloor, circle);
        rigidB = GetComponent<Rigidbody>();
        startingPointZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        CameraMovement();
        rigidB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        if(transform.position.z > (wallTop.transform.localScale.z * (wallNumber - 20)) + startingPointZ)
        {
            BuildWalls(wallTop, wallBottom, wallFloor, circle);
            startingPointZ = transform.position.z;
        }
        CheckViewFinder();
    }

    private void CheckViewFinder()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Dead();
    }

    private void Dead()
    {
        rigidB.velocity = Vector3.zero;
        rigidB.useGravity = false;
        rigidB.constraints = RigidbodyConstraints.FreezeAll;
        FindObjectOfType<RopeController>().gameOver = true;
        GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        Behaviour halo = (Behaviour)GetComponent("Halo");
        halo.enabled = false;
        SetScore();
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, volumeOfDeath);
        Invoke("GameOver", 1f);
    }

    private void SetScore()
    {
        if(PlayerPrefs.GetFloat("best") < transform.position.z/20)
        {
            string tempScore = (transform.position.z / 20).ToString("F1");
            PlayerPrefs.SetFloat("best",float.Parse(tempScore));
        }
    }

    private void GameOver()
    {
        if(FindObjectOfType<GameOverCanvas>().gameObject != null)
        {
            FindObjectOfType<GameOverCanvas>().GetComponent<Canvas>().enabled = true;
            FindObjectOfType<GameOverCanvas>().gameObject.GetComponentInChildren<DisplayBest>().Display();
        }
    }

    private void BuildWalls(GameObject up, GameObject down,GameObject floor, GameObject circle)
    {
        float randomBottomY;
        float randomTopY;
        Vector3 tempWallTop = wallPositionTop;
        Vector3 tempWallBottom = wallPositionBottom;
        for (int i = 0; i < wallNumber; i++)
        {
            randomTopY = Random.Range(-10f, 25f);
            randomBottomY = Random.Range(-25f, 10f);
            //duvarın z kordinatı doğrultusunda bitişik bir şekilde ilerlemesi için
            wallPositionTop.z += up.transform.localScale.z;
            wallPositionBottom.z += down.transform.localScale.z;
            wallPositionFloor.z += floor.transform.localScale.z;
            //rastgele y değeri ile duvarın yüksekliği değiştirilir.
            wallPositionTop.y += randomTopY;
            wallPositionBottom.y += randomBottomY;
            wallPositionFloor.y += randomBottomY;
            //duvarı oluştur.
            GameObject upO = Instantiate(up, wallPositionTop, Quaternion.identity);
            GameObject downO = Instantiate(down, wallPositionBottom, Quaternion.identity);
            GameObject floorO = Instantiate(floor, wallPositionFloor, Quaternion.identity);
            //renk değişimi
            if (i % 2 == 0)
            {
                upO.GetComponent<MeshRenderer>().material = wallMaterialBlue;
                downO.GetComponent<MeshRenderer>().material = wallMaterialBlue;
                floorO.GetComponent<MeshRenderer>().material = wallMaterialBlue;
            }
            if (i % 15 == 0 && i != 0)
            {
                Debug.Log(i);
                wallPositionTop.y = 7f;
                Instantiate(circle, wallPositionTop, Quaternion.identity);
            }
            //duvarın y değerleri geri getirilir
            wallPositionBottom.y = tempWallBottom.y;
            wallPositionFloor.y = tempWallBottom.y;
            wallPositionTop.y = tempWallTop.y;
        }
    }

    private void CameraMovement()
    {

        tempVec.x = cameraHolder.transform.position.x;
        tempVec.y = transform.position.y;
        tempVec.z = transform.position.z;
        cameraHolder.transform.position = tempVec;
    }
}
