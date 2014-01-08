using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public struct UpdatedVariable
{
	public string 	m_name;
	public string 	m_value;
}

public class RpsData : MonoBehaviour
{
	public static RpsData instance;

	public bool m_enableLogging;

	public string m_appName,
						  m_version;
		

	public List<UpdatedVariable>	m_updatedVariables = new List<UpdatedVariable>();

	private string m_rpsDataServerAdress = "http://aftertouch.io/at/index.jsp"; //make sure whatever link used ends with "/" or ".jsp" ie NOT http://aftertouch.io/at

	private string m_responseRecieved, m_configRecieved, m_requestReceived, m_stringRecieved;

	public static event Action updatedVariablesReceivedEvent;
	
	private List<Action> m_listeners = new List<Action>();

	private bool m_recievedDataSinceSessionStart;

	private string m_storedString;

	void Awake()
	{
		instance = this;

		DontDestroyOnLoad(gameObject);
	}

	void OnApplicationFocus(bool pauseStatus) 
	{
		if(pauseStatus)
		{
			m_recievedDataSinceSessionStart = false;

			UpdateVariables();
		}
	}

	public void UpdateVariables() //will refresh updated variables each time app is opened, falling back to cached variables if no internet is not available
	{

		if(Application.internetReachability != NetworkReachability.NotReachable)
		{
			Log ("Updating Vars - internet Ok Fetching");
			StartCoroutine_Auto(FetchData());
		}
		else
		{
			m_storedString = PlayerPrefs.GetString("rpsDatastoredString");
			LoadVariablesFromPlayerPrefs();
			Log ("Updating vars - no internet - using cached data");
		}
	}

	public void FetchDataSet(string _key)
	{
		StartCoroutine_Auto(FetchData(_key));
	}

	public System.Collections.IEnumerator FetchData(string _key = "")
	{
		Log("RPSDATA Fetching: " + m_rpsDataServerAdress);

		Hashtable device = new Hashtable();
		device.Add("os", SystemInfo.operatingSystem);
		
		Hashtable values = new Hashtable();
		values.Add("a", "1");
		
		Hashtable bodyHash = new Hashtable();
		
		bodyHash.Add("app", m_appName);
		bodyHash.Add("version", m_version);
		bodyHash.Add("installation", SystemInfo.deviceUniqueIdentifier);
		bodyHash.Add("request", System.DateTime.Now.ToString());
		bodyHash.Add("device", device);
		bodyHash.Add("values", values);
		if(_key != "") bodyHash.Add("key", _key);

		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/json");

		string jsonString = MiniJSON.jsonEncode(bodyHash);

		System.Text.UTF8Encoding encoding = new UTF8Encoding();

		WWW www = new WWW( m_rpsDataServerAdress, encoding.GetBytes(jsonString) , headers);
		yield return www; 

		
		if( www.error != null  )
		{
			//and error occured
			Log("wwwError "  + www.error );
		}
		else
		{	
			m_stringRecieved = www.text;
			
			if( m_stringRecieved != "" )
			{
				Log("RPS data complete");

				PlayerPrefs.SetString("rpsDataStringRecieved", m_stringRecieved);

				m_recievedDataSinceSessionStart = true;

				LoadUpdatedVariables();
			}	
			else
			{
				Log("RPS NO DATA RETURNED");
			}
		}
	}

	void LoadUpdatedVariables()
	{
//		Debug.Log("LOADING UPDATED VARIABLES /n"  + m_wwwString);

		ArrayList updatedVariablesArray;
		Hashtable updatedVariablesHashtable;
		UpdatedVariable tempUpdatedVariable = new UpdatedVariable();
		
		Hashtable updateHash = (Hashtable) MiniJSON.jsonDecode( m_stringRecieved );
		if( updateHash == null ) return;

		Hashtable values = (Hashtable)updateHash["values"];

		m_configRecieved = (string)updateHash["configuration"];

		if(updateHash["response"].GetType() == typeof(double))
		{
			double response = (double)updateHash["response"];
			m_responseRecieved =  response.ToString();
		}
		else if(updateHash["response"].GetType() == typeof(string))
		{	
			m_responseRecieved = (string)updateHash["response"];
		}

		m_requestReceived  =  (string)updateHash["request"];

		foreach(DictionaryEntry entry in values)
		{
			tempUpdatedVariable.m_name = (string)entry.Key;
			tempUpdatedVariable.m_value =(string)entry.Value;

			if(IfVariableAlreadyExists(tempUpdatedVariable.m_name))
		   	{
				UpdateExistingVariable(tempUpdatedVariable.m_name, tempUpdatedVariable.m_value);
			}
			else
			{
				m_updatedVariables.Add(tempUpdatedVariable);
			}
		}

		SaveVariablesToPlayerPrefs();

		updatedVariablesReceivedEvent();
	}

	void SaveVariablesToPlayerPrefs()
	{
		string newStoredString ="";

		for(int i = 0; i < m_updatedVariables.Count; i++)
		{
			string newLine = (m_updatedVariables[i].m_name + ":" + m_updatedVariables[i].m_value + ",");
			newStoredString += newLine;
		}

		PlayerPrefs.SetString("rpsDatastoredString", newStoredString);

		Log("Saved " + m_updatedVariables.Count + " variables: " + newStoredString);
	}

	void LoadVariablesFromPlayerPrefs()
	{
		UpdatedVariable tempUpdatedVariable;
		string[] lines = m_storedString.Split(',');

		for(int i = 0; i < lines.Length; i++)
		{
			string[] newVariable = (lines[i]).Split(':');

			if(newVariable[0] != "")
			{
				tempUpdatedVariable.m_name = newVariable[0];
				tempUpdatedVariable.m_value = newVariable[1];
				
				if(IfVariableAlreadyExists(tempUpdatedVariable.m_name))
				{
					UpdateExistingVariable(tempUpdatedVariable.m_name, tempUpdatedVariable.m_value);
				}
				else
				{
					m_updatedVariables.Add(tempUpdatedVariable);
				}
			}
		}

		updatedVariablesReceivedEvent();
	}

	bool IfVariableAlreadyExists(string _name)
	{
		for(int i = 0; i < m_updatedVariables.Count; i++)
		{
			if(m_updatedVariables[i].m_name == _name)
			{
				return true;
			}
		}

		return false;
	}

	void UpdateExistingVariable(string _name, string _value)
	{
		for(int i = 0; i < m_updatedVariables.Count; i++)
		{
			if(m_updatedVariables[i].m_name == _name)
			{
				UpdatedVariable tempUpdatedVariable;
				tempUpdatedVariable.m_name = _name;
				tempUpdatedVariable.m_value = _value;

				m_updatedVariables[i] = tempUpdatedVariable;

				return;
			}
		}
	}

	public string GetUpdatedVariable( string name , string defaultValue )
	{
		if( m_updatedVariables != null )
		{
			for( int i = 0; i < m_updatedVariables.Count; i++ )
			{
				if( m_updatedVariables[i].m_name == name )
				{
					return 	m_updatedVariables[i].m_value;
				}
			}
			return defaultValue;
		}
		else
		{
			return defaultValue;		
		}
	}

	public float GetUpdatedVariable(string name, float defaultValue)
	{
		if( m_updatedVariables != null )
		{
			for( int i = 0; i < m_updatedVariables.Count; i++ )
			{
				if( m_updatedVariables[i].m_name == name )
				{
					float newFloat;
					float.TryParse(m_updatedVariables[i].m_value, out newFloat);
					return newFloat;
				}
			}
		}
	
		return defaultValue;
	}

	public string GetConfigRecieved()
	{
		return m_configRecieved;
	}
	
	public string GetResponseReceived()
	{
		return m_responseRecieved;
	}
	
	public string GetRequestReceieved()
	{
		return m_requestReceived;
	}

	public string GetStringRecieved()
	{
		return m_stringRecieved;
	}

	public bool hasRecivedDataSinceSessionStart()
	{
		return m_recievedDataSinceSessionStart;
	}

	public void AddUpdatedVarsListener(Action _action)
	{
		m_listeners.Add(_action);
		updatedVariablesReceivedEvent += _action;
	}

	void OnEnable()
	{
		for(int i = 0; i < m_listeners.Count; i++)
		{
			updatedVariablesReceivedEvent += m_listeners[i];
		}
	}

	void OnDisable()
	{
		for(int i = 0; i < m_listeners.Count; i++)
		{
			updatedVariablesReceivedEvent -= m_listeners[i];
		}
	}

	void Log(string _log)
	{
		if(m_enableLogging)
		{
			Debug.Log(_log);
		}
	}
}
