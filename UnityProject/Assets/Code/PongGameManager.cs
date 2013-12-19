using UnityEngine;
using System.Collections;

public class PongGameManager : MonoBehaviour 
{
	public static PongGameManager instance;

	private int m_pointsForWin = 4;

	public int m_playerOneScore =0,
					m_playerTwoScore= 0;

	public  int m_winner;

	bool m_gameStarted = false;

	public Player[] m_players;


	void Awake()
	{
		instance = this;
	}

	void Reset()
	{
		Ball.instance.Reset();
		m_playerOneScore = 0;
		m_playerTwoScore = 0;

		m_players[0].Reset();
		m_players[1].Reset();
	}
	
	public void PlayerScored(int _playerNum)
	{
		Color playerColor;

		if(_playerNum == 1)
		{
			m_playerOneScore++;
			playerColor = (PongUI.instance.GetPlayerColor(1));
		}
		else
		{
			m_playerTwoScore++;
			playerColor = (PongUI.instance.GetPlayerColor(2));
		}

		m_players[_playerNum - 1].transform.localScale = new Vector3(m_players[_playerNum - 1].transform.localScale.x - 0.5f, m_players[_playerNum - 1].transform.localScale.y, m_players[_playerNum - 1].transform.localScale.z);

		Background.instance.PlayerScored(playerColor);

		if(m_playerOneScore >= m_pointsForWin)
		{
			GameOver();
			m_winner = 1;
		}
		else if(m_playerTwoScore >= m_pointsForWin)
		{
			GameOver();
			m_winner = 2;
		}


	}

	public bool isGameStarted()
	{
		return m_gameStarted;
	}


	void OnApplicationFocus(bool status)
	{
		if(status) //returning to game
		{
			if(m_gameStarted)
			{
				m_winner = 0;
				GameOver();
			}
		}
	}

	public void StartGame()
	{
		m_gameStarted = true;
		Ball.instance.StartMoving();

		PongUI.instance.m_titleObj.gameObject.SetActive(false);
	}

	public void GameOver()
	{
		m_gameStarted = false;
		Reset();

		if(m_winner != 0)
		{
			m_players[m_winner - 1].transform.localScale = new Vector3(2.0f, m_players[m_winner - 1].transform.localScale.y, m_players[m_winner - 1].transform.localScale.z);
		}

		PongUI.instance.m_titleObj.gameObject.SetActive(true);
	}
}
