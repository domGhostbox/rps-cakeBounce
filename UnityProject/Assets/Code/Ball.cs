using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour 
{
	public static Ball instance;

	float m_startForce = 2.0f,
			 m_speedIncreaseRate = 0.3f,
			m_bounceSensitivity = 3.0f;

	Vector3 m_velocity;

	Vector3 m_rotationDir;

	int m_lastPlayerHit = 0;


	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		RpsData.instance.AddUpdatedVarsListener(UpdateVariablesRecieved);

		gameObject.SetActive(false);
	}

	public void UpdateVariablesRecieved()
	{
	//	Debug.Log("BALL RECIEVED VARS");

		m_speedIncreaseRate = RpsData.instance.GetUpdatedVariable("ballSpeedInc", m_speedIncreaseRate);
		m_bounceSensitivity = RpsData.instance.GetUpdatedVariable("ballBounceSen", m_bounceSensitivity);
	}

	public void Reset()
	{
		transform.position = Vector3.zero;
		rigidbody.velocity = Vector3.zero; //dont use actual unity rigidbody velocity 
		gameObject.SetActive(false);
		m_lastPlayerHit = 0;
	}

	public void StartMoving(float _movDir = 1.0f)
	{
		transform.position = Vector3.zero;

		Vector3 startVel = new Vector3(Random.Range(-1.0f, 1.0f), _movDir, 0.0f) * m_startForce;

		rigidbody.velocity = Vector3.zero; //dont use actual unity rigidbody velocity 

		m_velocity = startVel;

		m_rotationDir = new Vector3(0.0f, 0.0f, Random.Range(0, 360.0f));

		gameObject.SetActive(true);

		m_lastPlayerHit = 0;
	}

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(m_lastPlayerHit != GetNewPlayerNum(other.gameObject.name))
		   {
				rigidbody.velocity = Vector3.zero;

				float xDiff = other.transform.position.x - transform.position.x;

				m_velocity = new Vector3(-xDiff * m_bounceSensitivity, -m_velocity.y, 0.0f);

				m_lastPlayerHit = GetNewPlayerNum(other.gameObject.name);	
			}
		}
		else
		{
			m_velocity = new Vector3(-m_velocity.x, m_velocity.y, 0.0f);
		}

		m_rotationDir = new Vector3(0.0f, 0.0f, Random.Range(0, 360.0f));
	}

	int GetNewPlayerNum(string _name)
	{
		if(_name == "PlayerOne")
		{
			return 1;
		}
		else
		{
			return 2;
		}
	}

	void Update()
	{
		if(PongGameManager.instance.isGameStarted())
		{
			transform.position += m_velocity * Time.smoothDeltaTime;

			m_velocity *= (1  + Time.smoothDeltaTime * m_speedIncreaseRate); 

			transform.Rotate(m_rotationDir * Time.smoothDeltaTime);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.name == "Player1Trigger")
		{
			Debug.Log("PLAYER TWO SCORED");
			StartMoving(1.0f);
			PongGameManager.instance.PlayerScored(2);
		}
		else
		{
			Debug.Log("PLAYER ONE SCORED");
			StartMoving(-1.0f);
			PongGameManager.instance.PlayerScored(1);
		}
	}

	void OnEnable()
	{

	}

	void OnDisable()
	{

	}
}
