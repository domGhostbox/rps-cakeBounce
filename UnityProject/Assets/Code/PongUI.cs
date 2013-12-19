using UnityEngine;
using System.Collections;

public class PongUI : MonoBehaviour
{
	public static PongUI instance;
	
	private Color m_playerOneColor,	
						   m_playerTwoColor;

	private int m_playerOneColorNum = 0,	
							m_playerTwoColorNum = 1;

	public GUIStyle m_myStyle,
							   m_titleStyle,
							   m_debugStyle;

	public Color[] m_colorOptions;

	public bool m_showURL;

	public GameObject m_titleObj;

	void Awake()
	{
		instance = this;
	} 

	void Start()
	{
		RpsData.instance.AddUpdatedVarsListener(UpdateVariablesRecieved);

		m_playerOneColor = m_colorOptions[m_playerOneColorNum];
		m_playerTwoColor = m_colorOptions[m_playerTwoColorNum];
	}
	
	public void UpdateVariablesRecieved()
	{
//		Debug.Log("ui RECIEVED VARS");

		m_playerOneColorNum = (int)RpsData.instance.GetUpdatedVariable("p1ColorNum", m_playerOneColorNum);
		m_playerTwoColorNum = (int)RpsData.instance.GetUpdatedVariable("p2ColorNum", m_playerTwoColorNum);

		m_playerOneColor = m_colorOptions[m_playerOneColorNum];
		m_playerTwoColor = m_colorOptions[m_playerTwoColorNum];

		PongGameManager.instance.m_players[0].Reset();
		PongGameManager.instance.m_players[1].Reset();
	}

	void OnGUI()
	{
		if(PongGameManager.instance.isGameStarted())
		{
			//m_myStyle.normal.textColor = m_playerOneColor;
			//GUI.Label(new Rect(Screen.width/2, Screen.height * 0.75f, 100, 50), "" + PongGameManager.instance.m_playerOneScore, m_myStyle);

		//	m_myStyle.normal.textColor = m_playerTwoColor;
		//	GUI.Label(new Rect(Screen.width/2, Screen.height * 0.25f, 100, 50), "" + PongGameManager.instance.m_playerTwoScore,m_myStyle);

			if(Input.touchCount > 4)
			{
				PongGameManager.instance.GameOver();
			}
			
			#if UNITY_EDITOR
			if(Input.GetButtonDown("Jump"))
			{
				PongGameManager.instance.GameOver();
			}
			#endif
		}
		else
		{
			if(m_showURL)
			{
//				string oldAddress = RpsData.instance.m_rpsDataServerAdress;
			//	RpsData.instance.m_rpsDataServerAdress = GUI.TextField(new Rect(0, 0, Screen.width, 100), RpsData.instance.m_rpsDataServerAdress, m_debugStyle);

			//	if(oldAddress != RpsData.instance.m_rpsDataServerAdress)
				{
			//		RpsData.instance.AddressChanged();
				}

				string stringToDraw = ParseRetreievedData();

			//	GUI.Label(new Rect(0, 100, Screen.width, Screen.height), stringToDraw, m_debugStyle);
			
				GUI.Label(new Rect(0, 100, Screen.width, 50), "Response: " + RpsData.instance.GetResponseReceived(), m_debugStyle);
				GUI.Label(new Rect(0, 140, Screen.width, 100), "Configuration: " + RpsData.instance.GetConfigRecieved(), m_debugStyle);
				GUI.Label(new Rect(0, 180, Screen.width, 50), "Request: " + RpsData.instance.GetRequestReceieved(), m_debugStyle);
				GUI.Label(new Rect(0, 260, Screen.width, 50), "Values:", m_debugStyle);

				for(int i = 0; i < RpsData.instance.m_updatedVariables.Count; i++)
				{
					GUI.Label(new Rect(0, 300 + (i * 40), Screen.width, 50), RpsData.instance.m_updatedVariables[i].m_name + " : " + RpsData.instance.m_updatedVariables[i].m_value, m_debugStyle);
				}

				if(GUI.Button(new Rect(0, Screen.height - 100.0f, Screen.width, 100), "TAP HERE TO REFRESH NOW", m_debugStyle))
				{
					StartCoroutine(RpsData.instance.FetchData());
				}
			}
			else
			{
				if(GUI.Button(new Rect(0, 0, Screen.width, Screen.height), "", m_myStyle))
				{
					m_showURL = false;
					PongGameManager.instance.StartGame();
				}
				
				if(PongGameManager.instance.m_winner == 1)
				{
					Vector3 screenPos = Camera.main.WorldToScreenPoint( PongGameManager.instance.m_players[0].transform.position);
					GUI.Label(new Rect(screenPos.x - 100, Screen.height - screenPos.y - 50, 300, 50.0f), "Winner!", m_myStyle);
				}
				else if(PongGameManager.instance.m_winner == 2)
				{
					Vector3 screenPos = Camera.main.WorldToScreenPoint( PongGameManager.instance.m_players[1].transform.position);
					GUI.Label(new Rect(screenPos.x - 100, Screen.height - screenPos.y - 50, 300, 50.0f), "Winner!", m_myStyle);
				}
				/*
				if(Input.touchCount > 3)
				{
					m_showURL = true;
					PongUI.instance.m_titleObj.gameObject.SetActive(false);
				}
				 */
			}
		}
	}

	string ParseRetreievedData()
	{
		string data = RpsData.instance.GetStringRecieved();

		string[] split = data.Split(',');

		string newString = "";

		for(int i = 0; i < split.Length; i++)
		{
			newString += "\n" + split[i];
		}

		return newString;
	}

	public Color GetPlayerColor(int _playerNum)
	{
		if(_playerNum == 1)
		{
			return m_playerOneColor;
		}
		else
		{
			return m_playerTwoColor;
		}
	}
}
