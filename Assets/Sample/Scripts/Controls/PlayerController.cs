using System;
using Framework.Scripts.Objects;
using UnityEngine;

namespace Sample.Scripts.Controls
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField] private GraphNode selectedStructure;


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(selectedStructure, 
                    GetMouseWorldPositionByPlane(Camera.main, new Plane(Vector3.up, 0), true),
                    Quaternion.identity);
            }
        }

        //https://github.com/Math-Man/Proving-Utilities/blob/main/Runtime/HelperLibs/STHelper.cs
        public static Vector3 GetMouseWorldPositionByPlane(Camera camera, Plane plane, bool useDefaultPlane = true)
        {
            if (useDefaultPlane)
                plane = new Plane(Vector3.up, 0);

            float distance;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            var worldPosition = new Vector3();
            if (plane.Raycast(ray, out distance))
                worldPosition = ray.GetPoint(distance);

            return worldPosition;
        }
    }
}