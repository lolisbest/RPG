//#define Test

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Common;

//namespace RPG.Player
//{
//    public class PlayerInputManager : Singleton<PlayerInputManager>
//    {
//        public VariableJoystick joystick;
//        public float X { get; private set; } = 0f;
//        public float Z { get; private set; } = 0f;

//        #region Buttons
//        [SerializeField]
//        private bool button0 = false;
//        [SerializeField]
//        private bool button1 = false;
//        [SerializeField]
//        private bool button2 = false;
//        #endregion

//        #region Button Property

//        public bool Button0 { 
//            get
//            {
//                if (button0)
//                {
//                    button0 = false;
//                    return true;
//                }
//                return false;
//            }
//            private set
//            {
//                button0 = value;
//            }
//        }

//        public bool Button1
//        {
//            get
//            {
//                if (button1)
//                {
//                    button1 = false;
//                    return true;
//                }
//                return false;
//            }
//            private set
//            {
//                button1 = value;
//            }
//        }

//        public bool Button2
//        {
//            get
//            {
//                if (button2)
//                {
//                    button2 = false;
//                    return true;
//                }
//                return false;
//            }
//            private set
//            {
//                button2 = value;
//            }
//        }
//        #endregion

//        public float x;
//        public float z;

//        public float delta = 0.1f;

//        private void Update()
//        {
//#if Test
//            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
//            {
//                //Z = 0f;
//            }
//            else if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
//            {
//                Z += delta;
//                Z = Mathf.Clamp(Z, -1f, 1f);
//            }
//            else if (!Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
//            {
//                Z -= delta;
//                Z = Mathf.Clamp(Z, -1f, 1f);
//            }
//            else if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
//            {
//                Z = 0f;
//            }

//            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
//            {
//                X = 0f;
//            }
//            else if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
//            {
//                X += delta;
//            }
//            else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
//            {
//                X -= delta;
//            }
//            else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
//            {
//                X = 0f;
//            }

//            Vector2 vec;

//            if (Input.GetKeyDown(KeyCode.UpArrow))
//            {
//                Debug.Log("UpArrow");
//                Button0 = true;
//            }

//            if (Input.GetKeyDown(KeyCode.LeftArrow))
//            {
//                Debug.Log("LeftArrow");
//                Button1 = true;
//            }

//            if (Input.GetKeyDown(KeyCode.DownArrow))
//            {
//                Debug.Log("DownArrow");
//                Button2 = true;
//            }

//            vec = new Vector2(X, Z);
//            if (vec.sqrMagnitude > 1f)
//            {
//                vec = vec.normalized;
//            }

//            X = vec.x;
//            Z = vec.y;
//#else
//            X = joystick.Horizontal;
//            Z = joystick.Vertical;
//#endif

//            x = X;
//            z = Z;

//        }

//        public void InitXZ()
//        {
//            X = 0f;
//            Z = 0f;
//        }

//        public override void Initialize()
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}