using System;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
	private float m_LookSpeed = 3;
	private readonly Vector3 m_Origin = new Vector3(0, 0, 0);
	private Vector3 m_LocalX;
	private Vector3 m_LocalY;


	void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			m_LookSpeed = 1;
		}		
	}

	
	public void Look()
	{
		float deltaX = Input.GetAxis("Mouse X") * m_LookSpeed;
		float deltaY = Input.GetAxis("Mouse Y") * m_LookSpeed;

		if (Input.GetMouseButtonDown(0))
		{
			m_LocalX = transform.right.normalized;
			m_LocalY = transform.up.normalized;
		}
		
		if (Input.GetMouseButton(0))
		{
			m_LocalX = transform.right.normalized;
			m_LocalY = transform.up.normalized;
			
			transform.RotateAround(m_Origin, m_LocalX, -deltaY);
			transform.RotateAround(m_Origin, m_LocalY, deltaX);
		}
	}

	private void Update()
	{
		Look();
	}
}
