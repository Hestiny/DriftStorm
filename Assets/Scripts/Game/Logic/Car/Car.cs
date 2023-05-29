using DriftStorm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DriftStorm
{
    public class Car : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _theRg;

        [Header("尾气粒子")]
        [SerializeField]
        private Transform _backLeft;
        [SerializeField]
        private Transform _backRight;
        [SerializeField]
        private Transform _frontLeft;
        [SerializeField]
        private Transform _frontRight;

        [Header("模型节点")]
        [SerializeField]
        private Transform _modeParent;

        [Header("汽车配置")]
        [SerializeField]
        private float maxSpeed;
        [SerializeField]
        private float forwardAccel = 8f;
        [SerializeField]
        private float reverseAccel = 4f;
        [SerializeField]
        private float turnStrength = 180f;


        private Transform _wheelBackLeft;
        private Transform _wheelBackRight;
        private Transform _wheelFrontLeft;
        private Transform _wheelFrontRight;

        private void Awake()
        {
            _theRg = transform.GetComponent<Rigidbody>();
            _modeParent = transform.Find("modeParent");
        }

        private void Update()
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                _speedInput = Input.GetAxis("Vertical") * forwardAccel;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                _speedInput = Input.GetAxis("Vertical") * reverseAccel;
            }

            _turnInput = Input.GetAxis("Horizontal");

        }

        private void FixedUpdate()
        {
            Run();
        }

        #region ====初始化====
        /// <summary>
        /// 初始化赛车
        /// </summary>
        /// <param name="carId"></param>
        public void Init(int carId)
        {
            InitCarConfig(carId);
            InitModle(carId);
        }

        #region ==初始化模型和节点==
        /// <summary>
        /// 初始化汽车模型
        /// </summary>
        /// <param name="carid"></param>
        public void InitModle(int carid)
        {
            var carObj = LoadCtrl.Instance.LoadPrefab<GameObject>(GameConfig.GetCarNameById(carid), _modeParent);
            if (carObj == null)
            {
                DebugCtrl.LogError($"汽车模型{carid}未正常加载");
            }
            _wheelBackLeft = carObj.transform.Find("wheel_backLeft");
            _wheelBackRight = carObj.transform.Find("wheel_backRight");
            _wheelFrontLeft = carObj.transform.Find("wheel_frontLeft");
            _wheelFrontRight = carObj.transform.Find("wheel_frontRight");
            UdateParticlePos();

        }

        /// <summary>
        /// 更新粒子特效位置
        /// </summary>
        public void UdateParticlePos()
        {
            _backLeft.position = _wheelBackLeft.position;
            _backRight.position = _wheelBackRight.position;
            _frontLeft.position = _wheelFrontLeft.position;
            _frontRight.position = _wheelFrontRight.position;
        }
        #endregion

        #region ==初始化赛车配置==
        public void InitCarConfig(int carId)
        {

        }
        #endregion 
        #endregion

        #region ====模型动画相关====
        /// <summary>
        /// 更新轮胎旋转
        /// </summary>
        public void UpdateWheel()
        {

        }
        #endregion

        #region ====运动相关====
        /// <summary>
        /// 更新汽车状态 比如是否在地面等
        /// </summary>
        private void UpdateCarState()
        {

        }

        /// <summary>
        /// 根据运动状态来调整
        /// </summary>
        private void ControllerDrag()
        {

        }

        /// <summary>
        /// 调整所有状态和参数后对赛车受力
        /// </summary>
        private void Run()
        {
            if (SpeedInput != 0)
            {
                _theRg.AddForce(transform.forward * _speedInput * 1000f);

                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, _turnInput * turnStrength * Time.deltaTime * Mathf.Sign(_speedInput) * (_theRg.velocity.magnitude / maxSpeed), 0f));
            }

        }
        #endregion

        #region ====输出控制====
        private float _speedInput;
        private float _turnInput;

        public float SpeedInput { get => _speedInput; set => _speedInput = value; }
        public float TurnInput { get => _turnInput; set => _turnInput = value; }
        #endregion
    }
}


