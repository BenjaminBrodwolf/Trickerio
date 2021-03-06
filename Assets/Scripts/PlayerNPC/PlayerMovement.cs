﻿using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

public class PlayerMovement : MonoBehaviour
{
    private GameObject gameManagerObject;
    private GameManager gameManagerScript;
    public int speed = 10;
    public bool isAlreadyMoving = false;
    AudioSource audioSource;
    public AudioClip footsteps;
    private Vector3 oldTurnPosition;
    private float tick = 0;
    public float movementTime = 0.1f;

    public Vector3 TurnPosition { get; private set; }

    private void Awake()
    {
        TurnPosition = transform.position;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        gameManagerScript = gameManagerObject.GetComponent<GameManager>();
        Debug.Log(gameManagerScript);
    }
    

    public enum Directions
    {
        Up,
        Rigth,
        Down,
        Left
    }


    public bool CanMove(Directions md)
    {
        var raycastDirection = new Vector3(0, -1, 0);
        var playerDirection = gameObject.transform.GetChild(0).gameObject.transform.eulerAngles;

        switch (md)
        {
            case Directions.Up:
            {
                playerDirection.y = 0;
                // gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
                gameObject.transform.GetChild(0).gameObject.transform.eulerAngles = playerDirection;
                
                var wannaMoveDirection = new Vector3(0, 0, 1);
                ReactToMoveableItem(TurnPosition, wannaMoveDirection);

                Debug.DrawRay(TurnPosition + wannaMoveDirection, raycastDirection, Color.red, 20);

                return Physics.Raycast(TurnPosition + wannaMoveDirection, raycastDirection, 1f, LayerMask.GetMask("Earth"))
                       && !Physics.Raycast(TurnPosition, new Vector3(0, 0, 1), 1f, LayerMask.GetMask("MoveableItem"));
            }
            case Directions.Down:
            {
                playerDirection.y = 180;
                gameObject.transform.GetChild(0).gameObject.transform.eulerAngles = playerDirection;
                
                var wannaMoveDirection = new Vector3(0, 0, -1);
                ReactToMoveableItem(TurnPosition, wannaMoveDirection);

                return Physics.Raycast(TurnPosition + wannaMoveDirection, raycastDirection, 1f, LayerMask.GetMask("Earth"))
                       && !Physics.Raycast(TurnPosition, new Vector3(0, 0, -1), 1f, LayerMask.GetMask("MoveableItem"));
            }
            case Directions.Left:
            {
                playerDirection.y = 270;
                gameObject.transform.GetChild(0).gameObject.transform.eulerAngles = playerDirection;
                
                var wannaMoveDirection = new Vector3(-1, 0, 0);
                ReactToMoveableItem(TurnPosition, wannaMoveDirection);

                return Physics.Raycast(TurnPosition + wannaMoveDirection, raycastDirection, 1f, LayerMask.GetMask("Earth"))
                       && !Physics.Raycast(TurnPosition, new Vector3(-1, 0, 0), 1f, LayerMask.GetMask("MoveableItem"));
            }
            case Directions.Rigth:
            {
                playerDirection.y = 90;
                gameObject.transform.GetChild(0).gameObject.transform.eulerAngles = playerDirection;
                
                var wannaMoveDirection = new Vector3(1, 0, 0);
                ReactToMoveableItem(TurnPosition, wannaMoveDirection);

                return Physics.Raycast(TurnPosition + wannaMoveDirection, raycastDirection, 1f, LayerMask.GetMask("Earth"))
                       && !Physics.Raycast(TurnPosition, new Vector3(1, 0, 0), 1f, LayerMask.GetMask("MoveableItem"));
            }
               
        }


        return false;
    }

    public void UpdatePosition(Directions direction)
    {
        var newPosition = TurnPosition;
        switch (direction)
        {
            case Directions.Left:
                newPosition += new Vector3(-1, 0, 0);
                break;
            case Directions.Up:
                newPosition += new Vector3(0, 0, 1);
                break;
            case Directions.Rigth:
                newPosition += new Vector3(1, 0, 0);
                break;
            case Directions.Down:
                newPosition += new Vector3(0, 0, -1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        audioSource.clip = footsteps;
        audioSource.pitch = UnityEngine.Random.Range(1f, 1.4f);
        audioSource.Play();

        oldTurnPosition = TurnPosition;
        TurnPosition = newPosition;
        tick = 0;
        StartCoroutine(nameof(UpdatePositionPerFrame));
    }

    public IEnumerator UpdatePositionPerFrame()
    {

        while (tick <= movementTime)
        {
            tick += Time.deltaTime;
            var lerp = Vector3.Lerp(oldTurnPosition, TurnPosition, tick / movementTime);
            // var actualPosition = transform.position;
            // var moveDirection = TurnPosition - actualPosition;
            // var deltaMovement = moveDirection * (speed * Time.deltaTime);
            // actualPosition += deltaMovement;
            transform.position = lerp;
            yield return null;
        }
    }

    public void ReactToMoveableItem(Vector3 currentPosition, Vector3 newDirection)
    {
        if (Physics.Raycast(currentPosition, newDirection, out var hit, 1f, LayerMask.GetMask("MoveableItem")))
        {
            var moveableItemScript = hit.collider.GetComponent<MoveableItem>();

            if (moveableItemScript.IsPlayerInTriggerToItem())
            {
                moveableItemScript.PushItemInDirection(newDirection);
                gameManagerScript.EnemyTurn();
            }
        }
    }
}