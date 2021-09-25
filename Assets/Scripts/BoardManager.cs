using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
     [Header("Board")]
    public Vector2Int size;
    public Vector2 offsetTile;
    public Vector2 offsetBoard;

    [Header("Tile")]
    public List<Sprite> tileTypes = new List<Sprite>();
    public GameObject tilePrefab;

    private Vector2 startPosition;
    private Vector2 endPosition;
    private TileController[,] tiles;


    // Start is called before the first frame update
    void Start()
    {
        Vector2 tileSize = tilePrefab.GetComponent<SpriteRenderer>().size;
        CreateBoard(tileSize);

        IsProcessing = false;
        IsSwapping = false;
    }

    // Update is called once per frame

    public bool IsAnimating
    {
        get
        {
            return IsProcessing || IsSwapping;
        }
    }

    public bool IsSwapping{get; set;}

    public bool IsProcessing{get; set;}

    public void Process()
    {
        IsProcessing = true;
        ProcessMatches();

    }

    
    private void CreateBoard(Vector2 tileSize)
    {
        tiles = new TileController[size.x, size.y];

        Vector2 totalSize = (tileSize + offsetTile) * (size - Vector2.one);

        startPosition = (Vector2)transform.position - (totalSize / 2) + offsetBoard;
        endPosition = startPosition + totalSize;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                TileController newTile = Instantiate(tilePrefab, new Vector2(startPosition.x + ((tileSize.x + offsetTile.x) * x), startPosition.y + ((tileSize.y + offsetTile.y) * y)), tilePrefab.transform.rotation, transform).GetComponent<TileController>();
                tiles[x, y] = newTile;

                // get no tile id
                List<int> possibleId = GetStartingPossibleIdList(x, y);
                int newId = possibleId[Random.Range(0, possibleId.Count)];

                newTile.ChangeId(newId, x, y);
            }
        }
    }

    private List<int> GetStartingPossibleIdList(int x, int y)
    {
        List<int> possibleId = new List<int>();

        for (int i = 0; i < tileTypes.Count; i++)
        {
            possibleId.Add(i);
        }

        if (x > 1 && tiles[x - 1, y].id == tiles[x - 2, y].id)
        {
            possibleId.Remove(tiles[x - 1, y].id);
        }

        if (y > 1 && tiles[x, y - 1].id == tiles[x, y - 2].id)
        {
            possibleId.Remove(tiles[x, y - 1].id);
        }

        return possibleId;
    }

    #region Singleton
    private static BoardManager _instance = null;

    public static BoardManager Instance{
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<BoardManager>();
                if(_instance == null)
                {
                    Debug.LogError("Fatal Error : Board Manager Not Found!");
                }
            }
        return _instance;
        }
    }

    private IEnumerator ClearMatches(List<TileController> matchingTiles, System.Action onCompleted)
    {
        List<bool> isCompleted = new List<bool>();
        for(int i = 0; i< matchingTiles.Count; i++)
        {
            isCompleted.Add(false);
        }

        for(int i = 0; i < matchingTiles.Count; i++)
        {
            int index = i;
            StartCoroutine(matchingTiles[i].SetDestroyed(()=>{isCompleted[index] = true;}));
        }

        yield return new WaitUntil(()=>{return IsAllTrue(isCompleted);});
        onCompleted?.Invoke();
    }

    #endregion

    public bool IsAllTrue(List<bool> list)
    {
        foreach(bool status in list)
        {
            if(!status) return false;
        }

        return true;
    }

    public List<TileController> GetAllMatches()
    {
        List<TileController> matchingTiles = new List<TileController>();

        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                List<TileController> tileMatched = tiles[x, y].GetAllMatches();

                //just go to the next tile if no match
                if(tileMatched == null|| tileMatched.Count == 0)
                {
                    continue;
                }

                foreach(TileController item in tileMatched)
                {
                    //Add only the one that is not added yet
                    if(!matchingTiles.Contains(item))
                    {
                        matchingTiles.Add(item);
                    }
                }
            }
        }
        return matchingTiles;
    }

    #region Match

    private void ProcessMatches()
    {
        List<TileController> matchingTiles = GetAllMatches();

        //stop locking if no match found 
        if(matchingTiles == null || matchingTiles.Count == 0)
        {
            IsProcessing = false;
            return;
        }

        StartCoroutine(ClearMatches(matchingTiles, ProcessDrop));
    }

    #endregion

    #region Swapping

    #region Drop
    private void ProcessDrop()
    {
        Debug.Log("Now Dropping");

    }
    #endregion
    
    public IEnumerator SwapTilePosition(TileController a, TileController b, System.Action onCompleted)
    {
        IsSwapping = true;

        Vector2Int indexA = GetTileIndex(a);
        Vector2Int indexB = GetTileIndex(b);

        tiles[indexA.x, indexB.y] = b;
        tiles[indexB.x, indexA.y] = a;

        a.ChangeId(a.id, indexB.x, indexB.y);
        b.ChangeId(b.id, indexA.x, indexA.y);

        bool isRoutineACompleted = false;
        bool isRoutineBCompleted = false;

        StartCoroutine(a.MoveTilePosition(GetIndexPosition(indexB), () => {isRoutineACompleted = true;}));
        StartCoroutine(b.MoveTilePosition(GetIndexPosition(indexA), () => {isRoutineBCompleted = true;}));

        yield return new WaitUntil(()=>{return isRoutineACompleted && isRoutineBCompleted;});

        onCompleted?.Invoke();
        IsSwapping = false;
    }

    #endregion
    public Vector2Int GetTileIndex(TileController tile)
    {
        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                if(tile == tiles[x, y]) return new Vector2Int(x, y);
            }
        }

        return new Vector2Int(-1, -1);
    }

    public Vector2 GetIndexPosition(Vector2Int index)
    {
        Vector2 tileSize = tilePrefab.GetComponent<SpriteRenderer>().size;
        return new Vector2(startPosition.x + ((tileSize.x + offsetTile.x) * index.x), startPosition.y + ((tileSize.y + offsetTile.y) * index.y));

    }
}
