using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PS
{
    [ExecuteInEditMode]
    public class TriAngleSkill : MonoBehaviour
    {
        public float jumpForce = 5f;// 위로 띄우는 힘 
        public float damage = 30; //스킬 데미지 
        public float distance = 10f; //길이
        public float angle = 30f; //각도
        public float height = 1; //높이  --> 3차원 삼각형이라 ㅇㅇ 
        public Color meshColor = Color.red; //시야에 보여질 색깔 
        public LayerMask layerMask;
        public List<GameObject> gameObjects = new List<GameObject>();
        public Collider[] colliders = new Collider[3];
        [SerializeField]
        private int playerCount;
        private Mesh mesh;//매쉬를 그릴거임 

        private void OnEnable()
        {
            
        }
        private void OnDisable()
        {
            //TriangleCheck();
        }
        private void Start()
        {
            //TriangleCheck();
        }
        void Update()
        {
           TriangleCheck();
        }

        public void TriangleCheck()
        {
           
            playerCount = Physics.OverlapSphereNonAlloc(transform.position,distance, colliders,layerMask ,QueryTriggerInteraction.Collide);
            
            gameObjects.Clear();

            for (int i = 0; i < playerCount; ++i)
            {
                GameObject player = colliders[i].gameObject;

                if (IsInSight(player) == true)
                {
                    gameObjects.Add(player);
                    gameObjects[i].GetComponent<Player>().PlayerHealth.TakeDamage(damage ,Vector3.zero);
                    gameObjects[i].GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
                    this.enabled = false;
                }
            }
        }
        public bool IsInSight(GameObject obj)
        {
            Vector3 origin = this.transform.position;
            Vector3 dest = obj.transform.position;
            Vector3 direction = dest - origin;
            if (direction.y < 0 || direction.y > height)
            {
                return false;
            }

            direction.y = 0;

            float deltaAngle =Vector3.Angle(direction, transform.forward);

            if (deltaAngle > angle)
            {
                return false;
            }

            return true;
        }

        private Mesh CreateMesh()
        { 
            Mesh mesh = new Mesh();
            int segments = 10;
            int numTriangle = (segments * 4)+2+2;
            int numVertices = numTriangle * 3; //정점수 ? 도형의 꼭지점인가?

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 bottomCenter = Vector3.zero; // 삼각형 기준으로 3개의 점중 가운데 점 
            Vector3 bottomLeft  =Quaternion.Euler(0,-angle,0) * Vector3.forward * distance;
            Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

            Vector3 TopCenter = bottomCenter+ Vector3.up * height; //아래도 똑같이
            Vector3 TopRight = bottomRight + Vector3.up * height;
            Vector3 TopLeft = bottomLeft + Vector3.up * height;

            #region 꼭지점 할당 
            int vert = 0;
            //left
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = TopLeft;

            vertices[vert++] = TopLeft;
            vertices[vert++] = TopCenter;
            vertices[vert++] = bottomCenter;
            //right
            vertices[vert++] = bottomCenter;
            vertices[vert++] = TopCenter;
            vertices[vert++] = TopRight;

            vertices[vert++] = TopRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            float currnetAngle = -angle;
            float deltaAngle = (angle * 2)/segments;
            for (int i = 0; i < segments; ++i)
            {

                bottomCenter = Vector3.zero; // 삼각형 기준으로 3개의 점중 가운데 점 
                bottomLeft = Quaternion.Euler(0, currnetAngle, 0) * Vector3.forward * distance;
                bottomRight = Quaternion.Euler(0, currnetAngle+deltaAngle, 0) * Vector3.forward * distance;

                 TopCenter = bottomCenter + Vector3.up * height; //아래도 똑같이
                 TopRight = bottomRight + Vector3.up * height;
                 TopLeft = bottomLeft + Vector3.up * height;

                //far
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = TopRight;

                vertices[vert++] = TopRight;
                vertices[vert++] = TopLeft;
                vertices[vert++] = bottomLeft;
                //Top
                vertices[vert++] = TopCenter;
                vertices[vert++] = TopLeft;
                vertices[vert++] = TopRight;
                //bottom
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;

                currnetAngle += deltaAngle;
            }
            #endregion
            for (int i = 0; i < numVertices; ++i)
            {
                triangles[i] = i;
            }
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }
        private void OnValidate()
        {
         
            mesh = CreateMesh();
            TriangleCheck();
        }
        private void OnDrawGizmos()
        {
            if (mesh)
            {
                Gizmos.color = meshColor;
                Gizmos.DrawMesh(mesh, this.transform.position,transform.rotation);
            }

            Gizmos.DrawWireSphere(transform.position , distance);

            for (int i = 0; i < playerCount; ++i)
            {
                Gizmos.color = UnityEngine.Color.blue;
                Gizmos.DrawSphere(colliders[i].transform.position ,1f);
            }

            Gizmos.color = UnityEngine.Color.green;
            foreach (var obj in gameObjects)
            {
                Gizmos.DrawSphere(obj.transform.position, 1f);
            }
           
        }

    }
}
