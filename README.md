RpsData Documentation
==============

This project contains our test game Cake Bounce, and our Unity3D plugin to work with RPS's Aftertouch.

Instructions for use of RpsData Plugin:

* Unpack RpsDataUnityPackage.unitypackage into your existing project.
* Drag the RpsData Prefab into your scene.
* Set the app's name on the RpsData prefab.

To update variables, first create a function like this:

          public void UpdateVariablesRecieved()
          {
          	m_playerSpeed = RpsData.instance.GetUpdatedVariable("playerSpeed", m_playerSpeed);
          }

Then Add this line in Start() in the same class:

          RpsData.instance.AddUpdatedVarsListener(UpdateVariablesRecieved);

  

This will add your UpdateVariablesRecieved function as a listener to updatedVariablesReceivedEvent;

RpsData will attempt to fetch new updated variables each time the app is opened, reverting back to last recieved variables if no internet connection is available.

To request specific data from Aftertouch, call RpsData.instance.FetchDataSet( string _key)


