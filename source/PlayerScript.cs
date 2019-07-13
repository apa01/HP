using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private float offset;
    private int playerPosition,levelSize,colNum,rowNum,targetPos,boxTargetPos;
    private GameObject box;
    

    void Start()
    {
        offset = LevelManager.instance.Offset;
        playerPosition = LevelManager.instance.PlayerStartPos;
        levelSize = PlayerPrefs.GetInt("LevelSize",0);
        colNum = PlayerPrefs.GetInt("ColNum",0);
        rowNum = PlayerPrefs.GetInt("RowNum",0);
    }
    void Update()
    {
        KeyPress();
    }
    
    private void KeyPress()
    {
        if(LevelManager.instance.CanMove)
        {
            // try to move player up
            if(Input.GetKeyDown(KeyCode.UpArrow)){
                targetPos = playerPosition - rowNum;
                boxTargetPos = targetPos - rowNum;

                //check if target position is occupied (wall/box)
                if (targetPos > 0 && !LevelManager.instance.IsWall(targetPos))
                {

                    // move box at target position (if posible), then move player up
                    if (LevelManager.instance.Occupied(targetPos) && !LevelManager.instance.Occupied(boxTargetPos) && boxTargetPos > 0 && !LevelManager.instance.IsWall(boxTargetPos))
                    {
                        TryMoveBox(targetPos,0);
                        MovePlayer(0);//up
                    }

                    // if target position is not occupied move player up
                    else if (!LevelManager.instance.Occupied(targetPos))
                    {
                        MovePlayer(0);//up
                    }
                }
            }
            
            // try to move player right
            else if(Input.GetKeyDown(KeyCode.RightArrow)){
                targetPos = playerPosition + 1;
                boxTargetPos = targetPos + 1;

                //check if target position is occupied (wall/box)   
                if (targetPos < levelSize && !LevelManager.instance.IsWall(targetPos))
                {

                    // move box at target position (if posible), then move player right
                    if (LevelManager.instance.Occupied(targetPos) && !LevelManager.instance.Occupied(boxTargetPos) && boxTargetPos < levelSize && !LevelManager.instance.IsWall(boxTargetPos))
                    {
                        TryMoveBox(targetPos,1);
                        MovePlayer(1);//right
                    }

                    // if target position is not occupied move player right
                    else if (!LevelManager.instance.Occupied(targetPos))
                    {
                        MovePlayer(1);//right
                    }
                }
            }

            // try to move player down
            else if(Input.GetKeyDown(KeyCode.DownArrow)){
                targetPos = playerPosition + rowNum;
                boxTargetPos = targetPos + rowNum;

                //check if target position is occupied (wall/box)
                if (targetPos <= levelSize && !LevelManager.instance.IsWall(targetPos))
                {

                    // move box at target position (if posible), then move player down
                    if (LevelManager.instance.Occupied(targetPos) && !LevelManager.instance.Occupied(boxTargetPos) &&  boxTargetPos <= levelSize && !LevelManager.instance.IsWall(boxTargetPos))
                    {

                        TryMoveBox(targetPos,2);
                        MovePlayer(2);//up
                    }

                    // if target position is not occupied move player down
                    else if (!LevelManager.instance.Occupied(targetPos))
                    {
                        MovePlayer(2);//down
                    }
                }
            }
            
            // try to move player left
            else if(Input.GetKeyDown(KeyCode.LeftArrow)){
                targetPos = playerPosition - 1;
                boxTargetPos = targetPos - 1;

                //check if target position is occupied (wall/box)
                if (targetPos > 0 && !LevelManager.instance.IsWall(targetPos))
                {

                    // move box at target position (if posible), then move player left
                    if (LevelManager.instance.Occupied(targetPos) && !LevelManager.instance.Occupied(boxTargetPos) && boxTargetPos > 0 && !LevelManager.instance.IsWall(boxTargetPos))
                    {
                        TryMoveBox(targetPos,3);
                        MovePlayer(3);//left
                    }

                    // if target position is not occupied move player left
                    else if (!LevelManager.instance.Occupied(targetPos))
                    {                   
                        MovePlayer(3);//left
                    }
                }
            }
        }
    }

    //move player according to direction
    private void TryMoveBox(int targetPos, int direction)
    {
        box = LevelManager.instance.ObjectPos[targetPos];
        LevelManager.instance.MoveBox(targetPos,direction,box);
    }


    
    //move player according to direction
    private void MovePlayer(int direction)
    {

        switch(direction){
            case 0:
            playerPosition -= rowNum;
            transform.Translate(0,offset,0);
            break;
            case 1:
            playerPosition += 1;
            transform.Translate(offset,0,0);
            break;
            case 2:
            playerPosition += rowNum;
            transform.Translate(0,-offset,0);
            break;
            case 3:
            playerPosition -= 1;
            transform.Translate(-offset,0,0);
            break;
        }
    }

}
