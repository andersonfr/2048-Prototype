using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManagerStateMachine : FSM<BoardManager>
{
    public GameObject BoardTileObj 
    { 
        get => m_boardTileObject;
        set => m_boardTileObject = value;
    }
    
    private GameObject m_boardTileObject;

    public BoardManagerStateMachine(BoardManager owner) : base (owner)
    {
    }

    private void DrawBoard()
    {
        this.Owner.DrawBoard();
    }


    public override void OnBegin()
    {
        base.OnBegin();
        if (this.CurrentState == null)
            OnChangeState(new BoardInitializeState(this, () => { DrawBoard(); }));
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}

public class BoardManagerState : State<BoardManager> 
{
    protected BoardManagerStateMachine MyOwner { get => (BoardManagerStateMachine)this.owner; private set { } }
    public BoardManagerState(BoardManagerStateMachine owner) : base(owner)
    {
    }
}

public class BoardInitializeState : BoardManagerState
{
    private Action drawBoard;
    public BoardInitializeState(BoardManagerStateMachine owner, Action drawboardAction) : base(owner)
    {
        drawBoard = drawboardAction;
    }

    public override void OnBegin() 
    {
        base.OnBegin();
        if (drawBoard != null)
            drawBoard();
    }
    public override void OnEnd()
    {
        base.OnEnd();
    }
}

public class BoardGamePlayState : BoardManagerState
{
    private Action<bool,int> slide;
    public BoardGamePlayState(BoardManagerStateMachine owner, Action<bool,int> slideAction) : base(owner) 
    {
        this.slide = slideAction;
    }
    public override void OnBegin()
    {
        base.OnBegin();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            if (slide != null)
                slide(true,0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (slide != null)
                slide(true,1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (slide != null)
                slide(false,1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (slide != null)
                slide(false,0);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}

public class BoardGameEndState : BoardManagerState
{

    private Action clear;
    public BoardGameEndState(BoardManagerStateMachine owner, Action clearAction) : base(owner)
    {
        this.clear = clearAction; 
    }
    public override void OnBegin()
    {
        base.OnBegin();
        if (this.clear != null)
            this.clear();
    }
    public override void OnEnd()
    {
        base.OnEnd();
    }
}
