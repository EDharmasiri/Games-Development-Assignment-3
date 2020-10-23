using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class GhostController : MonoBehaviour
{
    //Ghosts and pacman
    public GameObject[] ghosts = new GameObject[4];
    public GameObject pacman;

    //Tweener
    private GhostTweener ghostTweener;

    //Teleporters
    private Vector3 leftTeleport;
    private Vector3 rightTeleport;

    //Safe zones
    private Vector2 spawnZoneStart;
    private Vector2 spawnZoneEnd;

    //Leave spawn positions
    private Vector3 leaveSpawnTop1;
    private Vector3 leaveSpawnBottom1;
    private Vector3 leaveSpawnTop2;
    private Vector3 leaveSpawnBottom2;

    //List for pink ghosts positions
    private List<Vector2> pinkGhostPosition = new List<Vector2>();


    //Ghost States
    private enum GhostState
    {
        Normal,
        Scared,
        Recovering,
        Dead
    }

    //Trackers
    private Dictionary<GameObject, Vector3> startPosition = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector2> lastStep = new Dictionary<GameObject, Vector2>();
    private Dictionary<GameObject, GhostState> state = new Dictionary<GameObject, GhostState>();
    private Dictionary<GameObject, bool> spawnZone = new Dictionary<GameObject, bool>();
    private float ghostDuration;
    private int currentPinkTarget;


    void Start()
    {
        //Set Start Positions
        startPosition.Add(ghosts[0], new Vector3(64.0f, -52.0f, -1.0f));
        startPosition.Add(ghosts[1], new Vector3(64.0f, -60.0f, -1.0f));
        startPosition.Add(ghosts[2], new Vector3(44.0f, -60.0f, -1.0f));
        startPosition.Add(ghosts[3], new Vector3(44.0f, -52.0f, -1.0f));

        ghosts[0].transform.position = startPosition[ghosts[0]];
        ghosts[1].transform.position = startPosition[ghosts[1]];
        ghosts[2].transform.position = startPosition[ghosts[2]];
        ghosts[3].transform.position = startPosition[ghosts[3]];

        //Set dummy last steps
        //lastStep.Add(ghosts[0], Vector2.right * 4);
        //lastStep.Add(ghosts[1], Vector2.right * 4);
        //lastStep.Add(ghosts[2], Vector2.left * 4);
        //lastStep.Add(ghosts[3], Vector2.left * 4);

        //Set Start States
        state.Add(ghosts[0], GhostState.Normal);
        state.Add(ghosts[1], GhostState.Normal);
        state.Add(ghosts[2], GhostState.Normal);
        state.Add(ghosts[3], GhostState.Normal);

        //Set spawnzone tracker
        spawnZone.Add(ghosts[0], true);
        spawnZone.Add(ghosts[1], true);
        spawnZone.Add(ghosts[2], true);
        spawnZone.Add(ghosts[3], true);

        //Set teleporter positions
        leftTeleport = new Vector3(-4.0f, -56.0f, -1);
        rightTeleport = new Vector3(112.0f, -56.0f, -1);

        //Set safezone coordinates
        spawnZoneStart = new Vector2(40, -48);
        spawnZoneEnd = new Vector2(68, -64);

        //Set leave spawn coordinates
        leaveSpawnTop1 = new Vector3(56, -52, -1);
        leaveSpawnBottom1 = new Vector3(52, -60, -1);
        leaveSpawnTop2 = new Vector3(56, -44, -1);
        leaveSpawnBottom2 = new Vector3(52, -68, -1);

        //Set duration
        ghostDuration = 1f;

        ghostTweener = GetComponent<GhostTweener>();

        //Set pink ghost positions
        pinkGhostPosition.Add(new Vector2(24, -56));
        pinkGhostPosition.Add(new Vector2(24, -32));
        pinkGhostPosition.Add(new Vector2(4, -32));
        pinkGhostPosition.Add(new Vector2(4, -4));
        pinkGhostPosition.Add(new Vector2(48, -4));
        pinkGhostPosition.Add(new Vector2(60, -4));
        pinkGhostPosition.Add(new Vector2(104, -4));
        pinkGhostPosition.Add(new Vector2(104, -32));
        pinkGhostPosition.Add(new Vector2(24, -32));
        pinkGhostPosition.Add(new Vector2(84, -56));
        pinkGhostPosition.Add(new Vector2(84, -80));
        pinkGhostPosition.Add(new Vector2(104, -80));
        pinkGhostPosition.Add(new Vector2(104, -108));
        pinkGhostPosition.Add(new Vector2(104, -108));
        pinkGhostPosition.Add(new Vector2(60, -108));
        pinkGhostPosition.Add(new Vector2(48, -108));
        pinkGhostPosition.Add(new Vector2(4, -108));
        pinkGhostPosition.Add(new Vector2(4, -80));
        pinkGhostPosition.Add(new Vector2(24, -80));

        currentPinkTarget = 0;
    }

    void Update()
    {
        //Keep ghosts in spawn position if pacman is dead
        if (PacStudentController.CurrentState == PacStudentController.PacState.Dead)
        {
            ghostTweener.RemoveTween(ghosts[0].transform);
            ghostTweener.RemoveTween(ghosts[1].transform);
            ghostTweener.RemoveTween(ghosts[2].transform);
            ghostTweener.RemoveTween(ghosts[3].transform);
            ghosts[0].transform.position = startPosition[ghosts[0]];
            ghosts[1].transform.position = startPosition[ghosts[1]];
            ghosts[2].transform.position = startPosition[ghosts[2]];
            ghosts[3].transform.position = startPosition[ghosts[3]];
        }
        else
        {
            foreach (GameObject ghost in ghosts)
            {
                //Update animation stuff
                if (PacStudentController.CurrentState == PacStudentController.PacState.Powered)
                {
                    if (PacStudentController.PowerTime <= 3.0f)
                    {
                        ghost.GetComponent<Animator>().SetBool("isScared", false);
                        ghost.GetComponent<Animator>().SetBool("isRecovering", true);
                        state[ghost] = GhostState.Scared;
                    }
                    else
                    {
                        ghost.GetComponent<Animator>().SetBool("isScared", true);
                        ghost.GetComponent<Animator>().SetBool("isRecovering", false);
                        state[ghost] = GhostState.Recovering;
                    }

                    //Change state to dead if pacman touches the ghost
                    if (lastStep.ContainsKey(ghost))
                    {
                        if (hitPacman(ghost, directionSwap(lastStep[ghost])))
                        {
                            state[ghost] = GhostState.Dead;
                            ghost.GetComponent<Animator>().SetBool("isScared", false);
                            ghost.GetComponent<Animator>().SetBool("isRecovering", false);
                            ghost.GetComponent<Animator>().SetBool("isDead", true);
                        }
                    }
                        

                }

                else if (PacStudentController.CurrentState == PacStudentController.PacState.Normal)
                {
                    ghost.GetComponent<Animator>().SetBool("isScared", false);
                    ghost.GetComponent<Animator>().SetBool("isRecovering", false);
                    state[ghost] = GhostState.Normal;
                }

                //Check if ghost is in its spawn spot
                if (ghost.transform.position == startPosition[ghost])
                {
                    //Dead state machine
                    if (ghost.GetComponent<Animator>().GetBool("isDead"))
                    {
                        //Set ghost to not dead
                        ghost.GetComponent<Animator>().SetBool("isDead", false);
                        state[ghost] = GhostState.Normal;

                        //Set ghost to normal is pacman is normal
                        if (PacStudentController.CurrentState == PacStudentController.PacState.Normal)
                        {
                            ghost.GetComponent<Animator>().SetBool("isScared", false);
                            ghost.GetComponent<Animator>().SetBool("isRecovering", false);
                        }
                        //Set ghost to scared/recovering if pacman is powered
                        else if (PacStudentController.CurrentState == PacStudentController.PacState.Powered)
                        {
                            if (PacStudentController.PowerTime <= 3.0f)
                            {
                                ghost.GetComponent<Animator>().SetBool("isScared", true);
                                ghost.GetComponent<Animator>().SetBool("isRecovering", false);
                            }
                            else
                            {
                                ghost.GetComponent<Animator>().SetBool("isScared", false);
                                ghost.GetComponent<Animator>().SetBool("isRecovering", true);
                            }
                        }
                    }
                }

                //Update ghost spawn check
                spawnZoneCheck(ghost);

                //Getting out of spawn state machine
                if (spawnZone[ghost])
                {
                    if (ghost.transform.position == startPosition[ghosts[3]])
                    {
                        if (spawnZone[ghosts[2]])
                            ghostTweener.AddTween(ghost.transform, ghost.transform.position, startPosition[ghosts[2]], ghostDuration * 2);

                        else
                            ghostTweener.AddTween(ghost.transform, ghost.transform.position, leaveSpawnTop1, ghostDuration * 3);
                    }
                    else if (ghost.transform.position == startPosition[ghosts[2]])
                    {
                        ghostTweener.AddTween(ghost.transform, ghost.transform.position, leaveSpawnBottom1, ghostDuration * 2);
                    }

                    else if (ghost.transform.position == startPosition[ghosts[1]])
                    {
                        if (spawnZone[ghosts[0]])
                            ghostTweener.AddTween(ghost.transform, ghost.transform.position, startPosition[ghosts[0]], ghostDuration * 2);
                        else
                            ghostTweener.AddTween(ghost.transform, ghost.transform.position, leaveSpawnBottom1, ghostDuration * 2);
                    }

                    if (ghost.transform.position == startPosition[ghosts[0]])
                    {
                        ghostTweener.AddTween(ghost.transform, ghost.transform.position, leaveSpawnTop1, ghostDuration * 2);
                    }


                    else if (ghost.transform.position == leaveSpawnTop1)
                    {
                        ghostTweener.AddTween(ghost.transform, ghost.transform.position, leaveSpawnTop2, ghostDuration * 2);
                    }

                    else if (ghost.transform.position == leaveSpawnBottom1)
                        ghostTweener.AddTween(ghost.transform, ghost.transform.position, leaveSpawnBottom2, ghostDuration * 2);
                }

                else
                {
                    //Set up the ghost movement

                    //List of potential directions
                    List<Vector2> directions = new List<Vector2>();
                    directions.Add(Vector2.up * 4);
                    directions.Add(Vector2.left * 4);
                    directions.Add(Vector2.down * 4);
                    directions.Add(Vector2.right * 4);

                    //Reduce viable moves

                    //Remove laststep
                    if (lastStep.ContainsKey(ghost))
                    {
                        directions.Remove(lastStep[ghost]);
                    }

                    foreach (Vector2 direction in directions.ToList())
                    {
                        //Remove the direction if its in the spawn zone
                        Vector3 spawnCheckPosition = new Vector3(ghost.transform.position.x + direction.x, ghost.transform.position.y + direction.y, ghost.transform.position.z);
                        if (inSpawnZone(spawnCheckPosition))
                            directions.Remove(direction);

                        //Remove the direction if its facing a wall
                        if (hitWall(ghost, direction))
                            directions.Remove(direction);

                        //Remove the direction if its next to a teleporter
                        if (spawnCheckPosition.x <= leftTeleport.x)
                        {
                            directions.Remove(direction);
                            lastStep[ghost] = Vector2.left * 4;
                        }
                        else if (spawnCheckPosition.x >= rightTeleport.x)
                        {
                            directions.Remove(direction);
                            lastStep[ghost] = Vector2.right * 4;
                        }

                        //Remove direction if its next to another ghost
                        if (hitGhost(ghost, direction))
                        {
                            directions.Remove(direction);
                        }
                            

                    }

                    //Move ghost backwards if there are no potential directions
                    if (!directions.Any())
                    {
                        ghostTweener.AddTween(ghost.transform, ghost.transform.position, ghost.transform.position + new Vector3(lastStep[ghost].x, lastStep[ghost].y, 0), ghostDuration);

                        if (ghostTweener.compareEndPos(ghost.transform.position + new Vector3(lastStep[ghost].x, lastStep[ghost].x, 0)))
                        {
                            if (lastStep.ContainsKey(ghost))
                                lastStep[ghost] = directionSwap(lastStep[ghost]);
                            else
                                lastStep.Add(ghost, directionSwap(lastStep[ghost]));
                            updateMovementAnimation(ghost, lastStep[ghost]);
                        }
                    }
                    else
                    {
                        if (state[ghost] == GhostState.Normal)
                        {
                            //Ghost 0 move further away
                            if (ghost == ghosts[0])
                            {
                                Vector2 direction = maxDistanceFromPacman(ghost, directions);
                                ghostTweener.AddTween(ghost.transform, ghost.transform.position, ghost.transform.position + new Vector3(direction.x, direction.y, 0), ghostDuration);

                                if (ghostTweener.compareEndPos(ghost.transform.position + new Vector3(direction.x, direction.y, 0)))
                                {
                                    if (lastStep.ContainsKey(ghost))
                                        lastStep[ghost] = directionSwap(direction);
                                    else
                                        lastStep.Add(ghost, directionSwap(direction));
                                    updateMovementAnimation(ghost, direction);
                                }
                            }
                            //Ghost 1 move closer
                            else if (ghost == ghosts[1])
                            {
                                Vector2 direction = minDistanceFromPacman(ghost, directions);
                                ghostTweener.AddTween(ghost.transform, ghost.transform.position, ghost.transform.position + new Vector3(direction.x, direction.y, 0), ghostDuration);

                                if (ghostTweener.compareEndPos(ghost.transform.position + new Vector3(direction.x, direction.y, 0)))
                                {
                                    if (lastStep.ContainsKey(ghost))
                                        lastStep[ghost] = directionSwap(direction);
                                    else
                                        lastStep.Add(ghost, directionSwap(direction));
                                    updateMovementAnimation(ghost, direction);
                                }
                            }
                            //Ghost 2 move randomly
                            else if (ghost == ghosts[2])
                            {
                                Vector2 direction = randomMove(ghost, directions);
                                ghostTweener.AddTween(ghost.transform, ghost.transform.position, ghost.transform.position + new Vector3(direction.x, direction.y, 0), ghostDuration);

                                if (ghostTweener.compareEndPos(ghost.transform.position + new Vector3(direction.x, direction.y, 0)))
                                {
                                    if (lastStep.ContainsKey(ghost))
                                        lastStep[ghost] = directionSwap(direction);
                                    else
                                        lastStep.Add(ghost, directionSwap(direction));
                                    updateMovementAnimation(ghost, direction);
                                }
                            }

                            //Ghost 3 move clockwise
                            else if (ghost == ghosts[3])
                            {
                                if (new Vector2(ghost.transform.position.x, ghost.transform.position.y) == pinkGhostPosition[currentPinkTarget])
                                    currentPinkTarget = currentPinkTarget + 1;

                                Vector3 direction = minDistanceFromPinkTarget(ghost, directions);

                                ghostTweener.AddTween(ghost.transform, ghost.transform.position, ghost.transform.position + new Vector3(direction.x, direction.y, 0), ghostDuration);

                                if (ghostTweener.compareEndPos(ghost.transform.position + new Vector3(direction.x, direction.y, 0)))
                                {
                                    if (lastStep.ContainsKey(ghost))
                                        lastStep[ghost] = directionSwap(direction);
                                    else
                                        lastStep.Add(ghost, directionSwap(direction));
                                    updateMovementAnimation(ghost, direction);
                                }
                            }

                        }
                        else if (state[ghost] == GhostState.Scared || state[ghost] == GhostState.Recovering)
                        {
                            Vector2 direction = maxDistanceFromPacman(ghost, directions);
                            ghostTweener.AddTween(ghost.transform, ghost.transform.position, ghost.transform.position + new Vector3(direction.x, direction.y, 0), ghostDuration);

                            if (ghostTweener.compareEndPos(ghost.transform.position + new Vector3(direction.x, direction.y, 0)))
                            {
                                if (lastStep.ContainsKey(ghost))
                                    lastStep[ghost] = directionSwap(direction);
                                else
                                    lastStep.Add(ghost, directionSwap(direction));
                            }
                        }

                        else if (state[ghost] == GhostState.Dead)
                        {
                            ghostTweener.RemoveTween(ghost.transform);
                            ghostTweener.AddTween(ghost.transform, ghost.transform.position, startPosition[ghost], 3);
                        }
                    }
                }
            }
        }

        
    }

    private Vector2 minDistanceFromPinkTarget(GameObject ghost, List<Vector2> directions)
    {
        Vector2 minDirection = new Vector2(ghost.transform.position.x, ghost.transform.position.y);
        float minDistance = 1000;
        Vector2 targetPosition = pinkGhostPosition[currentPinkTarget];
        Vector2 ghostPosition = new Vector2(ghost.transform.position.x, ghost.transform.position.y);

        foreach (Vector2 direction in directions.ToList())
        {
            float currentDistance = Vector2.Distance(targetPosition, ghostPosition + direction);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                minDirection = direction;
            }
        }

        return minDirection;
    }

    private Vector2 directionSwap(Vector2 direction)
    {
        Vector2 directionToAvoid = direction;
        if (direction == Vector2.up * 4)
            directionToAvoid = Vector2.down * 4;
        else if (direction == Vector2.down * 4)
            directionToAvoid = Vector2.up * 4;
        if (direction == Vector2.left * 4)
            directionToAvoid = Vector2.right * 4;
        if (direction == Vector2.right * 4)
            directionToAvoid = Vector2.left * 4;


        return directionToAvoid;
    }

    private void updateMovementAnimation(GameObject ghost, Vector2 direction)
    {
        if (direction == Vector2.up * 4)
        {
            ghost.GetComponent<Animator>().SetBool("isUp", true);
            ghost.GetComponent<Animator>().SetBool("isDown", false);
            ghost.GetComponent<Animator>().SetBool("isLeft", false);
            ghost.GetComponent<Animator>().SetBool("isRight", false);
        }
    }

    private Vector2 maxDistanceFromPacman(GameObject ghost, List<Vector2> directions)
    {
        Vector2 maxDirection = new Vector2(ghost.transform.position.x, ghost.transform.position.y);
        float maxDistance = 0;
        Vector2 pacPosition = new Vector2(pacman.transform.position.x, pacman.transform.position.y);
        Vector2 ghostPosition = new Vector2(ghost.transform.position.x, ghost.transform.position.y);

        foreach (Vector2 direction in directions.ToList())
        {
            float currentDistance = Vector2.Distance(pacPosition, ghostPosition + direction);
            if (currentDistance > maxDistance)
            {
                maxDistance = currentDistance;
                maxDirection = direction;
            }
        }

        return maxDirection;
    }

    private Vector2 minDistanceFromPacman(GameObject ghost, List<Vector2> directions)
    {
        Vector2 minDirection = new Vector2(ghost.transform.position.x, ghost.transform.position.y);
        float minDistance = 1000;
        Vector2 pacPosition = new Vector2(pacman.transform.position.x, pacman.transform.position.y);
        Vector2 ghostPosition = new Vector2(ghost.transform.position.x, ghost.transform.position.y);

        foreach (Vector2 direction in directions.ToList())
        {
            float currentDistance = Vector2.Distance(pacPosition, ghostPosition + direction);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                minDirection = direction;
            }
        }

        return minDirection;
    }

    private Vector2 randomMove(GameObject ghost, List<Vector2> directions)
    {
        int randomElement = Random.Range(0, directions.Count);
        return directions[randomElement];
    }

    private void spawnZoneCheck(GameObject ghost)
    {
        if (ghost.transform.position.x >= spawnZoneStart.x && ghost.transform.position.x <= spawnZoneEnd.x && ghost.transform.position.y <= spawnZoneStart.y && ghost.transform.position.y >= spawnZoneEnd.y)
            spawnZone[ghost] = true;
        else
            spawnZone[ghost] = false;
    }

    private bool inSpawnZone(Vector3 position)
    {
        if (position.x >= spawnZoneStart.x && position.x <= spawnZoneEnd.x && position.y <= spawnZoneStart.y && position.y >= spawnZoneEnd.y)
            return true;
        return false;
    }

    private bool hitWall (GameObject ghost, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(ghost.transform.position, direction, 4.0f, LayerMask.GetMask("Walls"));

        return hit.collider != null;
    }

    private bool hitPacman (GameObject ghost, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(ghost.transform.position, direction, 1.8f, LayerMask.GetMask("Pacman"));

        return hit.collider != null;
    }

    private bool hitGhost(GameObject ghost, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(ghost.transform.position + new Vector3(direction.x, direction.y, 0), direction, 2.0f, LayerMask.GetMask("Ghosts"));

        return hit.collider != null;
    }
}
