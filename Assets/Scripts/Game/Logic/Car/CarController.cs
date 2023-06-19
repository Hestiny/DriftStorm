using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DriftStorm
{
    public class CarController : MonoBehaviour
    {
        [SerializeField]
        private Car _car;
        public bool IsAI = true;

        //检查点
        public List<Transform> _checkPos = new List<Transform>();
        //下一个目标点 targetpoint=_checkpos[currenttarget]
        private Vector3 targetPoint;
        //当前目标点索引
        public int currentTarget = 0;

        [Header("AI输出控制")]
        public float aiAccelerateSpeed = 1f;
        public float aiTurnSpeed = .8f;
        public float aiReachPointRange = 5f;
        public float aiPointVariance = 3f;
        public float aiMaxTurn = 15f;

        private void Awake()
        {

        }

        private void Start()
        {
            
            //targetPoint = _checkPos[currentTarget].transform.position;
            //RandomiseAITarget();
        }

        private void Update()
        {
            if (!IsAI)
            {
                _car.SpeedInput = Input.GetAxis("Vertical");
                _car.TurnInput = Input.GetAxis("Horizontal");
            }
            else
            {
                AIInput();
            }
        }

        private void AIInput()
        {
            targetPoint.y = transform.position.y;

            if (Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
            {
                SetNextAITarget();
            }

            Vector3 targetDir = targetPoint - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);

            Vector3 localPos = transform.InverseTransformPoint(targetPoint);
            if (localPos.x < 0f)
            {
                angle = -angle;
            }

            _car.TurnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);
        }

        public void Init(int carId, List<Transform> checkPos)
        {
            _car.Init(carId);
            _checkPos = checkPos;
        }

        public void SetNextAITarget()
        {
            currentTarget++;
            if (currentTarget >= _checkPos.Count)
            {
                currentTarget = 0;
            }

            targetPoint = _checkPos[currentTarget].transform.position;
            //RandomiseAITarget();
        }
    }
}

