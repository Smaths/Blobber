using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class BlobPatrolState : BlobBaseState
    {
        private bool _hasPatrolPoint;
        private Vector3 _targetDestination;
        private Collider[] _cachedPatrolColliders;
        private Collider[] _cachedSearchColliders;
        private bool _showDebug;

        private float _patrolWaitTimer;

        private const int MaxColliders = 10;

        // Constructor
        public BlobPatrolState(BlobStateManager context) : base(context) { }

        // Required State Methods
        public override void EnterState()
        {
            _hasPatrolPoint = false;
        }

        public override void UpdateState()
        {
            SearchForPlayer();

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

        public override void FixedUpdateState() { }

        public override void ExitState() { }

        public override void OnCollisionEnter(Collision other)
        {
            Debug.Log($"Collision Enter {nameof(BlobPatrolState)}");
            context.SwitchState(BlobState.Dead);
        }

        // Patrol private implementation

        #region Patrol Logic
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

        #region Search for Player Blob
        private void SearchForPlayer()
        {
            _cachedSearchColliders ??= new Collider[MaxColliders];
            int numColliders = Physics.OverlapSphereNonAlloc(context.transform.position, context.Blob.SightRange,
                _cachedSearchColliders, context.Blob.SightMask);
            if (numColliders > 0)
            {
                // Target found
                context.SwitchState(BlobState.Chase);
            }
            // No target found
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