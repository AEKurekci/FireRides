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
    private float hardnessTop = 0f;
    [Header("Circle")]
    [SerializeField] GameObject circle;

    

    // Start is called before the first frame update
    void Start()
    {
        cameraHolder = Camera.main.transform.parent;
        wallPositionTop = new Vector3(-1.5f, 75f, 156);
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
            if(hardnessTop > -10)
            {
                hardnessTop -= 0.5f;
            }
        }
        CheckViewFinder();
    }

    private void CheckViewFinder()
    {
        Wall[] walls = FindObjectsOfType<Wall>();
        int lenght = walls.Length;
        for(int i = 0; i < lenght; i++)
        {
            if(walls[i].gameObject.transform.position.z < (transform.position.z - 50f))
            {
                Destroy(walls[i].gameObject);
            }
        }
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
        //direction:random value; up: 0 straight: 1 down: 2 
        int directionRandom = (int)Random.Range(0f,3f);
        float directionOfBuilding = DefineDirection(directionRandom);
        float tempDirection = directionOfBuilding;
        for (int i = 0; i < wallNumber; i++)
        {
            //duvarın z kordinatı doğrultusunda bitişik bir şekilde ilerlemesi için
            MoveAlongZ(up, down, floor);

            //rastgele y değeri ile duvarın yüksekliği değiştirilir.
            randomTopY = Random.Range(-10f, 20f) + hardnessTop + directionOfBuilding;
            randomBottomY = Random.Range(-20f, 10f) + directionOfBuilding;            
            wallPositionTop.y += randomTopY;
            wallPositionBottom.y += randomBottomY;
            wallPositionFloor.y += randomBottomY;
            //duvarı oluştur.
            GameObject upO = Instantiate(up, wallPositionTop, Quaternion.identity);
            GameObject downO = Instantiate(down, wallPositionBottom, Quaternion.identity);
            GameObject floorO = Instantiate(floor, wallPositionFloor, Quaternion.identity);
            if (i % 2 == 0)
            {
                //renk değişimi
                ChangeColors(upO, downO, floorO);
            }
            if (i % 15 == 0 && i != 0)
            {
                //15 duvarda bir halka oluşturuldu
                CreateCircle(circle);
            }
            //duvarın y değerleri geri getirilir
            wallPositionBottom.y = tempWallBottom.y + directionOfBuilding;
            wallPositionFloor.y = tempWallBottom.y + directionOfBuilding;
            wallPositionTop.y = tempWallTop.y + directionOfBuilding;
            directionOfBuilding += tempDirection;
        }
    }

    private void CreateCircle(GameObject circle)
    {
        Vector3 circlePos = new Vector3(wallPositionTop.x, (wallPositionTop.y + wallPositionBottom.y) / 2, wallPositionTop.z);
        Instantiate(circle, circlePos, Quaternion.identity);
    }

    private void MoveAlongZ(GameObject up, GameObject down, GameObject floor)
    {
        wallPositionTop.z += up.transform.localScale.z;
        wallPositionBottom.z += down.transform.localScale.z;
        wallPositionFloor.z += floor.transform.localScale.z;
    }

    private void ChangeColors(GameObject upO, GameObject downO, GameObject floorO)
    {
        upO.GetComponent<MeshRenderer>().material = wallMaterialBlue;
        downO.GetComponent<MeshRenderer>().material = wallMaterialBlue;
        floorO.GetComponent<MeshRenderer>().material = wallMaterialBlue;
    }

    private float DefineDirection(int randDir)
    {
        float directionOfBuilding = 0f;
        switch (randDir)
        {
            case 0:
                directionOfBuilding = .5f;
                break;
            case 1:
                directionOfBuilding = 0f;
                break;
            case 2:
                directionOfBuilding = -.5f;
                break;
        }
        return directionOfBuilding;
    }

    private void CameraMovement()
    {

        tempVec.x = cameraHolder.transform.position.x;
        tempVec.y = transform.position.y;
        tempVec.z = transform.position.z;
        cameraHolder.transform.position = tempVec;
    }
}
