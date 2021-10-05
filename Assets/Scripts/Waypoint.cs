using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private Transform sphere;

    private List<Transform> waypoints = new List<Transform>();
    
    void Awake()
    {
        // waypoints 캐싱 
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            waypoints.Add(transform.GetChild(i).gameObject.transform);
        }
        
        // 캐싱한 waypoints를 GameManger에 넘기기.
        GameManager.Instance.Waypoints = waypoints;
        
        SettingWayPoints();
    }
    
    // waypoints를 map표면에 위치시켜줌.
    private void SettingWayPoints()
    {
        float radius = sphere.GetComponent<SphereCollider>().radius;
        float scaleRadius = radius * sphere.localScale.x;
        
        // map의 표면 위치 구해서, waypoint를 위치시켜줌.
        foreach (Transform obj in waypoints)
        {
            Vector3 sphereToPoint = (obj.position - sphere.position).normalized;
            Vector3 toSurfaceVec = scaleRadius * sphereToPoint;

            obj.position = sphere.transform.position + toSurfaceVec;
        }
    }
}
