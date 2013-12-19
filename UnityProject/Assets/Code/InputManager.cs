using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour 
{
	public static InputManager instance;

	private float m_p1TargetX = 0.0f,
						m_p2TargetX = 0.0f;

	void Awake()
	{
		instance = this;
	}

	void Update()
	{
		#if UNITY_EDITOR
			if(Input.mousePosition.y < Screen.height /2)
			{
				m_p1TargetX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
			}
			else
			{
				m_p2TargetX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;	
			}
		#elif UNITY_ANDROID || UNITY_IPHONE
			for(int i = 0; i < Input.touches.Length; i++)
			{
				if(Input.touches[i].position.y < Screen.height /2)
				{
					m_p1TargetX = Camera.main.ScreenToWorldPoint(Input.touches[i].position).x;
				}
				else
				{
					m_p2TargetX = Camera.main.ScreenToWorldPoint(Input.touches[i].position).x;	
				}
			}
		#endif

		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			if(PongGameManager.instance.isGameStarted())
			{
				PongGameManager.instance.GameOver();
			}
			else if(PongUI.instance.m_showURL)
			{
				PongUI.instance.m_showURL = false;
				PongUI.instance.m_titleObj.gameObject.SetActive(true);
			}
			else
			{
				 Application.Quit(); 
			}
		}
	}

	public float GetTargetXPosition(int _player)
	{
		if(_player == 1)
		{
			return m_p1TargetX;
		}
		else
		{
			return m_p2TargetX;
		}
	}
}
