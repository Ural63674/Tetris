using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    [SerializeField] private Vector3 rotationPoint;
    [SerializeField] private float fallTime = 0.8f;
    private float previousTime;
    private int speedIndex = 1;
    private int lineOfGameOver = 19;

    private static int height = 24;
    private static int widthSimple = 10;
    private static Transform[,] gridSimple = new Transform[widthSimple, height];

    private static int widthHard = 12;
    private static Transform[,] gridHard = new Transform[widthHard, height];

    private int width;
    private Transform[,] grid;

    private bool isGaming = true;
    private bool isDeleteLine = false;
    private bool isHardGame = false;
    private bool isGridNull = true;
    private bool isGamePaused = false;

    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        Challenge();
        EventManager.OnGamePaused.AddListener(GamePaused);
    }    

    void Update()
    {
        if (!isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.position += new Vector3(1, 0, 0);
                if ((!isHardGame && !validMove()) || (isHardGame && !validMove() && !isGridNull))
                {
                    transform.position -= new Vector3(1, 0, 0);
                }

                if (isHardGame && !validMove())
                {
                    if (transform.position.x >= width)
                    {
                        transform.position = new Vector3(transform.position.x - width,
                                                         transform.position.y,
                                                         transform.position.z);

                        foreach (Transform children in transform)
                        {
                            int roundedX = Mathf.RoundToInt(children.transform.position.x);

                            if (roundedX < 0)
                            {
                                children.transform.position = new Vector3(children.transform.position.x + width,
                                                                          children.transform.position.y,
                                                                          children.transform.position.z);
                            }
                        }
                    }

                    foreach (Transform children in transform)
                    {
                        int roundedX = Mathf.RoundToInt(children.transform.position.x);

                        if (roundedX >= width)
                        {
                            children.transform.position = new Vector3(children.transform.position.x - width,
                                                                      children.transform.position.y,
                                                                      children.transform.position.z);
                        }
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.position += new Vector3(-1, 0, 0);
                if ((!isHardGame && !validMove()) || (isHardGame && !validMove() && !isGridNull))
                {
                    transform.position -= new Vector3(-1, 0, 0);
                }

                if (isHardGame && !validMove())
                {
                    if (transform.position.x < 0)
                    {
                        transform.position = new Vector3(transform.position.x + width,
                                                         transform.position.y,
                                                         transform.position.z);

                        foreach (Transform children in transform)
                        {
                            int roundedX = Mathf.RoundToInt(children.transform.position.x);

                            if (roundedX >= width)
                            {
                                children.transform.position = new Vector3(children.transform.position.x - width,
                                                                          children.transform.position.y,
                                                                          children.transform.position.z);
                            }
                        }
                    }

                    foreach (Transform children in transform)
                    {
                        int roundedX = Mathf.RoundToInt(children.transform.position.x);

                        if (roundedX < 0)
                        {
                            children.transform.position = new Vector3(children.transform.position.x + width,
                                                                      children.transform.position.y,
                                                                      children.transform.position.z);
                        }
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //поворот
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
                if (!validMove())
                {
                    transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                speedIndex = 10;
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                speedIndex = 1;
            }

            if (isGaming && Time.time - previousTime > fallTime / speedIndex)
            {
                StartCoroutine(MoveDown());
                previousTime = Time.time;
            }
        }     
    }

    private IEnumerator MoveDown()
    {
        isDeleteLine = false;

        transform.position += new Vector3(0, -1, 0);
        if (!validMove())
        {
            transform.position -= new Vector3(0, -1, 0);            
            AddToGrid();
            ChangeColor();            
            CheckForLines();

            if (isDeleteLine == false)
            {
                FindObjectOfType<AudioManager>().PlayBrickSound(audioSource);
            }
            else
            {
                FindObjectOfType<AudioManager>().PlayRowClearSound(audioSource);
            }

            CheckForGameOver();

            if (isGaming)
            {
                this.enabled = false;
                FindObjectOfType<Spawner>().SpawnNextShape();
            }            
        }
        yield return new WaitForSeconds(1f);
    }
    
    private void CheckForLines()
    {
        for (int i = height -1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);

                /// <summary>
                /// Если простая игра добавляем 1 очко, иначе (если сложная игра) - добавляем 2 очка
                /// </summary>
                if (!isHardGame)
                {
                    int addScore = 1;
                    EventManager.SendAddScore(addScore);
                }
                else
                {
                    int addScore = 2;
                    EventManager.SendAddScore(addScore);
                }
                
                isDeleteLine = true;               
            }
        }
    }

    /// <summary>
    /// Проверяем заполнена ли линия блоками
    /// </summary>
    private bool HasLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if (grid[j,i] == null)
            {
                return false;
            }

            if (isHardGame)
            {
                if (grid[j, i + 1] == null)
                {
                    return false;
                }
            }            
        }

        return true;
    }

    /// <summary>
    /// Удаляем линию
    /// </summary>
    private void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;

            if (isHardGame)
            {
                Destroy(grid[j, i + 1].gameObject);
                grid[j, i + 1] = null;
            }                      
        }
    }

    /// <summary>
    /// Опускаем все ряды
    /// </summary>
    private void RowDown(int i)
    {
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if(grid[j, y] != null && isHardGame == false)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
                
                if (grid[j, y] != null && isHardGame == true)
                {
                    grid[j, y - 2] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 2].transform.position -= new Vector3(0, 2, 0);
                }
            }
        }
    }

    private void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX, roundedY] = children;
        }
    }

    /// <summary>
    /// Смена цвета после прекращения движения фигуры 
    /// </summary>
    private void ChangeColor()
    {
        foreach(Transform children in transform)
        {
            children.GetComponent<SpriteRenderer>().color = Color.gray;
        }        
    }

    private void CheckForGameOver()
    {
        for (int i = 0; i < width; i++)
        {
            if (grid[i, lineOfGameOver] != null)
            {
                EventManager.SendGameOver();
                isGaming = false;
            }
        }
    }

    private bool validMove()
    {
        foreach(Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }

            if (grid[roundedX, roundedY] != null)
            {
                isGridNull = false;
                return false;
            }
        }

        return true;
    }
   
    /// <summary>
    /// Выбираем сложность игры
    /// </summary>
    private void Challenge()
    {
        switch (UIScript.GameChallenge)
        {
            case "Normal":
                grid = gridSimple;
                width = widthSimple;
                isHardGame = false;
                break;

            case "Hard":
                grid = gridHard;
                width = widthHard;
                isHardGame = true;
                break;

            default:
                break;
        }
    }

    private void GamePaused()
    {
        isGamePaused = !isGamePaused;
    }
}
