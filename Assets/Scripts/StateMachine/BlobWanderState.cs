using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class BlobWanderState : BlobBaseState
    {
        private bool _hasPatrolPoint;
        private Vector3 _targetDestination;
        private Collider[] _cachedPatrolColliders;
        private float _patrolWaitTimer;
        private const int MaxColliders = 10;
        private bool _showDebug;

        // Constructor
        public BlobWanderState(BlobStateManager context) : base(context) { }

        #region Required State Methods
        public override void EnterState()
        {
            if (context.Blob.NavMeshAgent.isStopped)
            {
                context.Blob.NavMeshAgent.isStopped = false;
            }
            else
            {
                _hasPatrolPoint = false;
            }
        }

        public override void UpdateState()
        {
            if (context.IsTransformed) return;

            if (PatrolWaitCheck() == false) return; // Guard

            if (!_hasPatrolPoint) SearchForPatrolPoint();

            if (_hasPatrolPoint) context.Blob.NavMeshAgent.SetDestination(_targetDestination);

            Vector3 distance = context.transform.position - _targetDestination;
            if (distance.magnitude < context.Blob.PatrolDistanceThreshold)
            {
                // Patrol point reached
                _hasPatrolPoint = false;
                context.Blob.NavMeshAgent.ResetPath(); // Clear path in navMeshAgent

                RestartWaitTimer(context.Blob.PatrolWaitTime);
            }
        }

        public override void OnTriggerEnter(Collider other)
        {
            context.SwitchState(BlobState.Dead);
        }

        public override void ExitState() { }
        #endregion

        #region Patrol/Wander
        private bool PatrolWaitCheck()
        {
            if (Time.time < _patrolWaitTimer) return false; // Timer running
            // Timer complete
            return true;
        }

        private void SearchForPatrolPoint()
        {
            float randomX = Random.Range(-context.Blob.PatrolRadius, context.Blob.PatrolRadius);
            float randomZ = Random.Range(-context.Blob.PatrolRadius, context.Blob.PatrolRadius);

            Vector3 cachedPosition = context.transform.position;
            var randomPosition = new Vector3(cachedPosition.x + randomX, cachedPosition.y, cachedPosition.z + randomZ);

            // Check - random position is NOT inside another collider
            _cachedPatrolColliders ??= new Collider[MaxColliders];
            int numColliders = Physics.OverlapBoxNonAlloc(randomPosition, Vector3.one * 0.5f, _cachedPatrolColliders,
                Quaternion.identity, context.Blob.AvoidMask);
            if (numColliders > 0) return;

            // Check - random position is above the ground terrain
            if (!Physics.Raycast(randomPosition, Vector3.down, 2f, context.Blob.GroundMask)) return;

            if (_showDebug)
            {
                var debugObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                debugObject.transform.localScale = Vector3.one * 0.2f;
                Debug.DrawLine(context.transform.position, randomPosition, Color.red, 1f);
            }

            _hasPatrolPoint = true;
            _targetDestination = randomPosition;
        }

        private void RestartWaitTimer(Vector2 timeRange)
        {
            float randomWaitTime = Random.Range(timeRange.x, timeRange.y);
            _patrolWaitTimer = Time.time + randomWaitTime;
        }
        #endregion

        // ––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––––
        #region Unused Patrol Code - Not working, but would be great if it did.
        private void GetNewPatrolPoint()
        {
            Vector3 randomPosition = RandomNavSphere(context.transform.position, context.Blob.PatrolRadius);

            Debug.Log($"{context.name} - Random Position: {randomPosition}");

            _hasPatrolPoint = true;
            _targetDestination = randomPosition;
        }

        private Vector3 RandomNavSphere(Vector3 origin, float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += origin;

            NavMesh.SamplePosition(new Vector3(randomDirection.x, origin.y, randomDirection.z), out NavMeshHit navHit,
                radius, 1);

            return navHit.position;
        }
        #endregion
    }
}