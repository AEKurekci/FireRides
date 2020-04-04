using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    [SerializeField]public float ballSpeed = 2f;
    private Rigidbody rigidB;
    [SerializeField] Transform cameraHolder;
    private Quaternion aimHolder;
    Vector3 tempVec;
    [Header("Walls")]
    [SerializeField] GameObject wallTop;
    [SerializeField] GameObject wallBottom;
    [SerializeField] GameObject wallFloor;
    [SerializeField] int wallNumber = 100;
    [SerializeField] Vector3 wallPositionTop;
    [SerializeField] Vector3 wallPositionBottom;
    [SerializeField] Vector3 wallPositionFloor;
    

    // Start is called before the first frame update
    void Start()
    {
        cameraHolder = Camera.main.transform.parent;
        //aimHolder = transform.parent.transform.rotation;
        wallPositionTop = new Vector3(-1.5f, 70f, 156);
        wallPositionBottom = new Vector3(-1.5f, -70f, 156);
        wallPositionFloor = new Vector3(-450f, -83f, 156);
        BuildWalls(wallTop,wallBottom,wallFloor);
        rigidB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraMovement();
        rigidB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
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
        SetScore();
        Invoke("GameOver", 1f);
    }

    private void SetScore()
    {
        if(PlayerPrefs.GetFloat("best") < transform.position.z/20)
        {
            PlayerPrefs.SetFloat("best", transform.position.z/20);
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

    private void BuildWalls(GameObject up, GameObject down,GameObject floor)
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
            /*renk değişimi
            if (i % 2 == 0)
            {
                up.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
            }*/
            //duvarı oluştur.
            Instantiate(up, wallPositionTop, Quaternion.identity);
            Instantiate(down, wallPositionBottom, Quaternion.identity);
            Instantiate(floor, wallPositionFloor, Quaternion.identity);
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
