using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public int m_playerNum;

	private float m_playerSpeed = 10.0f;


	void Start()
	{
		Reset();

		RpsData.instance.AddUpdatedVarsListener(UpdateVariablesRecieved);
	}

	public void UpdateVariablesRecieved()
	{
//		Debug.Log("player RECIEVED VARS");
		
		m_playerSpeed = RpsData.instance.GetUpdatedVariable("playerSpeed", m_playerSpeed);
	}

	public void Reset()
	{
	//	renderer.material.color = PongUI.instance.GetPlayerColor(m_playerNum);
		transform.localScale = new Vector3(2.0f, transform.localScale.y, transform.localScale.z);
	}

	void Update()
	{
		Move();
	}

	void Move()
	{
		Vector3 newTarget = new Vector3(InputManager.instance.GetTargetXPosition(m_playerNum), transform.position.y, 0.0f);

		transform.position = Vector3.MoveTowards(transform.position, newTarget, m_playerSpeed * Time.smoothDeltaTime);
	}
}
