using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour 
{
	public static Background instance;

	private int m_backgroundColorNum = 5;
				 int m_backgroundPatternNum;
	
	public Material[] m_patterns;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		RpsData.instance.AddUpdatedVarsListener(UpdateVariablesRecieved);
	}
	
	public void UpdateVariablesRecieved()
	{
		//Debug.Log("background RECIEVED VARS");

	//	m_backgroundColorNum = (int)RpsData.instance.GetUpdatedVariable("backgroundColor", m_backgroundColorNum);
	
	//	renderer.material = m_patterns[(int)RpsData.instance.GetUpdatedVariable("backgroundPattern", m_backgroundPatternNum)];
	}

	public void PlayerScored(Color _playerColor)
	{
		renderer.material.color = _playerColor;
	}

	void Update()
	{
	//	renderer.material.color = Color.Lerp(renderer.material.color, PongUI.instance.m_colorOptions[m_backgroundColorNum], Time.smoothDeltaTime);
	}
}
