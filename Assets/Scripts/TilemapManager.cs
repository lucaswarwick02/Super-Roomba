using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileEffect
{
    Invalid,
    SingleDirt,
    DoubleDirt,
    Slippery,
    Battery,
    CatPush,
    Ring
}

public class TilemapManager : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap effectsTilemap;
    public Tilemap invalidTilemap;
    public Tile singleDirtTile;

    public GameObject roomba1;
    public GameObject roomba2;

    private Color green = new Color(0.4f, 1f, 0.1f);

    private Animator r1p_Animator;
    private Animator r2p_Animator;

    [HideInInspector] public static bool sliding1 = false;
    [HideInInspector] public static bool sliding2 = false;
    [HideInInspector] public static bool catPush1 = false;
    [HideInInspector] public static bool catPush2 = false;

    [HideInInspector] public Vector3Int currentPos1;
    [HideInInspector] public Vector3Int currentPos2;
    [HideInInspector] public Vector3 offset = new Vector3(-0.5f, -0.5f, 0f);

    public static TilemapManager INSTANCE;

    private void Awake() {
        INSTANCE = this;
        sliding1 = false;
        sliding2 = false;
        catPush1 = false;
        catPush2 = false;
    }

    private void Start () {
        r1p_Animator = roomba1.GetComponent<Animator>();
        r2p_Animator = roomba2.GetComponent<Animator>();
        currentPos1 = GameState.INSTANCE.startPos1;
        currentPos2 = GameState.INSTANCE.startPos2;
    }

    public void newPos(Vector3Int velocity1,Vector3Int velocity2){
        bool invalidCheck1 = false;
        bool invalidCheck2 = false;
        Vector3Int nextPosRCheck1 = new Vector3Int(0,0,0);
        Vector3Int nextPosRCheck2 = new Vector3Int(0,0,0);
       
        Vector3Int nextPos1 = currentPos1 + velocity1;
        if (invalidTilemap.GetTile(nextPos1)){
            invalidCheck1 = true;
            sliding1 = false; //stop sliding from slipSquares if you were sliding
            catPush1 = false; //stop sliding from catPush if you were sliding
            // GameState.INSTANCE.catPushSound.Stop();
            GameState.INSTANCE.slipSound.Stop();
            GameState.INSTANCE.bumpWallSound.Play();
            nextPosRCheck1 =  currentPos1;

            
        } // This tilemap only has invalid tiles, so just check it's not null
        else{
            nextPosRCheck1 = nextPos1;}
        Vector3Int nextPos2 = currentPos2 + velocity2;
        if (invalidTilemap.GetTile(nextPos2)){
            invalidCheck2 = true;
            sliding2 = false; //stop sliding from slipSquares if you were sliding
            catPush2 = false; //stop sliding from catPush if you were sliding
            // GameState.INSTANCE.catPushSound.Stop();
            GameState.INSTANCE.slipSound.Stop();
            GameState.INSTANCE.bumpWallSound.Play();
            nextPosRCheck2 =  currentPos2;
        } // This tilemap only has invalid tiles, so just check it's not null
        else{
            nextPosRCheck2 = nextPos2;}

        if(nextPosRCheck1 != nextPosRCheck2){
            if(!invalidCheck1){
                if(sliding1 | catPush1){}
                else{
                    GameState.INSTANCE.roombaMoveSound.Play();
                }
                currentPos1 = nextPos1;
            }
            if(!invalidCheck2){
                if(sliding2 | catPush2){}
                else{
                    GameState.INSTANCE.roombaMoveSound.Play();
                }
                currentPos2 = nextPos2;
            }
        }
        else{
            // GameState.INSTANCE.catPushSound.Stop();
            GameState.INSTANCE.slipSound.Stop();
            GameState.INSTANCE.bumpRoombaSound.Play();
            sliding1 = false; 
            catPush1 = false;
            sliding2 = false; 
            catPush2 = false;
        }

        PlayerMovement.INSTANCE.movePoint1.position = currentPos1 - offset;
        PlayerMovement.INSTANCE.movePoint2.position = currentPos2 - offset;
    }

    public void ProcessInput1 (Vector3Int velocity)
    {
        if (effectsTilemap.GetTile(currentPos1)) // If we have landed on an "effect" tile (i.e., battery, slippery tile, etc)
        {
            switch (effectsTilemap.GetTile(currentPos1).name)
            {
                case "Effects_2":
                    PerformEffect1(TileEffect.Slippery, velocity);
                    break;
                case "Effects_6":
                    velocity = new Vector3Int(0, 1, 0);
                    PerformEffect1(TileEffect.CatPush, velocity);
                    break;
                case "Effects_7":
                    velocity = new Vector3Int(0, -1, 0);
                    PerformEffect1(TileEffect.CatPush, velocity);
                    break;
                case "Effects_8":
                    velocity = new Vector3Int(-1, 0, 0);
                    PerformEffect1(TileEffect.CatPush, velocity);
                    break;
                case "Effects_9":
                    velocity = new Vector3Int(1, 0, 0);
                    PerformEffect1(TileEffect.SingleDirt, velocity);
                    break;
                case "slippery":
                    PerformEffect1(TileEffect.Slippery, velocity);
                    break;
                case "catUpA":
                    velocity = new Vector3Int(0, 1, 0);
                    PerformEffect1(TileEffect.CatPush, velocity);
                    break;
                case "catDownA":
                    velocity = new Vector3Int(0, -1, 0);
                    PerformEffect1(TileEffect.CatPush, velocity);
                    break;
                case "catLeftA":
                    velocity = new Vector3Int(-1, 0, 0);
                    PerformEffect1(TileEffect.CatPush, velocity);
                    break;
                case "catRightA":
                    velocity = new Vector3Int(1, 0, 0);
                    PerformEffect1(TileEffect.CatPush, velocity);
                    break;

                default:
                    break;
            }
        }

        if (catPush1) {
            sliding1 = false;
            newPos(velocity, new Vector3Int(0, 0, 0));
            ProcessInput1(velocity);
            // PerformEffect(TileEffect.CatPush, currentPos, velocity);
        }
        
        if (sliding1) {
            sliding1 = false;
            PlayerMovement.INSTANCE.updateSpeed1(6f);
            newPos(velocity, new Vector3Int(0, 0, 0));
            ProcessInput1(velocity);
        }
    }

    public void ProcessInput2 (Vector3Int velocity)
    {
        if (effectsTilemap.GetTile(currentPos2)) // If we have landed on an "effect" tile (i.e., battery, slippery tile, etc)
        {
            switch (effectsTilemap.GetTile(currentPos2).name)
            {
                case "Effects_2":
                    PerformEffect2(TileEffect.Slippery, velocity);
                    break;
                case "Effects_6":
                    velocity = new Vector3Int(0, 1, 0);
                    PerformEffect2(TileEffect.CatPush, velocity);
                    break;
                case "Effects_7":
                    velocity = new Vector3Int(0, -1, 0);
                    PerformEffect2(TileEffect.CatPush, velocity);
                    break;
                case "Effects_8":
                    velocity = new Vector3Int(-1, 0, 0);
                    PerformEffect2(TileEffect.CatPush, velocity);
                    break;
                case "Effects_9":
                    velocity = new Vector3Int(1, 0, 0);
                    PerformEffect2(TileEffect.CatPush, velocity);
                    break;
                    case "slippery":
                    PerformEffect2(TileEffect.Slippery, velocity);
                    break;
                case "catUpA":
                    velocity = new Vector3Int(0, 1, 0);
                    PerformEffect2(TileEffect.CatPush, velocity);
                    break;
                case "catDownA":
                    velocity = new Vector3Int(0, -1, 0);
                    PerformEffect2(TileEffect.CatPush, velocity);
                    break;
                case "catLeftA":
                    velocity = new Vector3Int(-1, 0, 0);
                    PerformEffect2(TileEffect.CatPush, velocity);
                    break;
                case "catRightA":
                    velocity = new Vector3Int(1, 0, 0);
                    PerformEffect2(TileEffect.CatPush, velocity);
                    break;
                default:
                    break;
            }
        }

        if (catPush2) {
            sliding2 = false;
            newPos(new Vector3Int(0, 0, 0),velocity);
            ProcessInput2(velocity);
            // PerformEffect(TileEffect.CatPush, currentPos, velocity);
        }
        
        if (sliding2) {
            sliding2 = false;
            PlayerMovement.INSTANCE.updateSpeed2(6f);
            newPos(new Vector3Int(0, 0, 0),velocity);
            ProcessInput2(velocity);
        }
    }


    public void PerformEffect1 (TileEffect tileEffect2, Vector3Int velocity)
    {
        // Perform effect depending on "effect" tile type
        switch (tileEffect2)
        {
            case TileEffect.Slippery:
            PlayerMovement.INSTANCE.updateSpeed1(6f);
                // Player is now sliding
                GameState.INSTANCE.slipSound.Play();
                sliding1 = true;
                // Move player by velocity
                newPos(velocity, new Vector3Int(0, 0, 0));
                ProcessInput1(velocity);
                break;
            case TileEffect.CatPush:
            PlayerMovement.INSTANCE.updateSpeed1(8f);
            GameState.INSTANCE.catPushSound.Play();
                catPush1 = true;
                newPos(velocity, new Vector3Int(0, 0, 0));
                ProcessInput1(velocity);
                break;
            default:
                break;
        }
    }
    public void PerformEffect2 (TileEffect tileEffect2, Vector3Int velocity)
    {
        // Perform effect depending on "effect" tile type
        switch (tileEffect2)
        {
            case TileEffect.Slippery:
            PlayerMovement.INSTANCE.updateSpeed2(6f);
                // Player is now sliding
                GameState.INSTANCE.slipSound.Play();
                sliding2 = true;
                // Move player by velocity
                newPos(new Vector3Int(0, 0, 0),velocity);
                ProcessInput2(velocity);
                break;
            case TileEffect.CatPush:
            PlayerMovement.INSTANCE.updateSpeed2(8f);
            GameState.INSTANCE.catPushSound.Play();
                catPush2 = true;
                newPos(new Vector3Int(0, 0, 0),velocity);
                ProcessInput2(velocity);
                break;
            default:
                break;
        }
    }

    public void PerformCollection1 (TileEffect tileEffect2, Vector3Int tilePos) {
        switch (tileEffect2)
        {
            case TileEffect.SingleDirt:
                GameState.INSTANCE.dirtSound.Play();
                GameState.INSTANCE.getPoint.Play();
                GameState.INSTANCE.Dirt -= 1;
                GameState.INSTANCE.dirtCollected +=1;
                effectsTilemap.SetTile(tilePos, null);
                r1p_Animator.SetTrigger("dirtTrigger");
                break;
            case TileEffect.DoubleDirt:
                GameState.INSTANCE.dirtSound.Play();
                GameState.INSTANCE.getPoint.Play();
                GameState.INSTANCE.Dirt -= 1;
                GameState.INSTANCE.dirtCollected +=1;
                effectsTilemap.SetTile(tilePos, singleDirtTile);
                r1p_Animator.SetTrigger("dirtTrigger");
                break;
            case TileEffect.Battery:
                GameState.INSTANCE.batterySound.Play();
                GameState.INSTANCE.Battery1 += 3;
                effectsTilemap.SetTile(tilePos, null);
                r1p_Animator.SetTrigger("batteryTrigger");
                
                break;
            case TileEffect.Ring:
                GameState.INSTANCE.ringSound.Play();
                GameState.INSTANCE.losePoint.Play();
                GameState.INSTANCE.Rings += 1;
                effectsTilemap.SetTile(tilePos, null);
                r1p_Animator.SetTrigger("ringTrigger1");
                break;
            default:
                break;
        }
    }
    public void PerformCollection2 (TileEffect tileEffect2, Vector3Int tilePos) {
        switch (tileEffect2)
        {
            case TileEffect.SingleDirt:
                GameState.INSTANCE.dirtSound.Play();
                GameState.INSTANCE.getPoint.Play();
                GameState.INSTANCE.Dirt -= 1;
                GameState.INSTANCE.dirtCollected +=1;
                effectsTilemap.SetTile(tilePos, null);
                r2p_Animator.SetTrigger("dirt2trigger");
                break;
            case TileEffect.DoubleDirt:
                GameState.INSTANCE.dirtSound.Play();
                GameState.INSTANCE.getPoint.Play();
                GameState.INSTANCE.Dirt -= 1;
                GameState.INSTANCE.dirtCollected +=1;
                effectsTilemap.SetTile(tilePos, singleDirtTile);
                r2p_Animator.SetTrigger("dirt2trigger");
                break;
            case TileEffect.Battery:
                GameState.INSTANCE.batterySound.Play();
                GameState.INSTANCE.Battery2 += 3;
                effectsTilemap.SetTile(tilePos, null);
                r2p_Animator.SetTrigger("battery2Trigger");
                break;
            case TileEffect.Ring:
                GameState.INSTANCE.ringSound.Play();
                GameState.INSTANCE.losePoint.Play();
                GameState.INSTANCE.Rings += 1;
                effectsTilemap.SetTile(tilePos, null);
                r2p_Animator.SetTrigger("ring2trigger");
                break;
            default:
                break;
        }
    }

    public static TileEffect TileNameToEnum (string tileName) {
        switch (tileName)
            {
                case "slippery":
                    return TileEffect.Slippery;
                case "singleDirt":
                    return TileEffect.SingleDirt;
                case "doubleDirt":
                    return TileEffect.DoubleDirt;
                case "ring":
                    return TileEffect.Ring;
                case "battery":
                    return TileEffect.Battery;
                case "catDownA":
                    return TileEffect.CatPush;
                case "catUpA":
                    return TileEffect.CatPush;
                case "catLeftA":
                    return TileEffect.CatPush;
                case "catRightA":
                    return TileEffect.CatPush;
                default:
                    Debug.Log(tileName);
                    return TileEffect.Invalid;
            }
    }
}
