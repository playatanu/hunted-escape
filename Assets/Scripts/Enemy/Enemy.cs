using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{


    public enum enemyState
    {
        walk, chase, attack
    }

    [Header("Player Following")]
    public Transform playerBody;
    [Range(1f, 15f)]
    public float followDistance;
    [Range(1f, 5f)]
    public float followSpeed;

    [Header("Random Walk")]
    [Range(1f, 5f)]
    public float walkSpeed;
    public List<Transform> randomPostions;
    public LayerMask layerMask;
    public Transform enemyEye;

    Vector3 _newDestination;
    NavMeshAgent _navMashAgent;

    Animator _anim;
    RaycastHit _hit;

    bool _allreadyRandomPostion, _allreadyPlayerFollow;

    void Start()
    {
        _navMashAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        GoToRandomPostion();
    }

    void Update()
    {
        HandelEnemyFollow();
        HandelEnemyAttack();
    }



    void HandelEnemyAttack()
    {
        if (Physics.Raycast(enemyEye.position, enemyEye.TransformDirection(Vector3.forward), out _hit, 3.5f, layerMask))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Movement>().SetPlayerSpeed(0f);
                _anim.SetTrigger("Attack");
            }
        }
    }


    IEnumerator ActiveGameOverPanel(float time)
    {
        yield return new WaitForSeconds(time);

        LevelManager.instance.SetGameoverPanel(true);
        AudioManager.instance.PlayEnemyChaseSound(value: false);
        AudioManager.instance.playBackGroundSound(false);
        //gameObject.SetActive(false);
    }

    public void Attack()
    {

        StartCoroutine(ActiveGameOverPanel(2));
    }

    void HandelEnemyFollow()
    {
        float playerDistance = Vector3.Distance(transform.position, playerBody.transform.position);

        if (playerDistance < followDistance)
        {
            if (playerDistance < 3.5f)
            {
            }
            else
            {
                GoToPlayerPostion();
            }
        }
        else
        {
            GoToRandomPostion();
        }
    }



    /* Enemy Movement */
    public void GoToPlayerPostion()
    {
        print("Go to Player");
        if (!_allreadyPlayerFollow)
            AudioManager.instance.PlayEnemyChaseSound(true);
        _navMashAgent.stoppingDistance = 3f;
        _navMashAgent.speed = followSpeed;
        _anim.SetFloat("Blend", 1f);
        _navMashAgent.destination = playerBody.position;
        _allreadyRandomPostion = false;
        _allreadyPlayerFollow = true;
    }

    public void GoToRandomPostion()
    {
        // If Enemy not go for random postion
        if (!_allreadyRandomPostion)
        {

            print("Go to Random");
            AudioManager.instance.PlayEnemyChaseSound(false);
            _navMashAgent.speed = walkSpeed;
            _newDestination = randomPostions[GetRandomRange()].position;
            _navMashAgent.stoppingDistance = 1.5f;
            _anim.SetFloat("Blend", 0.5f);
            _navMashAgent.destination = _newDestination;
            _allreadyRandomPostion = true;
            _allreadyPlayerFollow = false;
        }

    }






    // Get Uniqe Random Postion In Range
    int _prvRange;
    int GetRandomRange()
    {
        int _newRange = Random.Range(0, randomPostions.Count);
        if (_prvRange == _newRange)
        {
            GetRandomRange();
        }
        else
        {
            _prvRange = _newRange;
        }
        return _newRange;
    }

    // If Enemy on Random postion
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Random"))
        {
            print("trigger");
            _allreadyRandomPostion = false;
            GoToRandomPostion();

        }
    }






}