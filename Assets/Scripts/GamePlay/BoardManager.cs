using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public enum ETileValues
{
    NUMBER2 = 1 << 1,
    NUMBER4 = 1 << 2,
    NUMBER8 = 1 << 3,
    NUMBER16 = 1 << 4,
    NUMBER32 = 1 << 5,
    NUMBER64 = 1 << 6,
    NUMBER128 = 1 << 7,
    NUMBER256 = 1 << 8,
    NUMBER512 = 1 << 9,
    NUMBER1024 = 1 << 10,
    NUMBER2048 = 1 << 11,
    INVALID = 0,
}
public class BoardManager : IService
{
    private BoardTile[,] m_board;
    private GameObject m_boardBack;
    private BoardPreset m_boardPreset;
    private GameManager m_manager;
    private Dictionary<int, BoardTilePreset> m_boardTilePresets;
    
    private int m_maxNumber = (int)ETileValues.NUMBER2048;
    private bool m_bSlided = false;
    private BoardManagerStateMachine m_machine;

    public Action<int> OnAddScore;
    public BoardManagerStateMachine FSM { get => m_machine; }

    public BoardManager(GameManager gm, BoardPreset boardPreset) 
    {
        m_boardPreset = boardPreset;

        int width = m_boardPreset.width;
        int height = m_boardPreset.height;

        m_manager = gm;
        m_board = new BoardTile[width, height];
        m_boardTilePresets = new Dictionary<int, BoardTilePreset>();

        foreach (var bt in m_boardPreset.boardTilePresets) 
        {
            m_boardTilePresets.Add(bt.tileValue, bt);
        }
    }

    void IService.Start()
    {
        m_machine = new BoardManagerStateMachine(this);
        this.m_machine.OnBegin();
    }

    public void End()
    {
        m_bSlided = false;
        m_machine.OnChangeState(new BoardGameEndState(m_machine, () => ClearBoard()));
        m_machine.OnEnd();
    }

    public void Update() 
    {
        m_machine.OnUpdate();
    }

    #region GamePlay
    public void Slide(bool row, int direction) 
    {
        if (row)
            SlideRows(direction);
        else
            SlideCols(direction);
    }
    private void SlideRows(int direction)
    {
        int i = 0;
        
        for (; i < m_board.GetLength(0); i++) 
        {
            var row = CustomBoardArray<BoardTile>.GetRow(m_board, i);

            var numbers = row.Where(x => x.Value > 0).ToList();
            
            if (numbers.Count == 0 || numbers.Count == m_boardPreset.width) continue;
            
            var zeros = row.Where(x => x.Value == 0).ToList();        

            List<int> values;
            
            if (direction > 0)
            {
                numbers.AddRange(zeros);
                values = numbers.Select((i, v) => new { v, i }).Select(t => t.i.Value).ToList();
            }
            else 
            {
                zeros.AddRange(numbers);
                values = zeros.Select((i, v) => new { v, i }).Select(t => t.i.Value).ToList();
            }

            for (int k = 0; k < m_board.GetLength(0); k++) 
            {
                m_bSlided |= m_board[i, k].Value != values[k];
                m_board[i, k].SetTile(m_boardTilePresets[values[k]]);
            }
        }
    }
    private void SlideCols(int direction)
    {
        int i = 0;

        for (; i < m_board.GetLength(0); i++)
        {
            var col = CustomBoardArray<BoardTile>.GetColumn(m_board, i);

            var numbers = col.Where(x => x.Value > 0).ToList();
            
            if (numbers.Count == 0 || numbers.Count == m_boardPreset.height) continue;
            
            var zeros = col.Where(x => x.Value == 0).ToList();
            
            List<int> values;

            if (direction > 0)
            {
                numbers.AddRange(zeros);
                values = numbers.Select((i, v) => new { v, i }).Select(t => t.i.Value).ToList();
            }
            else
            {
                zeros.AddRange(numbers);
                values = zeros.Select((i, v) => new { v, i }).Select(t => t.i.Value).ToList();
            }

            for (int k = 0; k < m_board.GetLength(0); k++) 
            {
                m_bSlided |= m_board[k,i].Value != values[k];
                m_board[k, i].SetTile(m_boardTilePresets[values[k]]);
            }
        }
    }

    public void Combine(bool row, int direction) 
    {
        if (row)
            CombineRows(direction);
        else
            CombineCols(direction);
    }
    private void CombineCols(int direction) 
    {
        int k = 0;
        int sum = 0;
        for (; k < m_board.GetLength(1); k++)
        {
            if (direction > 0)
            {
                for (int i = 0; i < m_board.GetLength(1); i++)
                {
                    if (i + 1 >= m_board.GetLength(1))
                        break;

                    if (m_board[i, k].Value == 0) continue;

                    if (m_board[i, k].Value == m_board[i + 1,k].Value)
                    {
                        sum = m_board[i, k].Value + m_board[i+1,k].Value;
                        if (sum > m_maxNumber)
                            continue;
                        m_board[i, k].SetTile(m_boardTilePresets[sum]);
                        m_board[i + 1,k].SetTile(m_boardTilePresets[(int)ETileValues.INVALID]);
                        m_bSlided = true;
                    }
                }
            }
            else
            {
                for (int i = m_board.GetLength(1) - 1; i >= 0; i--)
                {
                    if (i - 1 < 0)
                        break;

                    if (m_board[i, k].Value == 0) continue;

                    if (m_board[i, k].Value == m_board[i - 1,k].Value)
                    {
                        sum = m_board[i,k].Value + m_board[i - 1,k].Value;
                        if (sum > m_maxNumber)
                            continue;
                        m_board[i,k].SetTile(m_boardTilePresets[sum]);
                        m_board[i - 1,k].SetTile(m_boardTilePresets[(int)ETileValues.INVALID]);
                        m_bSlided = true;
                    }
                }
            }
        }

        OnAddScore(sum);
    }
    private void CombineRows(int direction) 
    {
        int k = 0;
        int sum = 0;
        for (; k < m_board.GetLength(0); k++)
        { 
            if ( direction > 0)
            {
                for (int i = 0; i < m_board.GetLength(0); i++)
                {
                    if (i + 1 >= m_board.GetLength(0))
                        break;
                    
                    if (m_board[k, i].Value == 0) continue;

                    if (m_board[k, i].Value == m_board[k, i + 1].Value)
                    {
                        sum = m_board[k, i].Value + m_board[k, i + 1].Value;
                        if (sum > m_maxNumber)
                            continue;
                        m_board[k, i].SetTile(m_boardTilePresets[sum]);
                        m_board[k, i + 1].SetTile(m_boardTilePresets[(int)ETileValues.INVALID]);
                        m_bSlided = true;
                    }
                }
            }
            else
            {
                for (int i = m_board.GetLength(0) -1; i >= 0; i--)
                {
                    if (i - 1 < 0)
                        break;

                    if (m_board[k, i].Value == 0) continue;

                    if (m_board[k,i].Value == m_board[k,i-1].Value)
                    {
                        sum = m_board[k, i].Value + m_board[k, i - 1].Value;
                        if (sum > m_maxNumber)
                            continue;
                        m_board[k, i].SetTile(m_boardTilePresets[sum]);
                        m_board[k, i-1].SetTile(m_boardTilePresets[(int)ETileValues.INVALID]);
                        m_bSlided = true;
                    }
                }
            }
        }
        OnAddScore(sum);
    }

    public void RestartGame() 
    {
        ClearBoard();
        m_machine.OnChangeState(new BoardInitializeState(this.m_machine, () => { DrawBoard(); }));
    }

    public void SpawnTile()
    {
        if (!m_bSlided) 
        {
            var Zeroes = new List<BoardTile>();
            for (int i = 0; i < m_board.GetLength(0); i++)
            {
                 Zeroes.AddRange(CustomBoardArray<BoardTile>.GetRow(m_board, i).Where(x => x.Value == 0).ToList());
            }
            
            if(Zeroes.Count ==0)
            {
                RestartGame();
            }
            return;
        }

        int minValue = int.MaxValue;
        for (int i = 0; i < m_board.GetLength(0); i++) 
        {
            var noZeroes = CustomBoardArray<BoardTile>.GetRow(m_board, i).Where(x => x.Value > 0);
            if (!noZeroes.Any()) continue;

            minValue = Mathf.Min(minValue, noZeroes.OrderBy(x => x.Value).First().Value);
        }
            
        //random spawn
        List<BoardTile> candidates = new List<BoardTile>();
        for (int i = 0; i < m_board.GetLength(0); i++)
            candidates.AddRange(CustomBoardArray<BoardTile>.GetRow(m_board, i).Where(x => x.Value == 0).ToList());

        int sort = Random.Range(0, candidates.Count);
        candidates[sort].SetTile(m_boardTilePresets[2]);
        m_manager.StartCoroutine(ApplyAnimation(candidates, sort));
    }

    public IEnumerator ApplyAnimation(List<BoardTile> candidates ,int itemIndex)
    {
        yield return new WaitForEndOfFrame();

        var sequence = DOTween.Sequence().
            Append(candidates[itemIndex].transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.0f), 0.4f, 10, 1));
        sequence.SetLoops(1).onComplete = () => { candidates[itemIndex].transform.localScale = Vector3.one; };

        yield return null;
    }

    public void Gameplay(bool isRow, int direction) 
    {
        Slide(isRow, direction); 
        Combine(isRow, direction); 
        Slide(isRow, direction); 
        SpawnTile();
        m_bSlided = false;
    }

    public void DrawBoard()
    {
        m_boardBack = GameObject.Instantiate(m_boardPreset.boardBackgroundObj);
        var boardObject = m_boardBack.transform.GetChild(0);
        RectTransform b = boardObject.GetComponent<RectTransform>();

        var min = b.sizeDelta / 2.0f;
        
        for (int r = 0; r < m_boardPreset.width; r++)
        {
            for (int c = 0; c < m_boardPreset.height; c++)
            {
                var tile = GameObject.Instantiate(m_boardPreset.boardTileObj, boardObject.transform);
                
                var bt = tile.GetComponent<BoardTile>();
                bt.SetTile(m_boardTilePresets[(int)ETileValues.INVALID]);
                
                var size = bt.GetSizeX();
                
                tile.transform.localPosition = new Vector3(min.x - size/2.0f - c * size, min.y - size/2.0f -  r * size, 1);
                tile.transform.localScale = Vector3.one;

                m_board[r, c] = bt;
            }
        }

        m_board[0, 0].SetTile(m_boardTilePresets[(int)ETileValues.NUMBER2]);
        m_board[0, 1].SetTile(m_boardTilePresets[(int)ETileValues.NUMBER2]);
        m_machine.OnChangeState(new BoardGamePlayState(this.m_machine, (bool isRow, int direction) => { Gameplay(isRow, direction); }));
    }

    private void ClearBoard()
    {
        for (int r = 0; r < m_boardPreset.width; r++)
        {
            for (int c = 0; c < m_boardPreset.height; c++)
            {
                if(m_board[r, c] != null)
                    GameObject.Destroy((m_board[r, c].gameObject));
            }
        }

        GameObject.Destroy(m_boardBack.gameObject);
    }

    #endregion
}