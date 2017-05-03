using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Text;
using UnityEngine.Events;
#if (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS)
using AOT;
using System.Runtime.InteropServices;
#endif


namespace Ardunity
{
    [AddComponentMenu("ARDUnity/CommSocket/CommBLE")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/utility/commserialui")]
    public class CommBLE : CommSocket
    {
        public float searchTimeout = 5f;
        
        public CommDeviceEvent OnChangedDevice;
    
        private bool _foundTX = false;
        private bool _foundRX = false;
        private bool _foundName = false;
        private bool _isBleOpen = false;
        private static bool _isSupport = true;
        private float _searchTimeout = 0f;        
        private bool _threadOnOpen = false;
        private bool _threadOnOpenFailed = false;
        private bool _threadOnStartSearch = false;
        private bool _threadOnStopSearch = false;
        private bool _threadOnFoundDevice = false;
        private bool _threadOnErrorClosed = false;
        private bool _threadOnChangedDevice = false;
        private bool _threadOnWriteCompleted = false;
        private Thread _openThread;
        private List<byte> _txBuffer = new List<byte>();
        private List<byte> _rxBuffer = new List<byte>();
        private string _changedName;
        private bool _txWait = false;
        private bool _getTxCompleted;

#if UNITY_ANDROID
        private AndroidJavaObject _android = null;
        
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
        private static bool _bleInitialized = false;        
        private static List<CommBLE> _commBleList = new List<CommBLE>();        
	    private delegate void UnityCallbackDelegate(IntPtr arg1, IntPtr arg2);
        
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleInitialize([MarshalAs(UnmanagedType.FunctionPtr)]UnityCallbackDelegate unityCallback);       
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleDeinitialize();       
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleStartScan(string service);       
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleStopScan();
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleConnect(string uuid);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleDisconnect(string uuid);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleDiscoverService(string uuid, string service);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleDiscoverCharacteristic(string uuid, string service, string characteristic);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleWrite(string uuid, string service, string characteristic, byte[] data, int length, bool withResponse);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleRead(string uuid, string service, string characteristic);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleSubscribe(string uuid, string service, string characteristic);
#if UNITY_IOS
	   [DllImport("__Internal")]
#else
	   [DllImport("OsxPlugin")]
#endif
       private static extern void bleUnsubscribe(string uuid, string service, string characteristic);

#endif
        
        protected override void Awake()
		{
            base.Awake();

#if UNITY_ANDROID
            try
            {
                AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass pluginClass = new AndroidJavaClass("com.ardunity.android.BluetoothLE");
                _android = pluginClass.CallStatic<AndroidJavaObject>("GetInstance");
                _isSupport = _android.Call<bool>("Initialize", activityContext, gameObject.name, "BleCallback");
                if (_isSupport == false)
                    _android = null;
            }
            catch(Exception)
            {
                _android = null;
            }

            if(_android == null)
                Debug.Log("Android BLE Failed!");
                
#elif (UNITY_STANDALONE_OSX|| UNITY_IOS)
            _commBleList.Add(this);
            if(_commBleList.Count == 1)
		        bleInitialize(BleCallbackDelegate);
#endif
        }
        
        // Update is called once per frame
        void Update ()
        {
            if(_threadOnOpen)
            {
                OnOpen.Invoke();
                _threadOnOpen = false;
            }
            if (_threadOnOpenFailed)
            {
                ErrorClose();
                OnOpenFailed.Invoke();
                _threadOnOpenFailed = false;
            }
            if (_threadOnErrorClosed)
            {
                ErrorClose();
                OnErrorClosed.Invoke();
                _threadOnErrorClosed = false;
            }
            if (_threadOnStartSearch)
            {
                OnStartSearch.Invoke();
                _threadOnStartSearch = false;
            }
            if (_threadOnStopSearch)
            {
                OnStopSearch.Invoke();
                _threadOnStopSearch = false;
            }
            if (_threadOnFoundDevice)
            {
                OnFoundDevice.Invoke(new CommDevice(foundDevices[foundDevices.Count - 1]));
                _threadOnFoundDevice = false;
            }
            if (_threadOnChangedDevice)
            {
                OnChangedDevice.Invoke(new CommDevice(device));
                _threadOnChangedDevice = false;
            }
            if (_threadOnWriteCompleted)
            {
                OnWriteCompleted.Invoke();
                _threadOnWriteCompleted = false;
            }
            
            if (_searchTimeout > 0f)
            {
                _searchTimeout -= Time.deltaTime;
                if (_searchTimeout <= 0f)
                    StopSearch();
            }
        }
        
        void LateUpdate()
        {
            if(IsOpen && !_txWait)
                txWrite();
        }
        
        void OnDestroy()
        {
#if UNITY_ANDROID

#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
            ErrorClose();
            _commBleList.Remove(this);
            if(_commBleList.Count == 0)
		        bleDeinitialize();            
#endif
        }
        
        public bool isSupport
        {
            get
            {
#if UNITY_ANDROID
                return _isSupport;
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                if(_bleInitialized)
                    return _isSupport;
                else
                    return false;
#else
                return false;
#endif
            }            
        }
        
        public bool isSearching
        {
            get
            {
                if(_searchTimeout > 0f)
                    return true;
                else
                    return false;
            }
        }
        
        #region Override
        public override void Open()
        {
            StopSearch();
            
            if (IsOpen)
                return;

            _openThread = new Thread(openThread);
            _openThread.Start();
        }

        public override void Close()
        {
            if (!IsOpen)
                return;

            ErrorClose();
            OnClose.Invoke();
        }

        protected override void ErrorClose()
        {
            if (_openThread != null)
            {
                if (_openThread.IsAlive)
                    _openThread.Abort();
            }
            
            if(_foundRX)
            {
 #if UNITY_ANDROID
                if (_android != null)
                    _android.Call("UnsubscribeCharacteristic", ArdunityBLE.serviceUUID, ArdunityBLE.rxUUID);                
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                if (_bleInitialized)
                    bleUnsubscribe(device.address, ArdunityBLE.serviceUUID, ArdunityBLE.rxUUID);
#endif
            }
            
			_rxBuffer.Clear();
            
#if UNITY_ANDROID
            if (_android != null)
                _android.Call("Disconnect");                
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)            
            if (_bleInitialized)
                bleDisconnect(device.address);
#endif
            _isBleOpen = false;
            _foundTX = false;
            _foundRX = false;
            _foundName = false;
        }

        public override bool IsOpen
        {
            get
            {
                return _isBleOpen;
            }
        }

        public override void StartSearch()
        {
            _searchTimeout = searchTimeout;

#if UNITY_ANDROID
            if (_android != null)
                _android.Call("StartScan", ArdunityBLE.serviceUUID);                
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
            if (_bleInitialized)
                bleStartScan(ArdunityBLE.serviceUUID);
#endif
        }
        
        public override void StopSearch()
        {
            _searchTimeout = 0f;
            
#if UNITY_ANDROID
            if (_android != null)
                _android.Call("StopScan");                
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
            if (_bleInitialized)
                bleStopScan();
#endif
        }

        public override void Write(byte[] data, bool getCompleted = false)
        {         
            if (data == null)
                return;
            if (data.Length == 0)
                return;
            
            _txBuffer.AddRange(data);
            _getTxCompleted = getCompleted;
        }
        
        private void txWrite()
        {            
            if(_txBuffer.Count == 0)
            {
                _txWait = false;
                if(_getTxCompleted)
                    _threadOnWriteCompleted = true;
                return;
            }
            
            _txWait = true;
            byte[] data20 = new byte[Mathf.Min(20, _txBuffer.Count)];
            for(int i=0; i<data20.Length; i++)
                data20[i] = _txBuffer[i];
            
            _txBuffer.RemoveRange(0, data20.Length);
            
#if UNITY_ANDROID
           if (_android != null)
                _android.Call("Write", ArdunityBLE.serviceUUID, ArdunityBLE.txUUID, data20, false);
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
           if (_bleInitialized)
                bleWrite(device.address, ArdunityBLE.serviceUUID, ArdunityBLE.txUUID, data20, data20.Length, false);
#endif
        }

        public override byte[] Read()
        {
            if(_rxBuffer.Count > 0)
            {
                byte[] bytes = _rxBuffer.ToArray();
                _rxBuffer.Clear();
                return bytes;
            }
            else
                return null;
        }
        #endregion
        
        public void SetDeviceName(string name)
        {
            if(!_isBleOpen)
                return;
                
            byte[] nameBytes = Encoding.ASCII.GetBytes(name);
            byte[] data = new byte[Math.Min(20, nameBytes.Length)];
            for(int i=0; i<data.Length; i++)
                data[i] = nameBytes[i];
            _changedName = Encoding.ASCII.GetString(data);

#if UNITY_ANDROID
            if (_android != null)
                _android.Call("Write", ArdunityBLE.serviceUUID, ArdunityBLE.nameUUID, data, true);                
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
            if (_bleInitialized)
                bleWrite(device.address, ArdunityBLE.serviceUUID, ArdunityBLE.nameUUID, data, data.Length, true);
#endif
        }

        private void openThread()
        {
#if UNITY_ANDROID
            AndroidJNI.AttachCurrentThread();
#endif
            bool openTry = false;
            _txBuffer.Clear();
            _txWait = false;
            
#if UNITY_ANDROID
            if (_android != null)
            {
                _android.Call("Connect", device.address);
                openTry = true;
            }
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
            if (_bleInitialized)
            {
                bleConnect(device.address);
                openTry = true;
            }
#endif

            if (!openTry)
                _threadOnOpenFailed = true;

#if UNITY_ANDROID
            AndroidJNI.DetachCurrentThread();
#endif
            _openThread.Abort();
            return;
        }

#if (UNITY_STANDALONE_OSX || UNITY_IOS)
        [MonoPInvokeCallback(typeof(UnityCallbackDelegate))]
        private static void BleCallbackDelegate(IntPtr arg1, IntPtr arg2)
        {
            string uuid = Marshal.PtrToStringAuto(arg1);
            string message = Marshal.PtrToStringAuto(arg2);

            if(_commBleList.Count > 0)
            {
                for(int i=0; i<_commBleList.Count; i++)
                {
                    if(uuid == null)
                        _commBleList[i].BleCallback(message);
                    else
                    {
                        if(_commBleList[i].device.address.Equals(uuid))
                            _commBleList[i].BleCallback(message);
                    }
                }
            }
            else
                Debug.Log(message);            
        }
#endif
        
        private void BleCallback(string message)
        {
            if (message == null)
                return;
            
            string[] tokens = message.Split(new char[] { '~' });
            if(tokens.Length == 0)
                return;
            
            if(tokens[0].Equals("Initialized"))
            {
                Debug.Log("BLE Initialized");
#if (UNITY_STANDALONE_OSX || UNITY_IOS)                
                _bleInitialized = true;
#endif
            }
            else if(tokens[0].Equals("Deinitialized"))
            {
                Debug.Log("BLE Deinitialized");
#if (UNITY_STANDALONE_OSX || UNITY_IOS)                
                _bleInitialized = false;
#endif
            }
            else if(tokens[0].Equals("NotSupported"))
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS)
                Debug.Log("BLE not supported");
                _isSupport = false;
#endif
            }
            else if(tokens[0].Equals("PoweredOff"))
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS)
                Debug.Log("BLE Power Off");
#endif
            }
            else if(tokens[0].Equals("PoweredOn"))
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS)
                Debug.Log("BLE Power On");
#endif
            }
            else if(tokens[0].Equals("Unauthorized"))
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS)
                Debug.Log("BLE Unauthorized");
#endif
            }
            else if(tokens[0].Equals("StateUnknown"))
            {
#if (UNITY_STANDALONE_OSX || UNITY_IOS)
                Debug.Log("BLE Unauthorized");
#endif
            }
            else if(tokens[0].Equals("StartScan"))
            {
                Debug.Log("BLE Start Scanning");
                foundDevices.Clear();
                _threadOnStartSearch = true;
            }
            else if(tokens[0].Equals("StopScan"))
            {
                Debug.Log("BLE Stop Scanning");
                _threadOnStopSearch = true;
            }
            else if(tokens[0].Equals("ConnectFailed"))
            {
                Debug.Log("BLE GATT Connect Failed");
                _threadOnOpenFailed = true;
            }
            else if(tokens[0].Equals("DiscoveredDevice"))
            {
                CommDevice foundDevice = new CommDevice();
                foundDevice.name = tokens[1];
                foundDevice.address = tokens[2];
                
                for(int i=0; i<foundDevices.Count; i++)
                {
                    if (foundDevices[i].Equals(foundDevice))
                        return;
                }
                
                foundDevices.Add(foundDevice);
                _threadOnFoundDevice = true;
            }
            else if(tokens[0].Equals("Disconnected"))
            {
                Debug.Log("BLE GATT Disconnected");
                
                if(_isBleOpen)
                    _threadOnErrorClosed = true;
            }
            else if(tokens[0].Equals("Connected"))
            {
                Debug.Log("BLE GATT Connected");
              
#if UNITY_ANDROID
                if (_android != null)
                    _android.Call("DiscoverService", ArdunityBLE.serviceUUID);
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                if (_bleInitialized)
                    bleDiscoverService(device.address, ArdunityBLE.serviceUUID);
#endif
            }
            else if(tokens[0].Equals("DiscoveredService"))
            {
                Debug.Log("BLE Discovered Service");
                              
#if UNITY_ANDROID
                if (_android != null)
                    _android.Call("DiscoverCharacteristic", ArdunityBLE.serviceUUID, ArdunityBLE.txUUID);
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                if (_bleInitialized)
                    bleDiscoverCharacteristic(device.address, ArdunityBLE.serviceUUID, ArdunityBLE.txUUID);
#endif
            }
            else if(tokens[0].Equals("ErrorDiscoveredService"))
            {
                Debug.Log("BLE Discovered Service Error: " + tokens[1]);
                _threadOnOpenFailed = true;
            }
            else if(tokens[0].Equals("DiscoveredCharacteristic"))
            {
                Debug.Log("BLE Discovered Characteristic");
                if(!_foundTX)
                {
                    _foundTX = true;
                    
#if UNITY_ANDROID
                    if (_android != null)
                        _android.Call("DiscoverCharacteristic", ArdunityBLE.serviceUUID, ArdunityBLE.rxUUID);
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                    if (_bleInitialized)
                        bleDiscoverCharacteristic(device.address, ArdunityBLE.serviceUUID, ArdunityBLE.rxUUID);
#endif
                }
                else if(!_foundRX)
                {
                    _foundRX = true;
                    
#if UNITY_ANDROID
                    if (_android != null)
                    {
                        _android.Call("SubscribeCharacteristic", ArdunityBLE.serviceUUID, ArdunityBLE.rxUUID);
                        _android.Call("DiscoverCharacteristic", ArdunityBLE.serviceUUID, ArdunityBLE.nameUUID);
                    }                        
#elif (UNITY_STANDALONE_OSX || UNITY_IOS)
                    if (_bleInitialized)
                    {
                        bleSubscribe(device.address, ArdunityBLE.serviceUUID, ArdunityBLE.rxUUID);
                        bleDiscoverCharacteristic(device.address, ArdunityBLE.serviceUUID, ArdunityBLE.nameUUID);
                    }
#endif
                }
                else if(!_foundName)
                {
                    _foundName = true;

                    _isBleOpen = true;
                    _threadOnOpen = true;
                }
            }
            else if(tokens[0].Equals("ErrorDiscoverCharacteristic"))
            {
                Debug.Log("BLE Discovered Characteristic Error: " + tokens[1]);
                _threadOnOpenFailed = true;
            }
            else if(tokens[0].Equals("ErrorWrite"))
            {
                Debug.Log("BLE Write Error: " + tokens[1]);
                _threadOnErrorClosed = true;
            }
            else if(tokens[0].Equals("Write"))
            {
                if(string.Compare(ArdunityBLE.txUUID, tokens[1], true) == 0)
                {
                    txWrite();
                }
                else if(string.Compare(ArdunityBLE.nameUUID, tokens[1], true) == 0)
                {
                    device.name = _changedName;
                    _threadOnChangedDevice = true;
                }
            }
            else if(tokens[0].Equals("ErrorRead"))
            {
                Debug.Log("BLE Read Error: " + tokens[1]);
                _threadOnErrorClosed = true;
            }
            else if(tokens[0].Equals("Read"))
            {
                byte[] base64Bytes = Convert.FromBase64String(tokens[2]);
                if(base64Bytes.Length > 0)
                {
                    if(string.Compare(ArdunityBLE.rxUUID, tokens[1], true) == 0)
                    {
                        _rxBuffer.AddRange(base64Bytes);
                    }
                }
            }
        }
    }
}