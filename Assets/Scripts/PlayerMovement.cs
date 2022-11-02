using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement INSTANCE;

    private Vector3Int nd = new Vector3Int(0,0,0);

    private float movespeed;
    public Transform movePoint; //This is what gets moved instantly and the roomba sprite follows non instantly causing animation

    private bool wfe; //Wait For Effect, specifically if the square is due for cat push (cannot move until you let cat push roomba)

    private Vector3Int movementDir; //vector follows what key is last pressed and saved in a variable (for slip squares)

    private Vector3Int vel;

    private void Start() {
        INSTANCE = this;
        movePoint.parent = null;
        movePoint.position = transform.position - TilemapManager.INSTANCE.offset;
        // transform.position = TilemapManager.INSTANCE.currentPos - TilemapManager.INSTANCE.offset;
        UpdatePlayerPosition(nd);
    }

    public bool isMoving(){
        if(Vector3.Distance(transform.position, movePoint.position) != 0f){
            return true;
        }
        else{
            return false;
        }
    }

    public void updateSpeed(float speed){
        movespeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, 
                movePoint.position,
                movespeed * Time.deltaTime);

        if(!isMoving()){ // cant have tile effects act until roomba stops moving
            if(wfe){
                wfe = false;
                TilemapManager.INSTANCE.ProcessInput(movementDir);
            }
            else{ 
        updateSpeed(3f);

        if(GameState.INSTANCE.battery > 0){

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            wfe = true;
            movementDir = new Vector3Int(0, 1, 0);
            TilemapManager.INSTANCE.newPos(new Vector3Int(0, 1, 0));
            GameState.INSTANCE.DecreaseBattery(1);
         
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            wfe = true;
            movementDir = new Vector3Int(0, -1, 0);
            TilemapManager.INSTANCE.newPos(new Vector3Int(0, -1, 0));
            GameState.INSTANCE.DecreaseBattery(1);
          
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            wfe = true;
            movementDir = new Vector3Int(-1, 0, 0);
            TilemapManager.INSTANCE.newPos(new Vector3Int(-1,0, 0));
            GameState.INSTANCE.DecreaseBattery(1);
        
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            wfe = true;
            movementDir = new Vector3Int(1, 0, 0);
            TilemapManager.INSTANCE.newPos(new Vector3Int(1, 0, 0));
            GameState.INSTANCE.DecreaseBattery(1);
            
        }
        }
        }
        }

    }

    public void UpdatePlayerPosition (Vector3Int velocity)
    {
        transform.position = TilemapManager.INSTANCE.currentPos - TilemapManager.INSTANCE.offset;
        // transform.position = Vector3.MoveTowards(TilemapManager.INSTANCE.currentPos, 
        // TilemapManager.INSTANCE.currentPos,
        // movespeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name.Equals("Effects")) {
            // TODO Roomba has collided with an effects tile
            string tileName = TilemapManager.INSTANCE.effectsTilemap.GetTile(TilemapManager.INSTANCE.currentPos).name;
            //TilemapManager.INSTANCE.PerformEffect(TilemapManager.TileNameToEnum(tileName), new Vector3Int(0, 0, 0));
        }
    }
}
    

    
