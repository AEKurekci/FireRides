using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{   /*ballSpeed: It specify ball speed value.
     * rigibBodyOfBall: It is kept for future rigidbody activities of the ball
     * cameraHolder: It is for camera movement
     * tempVec: It exists for tempopary vector3 value of camera. It is used for specify position of camera. It takes z coordinate position of player
     * pointSerious: It keeps serious of player points.
     * wallTop/Bottom/Floor: These game objects values keeps wall sample to build in game
     * wallMaterialBlue: It takes Material object to color blue one of sequencial builded wall. It saves to keep 3 more wall object.
     * posZOfTop/BottomWall: They are serializeField to presents developer to specify z values later.
     * wallNumber: It takes value to build wall every defined time in running game. It takes between 5 and 25 integer value.
     * wallPositions: They keep values of wall positions.
     * startingPointZ: It keeps starting point Z coordinate of the player and it is updated every wall creation time.
     * hardnessTop: It exists to make game harder. It increase y coordinate value of top wall.
     */
    [Header("Player")]
    [SerializeField]public float ballSpeed = 100f;
    private Rigidbody rigidBodyOfBall;
    [SerializeField] Transform cameraHolder;
    Vector3 tempVec;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float volumeOfDeath;
    public int pointSerious;
    [Header("Walls")]
    [SerializeField] GameObject wallTop;
    [SerializeField] float posZOfTopWall = 75f;
    [SerializeField] GameObject wallBottom;
    [SerializeField] float posZOfBottomWall = -70f;
    [SerializeField] GameObject wallFloor;
    [SerializeField] Material wallMaterialBlue;
    [SerializeField] [Range(5,25)]int wallNumber = 25;
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
        wallPositionTop = new Vector3(-1.5f, posZOfTopWall, 156);
        wallPositionBottom = new Vector3(-1.5f, posZOfBottomWall, 156);
        wallPositionFloor = new Vector3(-450f, posZOfBottomWall - 15f, 156);
        BuildWalls(wallTop,wallBottom,wallFloor, circle);
        rigidBodyOfBall = GetComponent<Rigidbody>();
        startingPointZ = transform.position.z;
        pointSerious = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CameraMovement();
        CheckViewFinder();
        rigidBodyOfBall.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        if(transform.position.z > (wallTop.transform.localScale.z * (wallNumber - 20)) + startingPointZ)
        {
            BuildWalls(wallTop, wallBottom, wallFloor, circle);
            startingPointZ = transform.position.z;
            if(hardnessTop > -10)
            {
                hardnessTop -= 0.5f;
            }
        }
        
    }
    /*
     * This method destroys walls that player pass without touch
     */
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
        rigidBodyOfBall.velocity = Vector3.zero;
        rigidBodyOfBall.useGravity = false;
        rigidBodyOfBall.constraints = RigidbodyConstraints.FreezeAll;
        FindObjectOfType<RopeController>().gameOver = true;
        GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        Behaviour halo = (Behaviour)GetComponent("Halo");
        halo.enabled = false;
        SetScore();
        FindObjectOfType<whooshSound>().gameObject.GetComponent<AudioSource>().Stop();
        FindObjectOfType<FireCrackling>().gameObject.GetComponent<AudioSource>().Stop();
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, volumeOfDeath);
        Invoke("GameOver", 1f);
    }
    /*
     * It checks weather player pass the highest score or not
     * If it passed this method updates new highest score.
     */
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
    /*
     * It is a complicated method to build walls. It is called at the beginning of the game and every build time. It builds walls
     */
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
    /*
     * 
     */
    private void CreateCircle(GameObject circle)
    {
        Vector3 circlePos = new Vector3(wallPositionTop.x, (wallPositionTop.y + wallPositionBottom.y) / 2, wallPositionTop.z);
        Instantiate(circle, circlePos, Quaternion.identity);
    }
    /*
     * It increase z values of the walls
     */
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
    /*
     * It defines direction of building walls. 
     * up: 0 straight: 1 down: 2 
     * */
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
