using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MartinsOdabi
{
    public static class ExtensionMethods
    {
        public static Vector2 Abs(this Vector2 _vector)
        {
            float _x = Mathf.Abs(_vector.x);
            float _y = Mathf.Abs(_vector.y);

            return new Vector2(_x, _y);
        }

        public static Vector3 Abs(this Vector3 _vector)
        {
            float _x = Mathf.Abs(_vector.x);
            float _y = Mathf.Abs(_vector.y);
            float _z = Mathf.Abs(_vector.y);

            return new Vector3(_x, _y, _z);
        }
    }
}