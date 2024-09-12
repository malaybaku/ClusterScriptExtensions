using System;
using UnityEngine;

namespace Baxter.ClusterScriptExtensions.Internal
{
    public class InspectorLookChecker : MonoBehaviour
    {
        public enum MyPatterns
        {
            Default,
            なんかオプション,
            Three,
        }

        [Flags]
        public enum MyOptions
        {
            None = 0x0000,
            Bold = 0x0001,
            Italic = 0x0002, 
            打ち消し線 = 0x0004,
            Underline = 0x0008,
            上にドット = 0x0010,
        }
        
        // 1st release
        [SerializeField] private bool myBool;
        [SerializeField] private int myInt;
        [SerializeField] private float myFloat;
        [SerializeField] private string myText;
        [SerializeField] private Vector2 myVector2;
        [SerializeField] private Vector3 myVector3;
        [SerializeField] private Quaternion myRotation;

        // 1st release に含んでいないがケーススタディになる事例集
        [TextArea] [SerializeField] private string myLongText;
        [Range(-3, 3)] [SerializeField] private int myRangeInt = 5;
        [Range(0.1f, 5.0f)] [SerializeField] private float myRangeFloat = 1.0f;

        [SerializeField] private MyPatterns myEnumValue;
        [SerializeField] private MyOptions myFlagEnumValue;
    }
}
