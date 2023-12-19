using System.Runtime.InteropServices;
using UnityEngine;

namespace KSPDropShadow
{
    public class DropShadowModule : VesselModule
    {
        public float width = 1;
        public float height = 1;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private Mesh mesh;
        private GameObject shadowObject;
        private RaycastHit hit;
        private bool didHit = false;

        public new void Start() {
            base.Start();

            if (!vessel.isActiveVessel) {
                return;
            }

            shadowObject = new GameObject(vessel.GetName() + " shadow");

            meshRenderer = shadowObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

            meshFilter = shadowObject.AddComponent<MeshFilter>();

            mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-width/2.0f, -height/2.0f, 0.0f),
                new Vector3(width/2.0f, -height/2.0f, 0.0f),
                new Vector3(-width/2.0f, height/2.0f, 0.0f),
                new Vector3(width/2.0f, height/2.0f, 0.0f)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.uv = uv;

            meshFilter.mesh = mesh;
        }

        public void Update() {
            if (vessel.isActiveVessel) {
                if (didHit) {
                    meshRenderer.enabled = true;
                    meshRenderer.transform.localScale = new Vector3(width, height, 0.0f);
                    meshRenderer.transform.position = hit.point + (hit.normal * 0.05f);
                    meshRenderer.transform.rotation = Quaternion.LookRotation(-hit.normal, vessel.orbit.Prograde(Planetarium.GetUniversalTime()));
                } else {
                    meshRenderer.enabled = false;
                }
                //Debug.Log("Test " + hit.distance + " " + width + " " + height);
            }
        }

        public void FixedUpdate() {
            //Vector3d rayDir = vessel.mainBody.position - vessel.CoMD;
            //rayDir.Normalize();
            Bounds b = calcBoundingBox(vessel);
            width = b.max.x - b.min.x;
            height = b.max.z - b.min.z;
            didHit = Physics.Raycast(vessel.CoMD, -vessel.up, out hit, 10000.0f, (1 << 15) | (1 << 4));
        }

        private Bounds calcBoundingBox(Vessel v) {
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;
            for (int i=0; i<v.parts.Count; i++) {
                Part part = v.parts[i];
                //Bounds partBounds = part.GetPartRendererBound();
                Bounds partBounds = new Bounds();
                partBounds.SetMinMax(part.transform.position, part.transform.position);
                if (partBounds.min.x < min.x) {
                    min.x = partBounds.min.x;
                }
                if (partBounds.min.y < min.y) {
                    min.y = partBounds.min.y;
                }
                if (partBounds.min.z < min.z) {
                    min.z = partBounds.min.z;
                }

                if (partBounds.max.x > max.x) {
                    max.x = partBounds.max.x;
                }
                if (partBounds.max.y > max.y) {
                    max.y = partBounds.max.y;
                }
                if (partBounds.max.z > max.z) {
                    max.z = partBounds.max.z;
                }
            }

            Bounds vesselBounds = new Bounds();
            vesselBounds.SetMinMax(min, max);
            return vesselBounds;
        }
    }
}
