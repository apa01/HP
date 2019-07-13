using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    public GameObject floorPrefab,wallPrefab,boxPrefab,playerPrefab,goalPrefab,gameOverPanel,PlayerObj;
    public Canvas canvas;
    
    [SerializeField]
    List<string> LevelIndex;  // base to create the stage
    [SerializeField]
    List<int> LevelData;    // marking tiles type(wall,floor or goal)
    public List<GameObject> ObjectPos; // keeping object (box)

    public float Offset;
    public int PlayerStartPos;
    public bool CanMove;
    int cols,rows,boxCount,goalCount,goalInCount,undoCount,NoMove;

    void Start()
    {
        CanMove = true;
        gameOverPanel.SetActive(false);
        undoCount = 3;
        ReadLevel();
    }
    
    
    //read stage from text then create level array
    void ReadLevel()
    {
        TextAsset textFile = Resources.Load("level") as TextAsset;
        string[] str = textFile.text.Split(new[] {'\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
        string[] str1 = str[0].Split(';');

        for (int i = 0; i < str.Length; i++)
        {
            str1 = str[i].Split(';');
            for(int j = 0; j < str1.Length; j++)
            {
                LevelIndex.Add (str1 [j]);
                if(str1 [j]=="$"){boxCount++;}
                LevelData.Add (j);
            }
        }

        cols = str.Length;
        rows = str1.Length;
        PlayerPrefs.SetInt("LevelSize",LevelIndex.Count);
        PlayerPrefs.SetInt("ColNum",cols);
        PlayerPrefs.SetInt("RowNum",rows);
        CreateStage();
    }

    //create level from level array
    void CreateStage()
    {
        MemorizeBoxPos = new int[boxCount,undoCount];
        boxCount = 0;
        float x = Offset/2;
		float y = -(Offset/2);
        SpriteRenderer sr;
        for (int i =0; i < LevelIndex.Count; i++)
        {
            if (i > 0 && i % rows == 0) {
				y -= Offset;
				x = Offset/2;
			}

            // create wall
            if (LevelIndex[i] == "#")
            {
                GameObject Wall = (GameObject)Instantiate(wallPrefab);
                Wall.transform.position = centering(x, y);
                Wall.transform.SetParent(canvas.transform);
                ObjectPos.Add(null);
                LevelData[i] = 1;   // mark tile as wall (1)
            }

            // create goal
            else if(LevelIndex[i] == ".")
            {
                goalCount++;
                GameObject Goal = (GameObject)Instantiate(goalPrefab);
                Goal.transform.position = centering(x, y);
                Goal.transform.SetParent(canvas.transform);
                ObjectPos.Add(null);
                LevelData[i] = 2;   // mark tile as goal (2)
            }

            // create floor
            else
            {
                GameObject Floor = (GameObject)Instantiate(floorPrefab);
                Floor.transform.localPosition = centering(x, y);
                Floor.transform.SetParent(canvas.transform);
                ObjectPos.Add(null);
                LevelData[i] = 0;  // mark tile as floor/blank (0)
            }

            // placing player
            if (LevelIndex[i] == "@")
            {
                GameObject Player = (GameObject)Instantiate(playerPrefab);
                Player.transform.localPosition = centering(x, y);
                sr = Player.GetComponent<SpriteRenderer>();
                sr.sortingOrder =1;
                Player.transform.SetParent(canvas.transform);
                PlayerStartPos = i;
                // PlayerPrefs.SetInt("PlayerPos",i);
                ObjectPos.Add(null);
                MemorizePlayerPos.Add(i);
            }
            else if(LevelIndex[i] == "$")
            {
                
                GameObject Box = (GameObject)Instantiate(boxPrefab);
                Box.transform.localPosition = centering(x, y);
                sr = Box.GetComponent<SpriteRenderer>();
                sr.sortingOrder=1;
                Box.transform.SetParent(canvas.transform);
                ObjectPos[i] = Box;
                MemorizeBoxPos[boxCount,0]= i;
                boxCount++;
            }

        x += Offset;
        }

        if (boxCount < goalCount)
        {
            Debug.Log("Level Error");
        }
    }

    //positioning stage in center of the screen
    Vector2 centering(float x, float y)
    {
        return new Vector2(x-(rows*Offset/2),y+(cols*Offset/2));
    }

    //check if tile is wall
    public bool IsWall (int numPos)
    {
        if(LevelData[numPos] == 1)
        {
            return true;
        }
        return false;
    }

    //check if tile is occupied (by box)
    public bool Occupied(int numPos)
    {
        if(ObjectPos[numPos])
        {
            return true;
        }
        return false;
    }

    // check if box is on the goal
    void GoalIn (int pos, int target)
    {
        if(LevelData[target] == 2 && ObjectPos[target])
        {
            goalInCount++;
            if(goalInCount == goalCount)
            {
                StartCoroutine(GameClear());
            }
        }
        else if (LevelData[pos] == 2 && !ObjectPos[pos])
        {
            goalInCount--;
        }
    }

    // game clear window
    IEnumerator GameClear()
    {   
        CanMove = false;
        yield return new WaitForSeconds(1);
        gameOverPanel.SetActive(true);
        ClearData();
    }
    
    void ClearData()
    {
        LevelIndex.Clear();
        LevelData.Clear();
        ObjectPos = null;
    }

    // move the box
    public void MoveBox(int i, int direction, GameObject box)
    {
        
        switch(direction){
            // move box position up
            case 0:
            box.transform.Translate(0,Offset,0);            
            ObjectPos[i-rows]= box;
            ObjectPos[i] = null;
            GoalIn(i, i-rows);
            break;

            // move box position right
            case 1:
            box.transform.Translate(Offset,0,0);
            ObjectPos[i+1]= box;
            ObjectPos[i] = null;
            GoalIn(i, i+1);
            break;

            // move box position down
            case 2:
            box.transform.Translate(0,-Offset,0);
            ObjectPos[i+rows]= box;
            ObjectPos[i] = null;
            GoalIn(i, i+rows);
            break;

            // move box position left
            case 3:
            box.transform.Translate(-Offset,0,0);
            ObjectPos[i-1]= box;
            ObjectPos[i] = null;
            GoalIn(i, i-1);
            break;
        }
    }
}
